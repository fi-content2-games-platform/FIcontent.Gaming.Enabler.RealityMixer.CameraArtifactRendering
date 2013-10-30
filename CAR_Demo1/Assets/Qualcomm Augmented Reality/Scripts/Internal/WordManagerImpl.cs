/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public enum WordPrefabCreationMode
{
    NONE,
    DUPLICATE
}

public class WordManagerImpl : WordManager
{
    #region PRIVATE_MEMBER_VARIABLES

    private readonly Dictionary<int, WordResult> mTrackedWords = new Dictionary<int, WordResult>();
    private readonly List<WordResult> mNewWords = new List<WordResult>();
    private readonly List<Word> mLostWords = new List<Word>();
    private readonly Dictionary<int, WordBehaviour> mActiveWordBehaviours = new Dictionary<int, WordBehaviour>();
    private readonly List<WordBehaviour> mWordBehavioursMarkedForDeletion = new List<WordBehaviour>(); 

    // list of words that cannot be associated with a behaviour yet (because none was available),
    // but should get priority over new results when a behaviour becomes available
    private readonly List<Word> mWaitingQueue = new List<Word>();

    private const string TEMPLATE_IDENTIFIER = "Template_ID";
    private readonly Dictionary<string, List<WordBehaviour>> mWordBehaviours = new Dictionary<string, List<WordBehaviour>>();
    // whether word template has been created automatically or has been defined by developer
    private bool mAutomaticTemplate = false;

    // maximum number of instances per word behaviour
    private int mMaxInstances = 1;
    
    // whether word prefabs are used or not
    private WordPrefabCreationMode mWordPrefabCreationMode;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    /// <summary>
    /// Returns all currently tracked WordResults
    /// </summary>
    public override IEnumerable<WordResult> GetActiveWordResults()
    {
        return mTrackedWords.Values;
    }

    /// <summary>
    /// Returns all words that have been newly detected in the last frame
    /// </summary>
    public override IEnumerable<WordResult> GetNewWords()
    {
        return mNewWords;
    }

    /// <summary>
    /// Returns all words that have been lost in the last frame and won't be tracked anymore
    /// </summary>
    public override IEnumerable<Word> GetLostWords()
    {
        return mLostWords;
    }

    /// <summary>
    /// Get the word behaviour that is associated with a currently tracked word
    /// </summary>
    /// <param name="word">trackable</param>
    /// <param name="behaviour">resulting word behaviour, might be null if specified word is not associated to a behaviour</param>
    /// <returns>returns true if word behaviour exists for specified word trackable</returns>
    public override bool TryGetWordBehaviour(Word word, out WordBehaviour behaviour)
    {
        return mActiveWordBehaviours.TryGetValue(word.ID, out behaviour);
    }

    /// <summary>
    /// Returns all currently instantiated word behaviours
    /// </summary>
    public override IEnumerable<WordBehaviour> GetTrackableBehaviours()
    {
        var list = new List<WordBehaviour>();
        foreach(var item in mWordBehaviours.Values)
            list.AddRange(item);
        return list;
    }

    /// <summary>
    /// Remove a specific word behaviour from list and optionally destroy game object.
    /// This might also remove a template, i.e. no further trackables will be augmented for a specific word or for a template
    /// </summary>
    public override void DestroyWordBehaviour(WordBehaviour behaviour, bool destroyGameObject = true)
    {
        var keys = mWordBehaviours.Keys.ToArray();
        foreach (var key in keys)
        {
            if (mWordBehaviours[key].Contains(behaviour))
            {
                mWordBehaviours[key].Remove(behaviour);

                // no behaviours left - delete whole list
                if (mWordBehaviours[key].Count == 0)
                    mWordBehaviours.Remove(key);

                if (destroyGameObject)
                {
                    Object.Destroy(behaviour.gameObject);
                    mWordBehavioursMarkedForDeletion.Add(behaviour);
                }
                else
                {
                    IEditorWordBehaviour editorTrackableBehaviour = behaviour;
                    editorTrackableBehaviour.UnregisterTrackable();
                }
            }
        }
    }

    #region INTERNAL_METHODS


    /// <summary>
    /// Gathers all Word Prefabs and create instances up to the specified number
    /// </summary>
    /// <param name="wordPrefabCreationMode">whether word prefabs are used or not</param>
    /// <param name="maxInstances">Maximum instances per prefab</param>
    public void InitializeWordBehaviourTemplates(WordPrefabCreationMode wordPrefabCreationMode, int maxInstances)
    {
        mWordPrefabCreationMode = wordPrefabCreationMode;
        mMaxInstances = maxInstances;
        InitializeWordBehaviourTemplates();
    }

    public void InitializeWordBehaviourTemplates()
    {
        // if word prefabs are used, initialize them:
        if (mWordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE)
        {
            // delete all automatically created templates - these shouldn't be used as real templates
            var destroyedWords = mWordBehavioursMarkedForDeletion.ToList();
            if (mAutomaticTemplate && mWordBehaviours.ContainsKey(TEMPLATE_IDENTIFIER))
            {
                foreach (var behaviour in mWordBehaviours[TEMPLATE_IDENTIFIER])
                {
                    // game objects are not destroyed immediately, i.e. we have to prevent further usage of them
                    destroyedWords.Add(behaviour);
                    Object.Destroy(behaviour.gameObject);
                }
                mWordBehaviours.Remove(TEMPLATE_IDENTIFIER);
            }


            var wordBehaviours = (WordBehaviour[])Object.FindObjectsOfType(typeof(WordBehaviour));

            foreach (var wb in wordBehaviours)
            {
                // don't use word behaviour if it has been destroyed already
                if (destroyedWords.Contains(wb))
                    continue;

                var ewb = (IEditorWordBehaviour)wb;
                var text = ewb.IsTemplateMode ? TEMPLATE_IDENTIFIER : ewb.SpecificWord.ToLowerInvariant();

                if (!mWordBehaviours.ContainsKey(text))
                {
                    mWordBehaviours[text] = new List<WordBehaviour> { wb };
                    if (text == TEMPLATE_IDENTIFIER)
                        mAutomaticTemplate = false;
                }
            }

            //create default template if not provided by developer
            if (!mWordBehaviours.ContainsKey(TEMPLATE_IDENTIFIER))
            {
                var defaultBehaviour = CreateWordBehaviour();
                mWordBehaviours.Add(TEMPLATE_IDENTIFIER, new List<WordBehaviour> { defaultBehaviour });
                mAutomaticTemplate = true;
            }
        }
        mWordBehavioursMarkedForDeletion.Clear();
    }

    /// <summary>
    /// Remove all word behaviours from the dictionary which are not valid anymore.
    /// </summary>
    public void RemoveDestroyedTrackables()
    {
        foreach (var list in mWordBehaviours.Values)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == null)
                    list.RemoveAt(i);
        }
        var keys = mWordBehaviours.Keys.ToArray();
        foreach (var key in keys)
            if (mWordBehaviours[key].Count == 0)
                mWordBehaviours.Remove(key);
    }

    /// <summary>
    /// Update currently tracked words
    /// </summary>
    /// <param name="arCamera">Word trackables will be positioned relative to this camera</param>
    /// <param name="newWordData">words that have been newly detected in the last frame</param>
    /// <param name="wordResults">all words that are currently tracked</param>
    public void UpdateWords(Camera arCamera, QCARManagerImpl.WordData[] newWordData, QCARManagerImpl.WordResultData[] wordResults)
    {
        UpdateWords(newWordData, wordResults);
        UpdateWordResultPoses(arCamera, wordResults);
    }



    /// <summary>
    /// Marks all WordBehaviours as "not found"
    /// </summary>
    public void SetWordBehavioursToNotFound()
    {
        foreach (var trackableBehaviour in mActiveWordBehaviours.Values)
        {
            trackableBehaviour.OnTrackerUpdate(TrackableBehaviour.Status.NOT_FOUND);
        }
    }

    #endregion // INTERNAL_METHODS

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void UpdateWords(IEnumerable<QCARManagerImpl.WordData> newWordData, IEnumerable<QCARManagerImpl.WordResultData> wordResults)
    {
        mNewWords.Clear();
        mLostWords.Clear();

        //add new words to list of tracked words and to list of new words
        foreach (var wd in newWordData)
        {
            if (!mTrackedWords.ContainsKey(wd.id))
            {
                var word = new WordImpl(wd.id, Marshal.PtrToStringUni(wd.stringValue), wd.size);
                var wordResult = new WordResultImpl(word);
                mTrackedWords.Add(wd.id, wordResult);
                mNewWords.Add(wordResult);
            }
        }

        //remove lost words from list of tracked words, add them to list of lost words
        var trackedKeys = new List<int>(); //tracked keys in current frame
        foreach (var wr in wordResults)
            trackedKeys.Add(wr.id);
        var allKeys = mTrackedWords.Keys.ToList(); //keys tracked in previous frame
        foreach (var id in allKeys)
        {
            if (!trackedKeys.Contains(id))
            {
                mLostWords.Add(mTrackedWords[id].Word);
                mTrackedWords.Remove(id);
            }
        }

        // update word behaviours if enabled:
        if (mWordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE)
        {
            // unregister lost words from behaviours
            UnregisterLostWords();

            // associate word results with behaviours:
            AssociateWordResultsWithBehaviours();
        }
    }

    private void UpdateWordResultPoses(Camera arCamera, IEnumerable<QCARManagerImpl.WordResultData> wordResults)
    {

        QCARBehaviour qcarbehaviour = (QCARBehaviour) Object.FindObjectOfType(typeof (QCARBehaviour));
        if (qcarbehaviour == null)
        {
            Debug.LogError("QCAR Behaviour could not be found");
            return;
        }

        // required information to transform camera frame coordinates into screen space coordinates:
        Rect bgTextureViewPortRect = qcarbehaviour.GetViewportRectangle();
        bool isMirrored = qcarbehaviour.VideoBackGroundMirrored;
        CameraDevice.VideoModeData videoModeData = qcarbehaviour.GetVideoMode();

        foreach (var wrd in wordResults)
        {
            var wordResult = (WordResultImpl) mTrackedWords[wrd.id];

            var position = arCamera.transform.TransformPoint(wrd.pose.position);

            var wrdOrientation = wrd.pose.orientation;
            var rotation = arCamera.transform.rotation*
                           wrdOrientation*
                           Quaternion.AngleAxis(270, Vector3.left);

            wordResult.SetPose(position, rotation);
            wordResult.SetStatus(wrd.status);

            var obb = new OrientedBoundingBox(wrd.orientedBoundingBox.center, wrd.orientedBoundingBox.halfExtents,
                                              wrd.orientedBoundingBox.rotation);
            wordResult.SetObb(QCARRuntimeUtilities.CameraFrameToScreenSpaceCoordinates(obb, bgTextureViewPortRect,
                                                                                       isMirrored, videoModeData));
        }

        // update word behaviours if enabled:
        if (mWordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE)
            UpdateWordBehaviourPoses();
    }


    /// <summary>
    /// Associate multiple words with word behaviours. This function prefers word results that are already in the waiting queue over new words.
    /// </summary>
    private void AssociateWordResultsWithBehaviours()
    {
        //first handle all items in the waiting queue (has to be copied because it will be changed)
        //new values will be added to the waiting queue inside AssociateWordBehaviour
        var waitingQueue = new List<Word>(mWaitingQueue);
        foreach (var word in waitingQueue)
        {
            if (mTrackedWords.ContainsKey(word.ID))
            {
                var wr = mTrackedWords[word.ID];
                //the word can be removed from the waiting list because it has been associated with a behaviour
                if (AssociateWordBehaviour(wr) != null)
                    mWaitingQueue.Remove(word);
            }
            else
            {
                //word is not tracked anymore
                mWaitingQueue.Remove(word);
            }
        }

        //handle new word results that are currently tracked
        foreach (WordResult wordResult in mNewWords)
        {
            WordBehaviour wordBehaviour = AssociateWordBehaviour(wordResult);

            //no corresponding word prefab was found. put tracked word result to waiting list
            if (wordBehaviour == null)
                mWaitingQueue.Add(wordResult.Word);
        }
    }

    /// <summary>
    /// Unregister lost words and remove the association with a word behaviour
    /// </summary>
    private void UnregisterLostWords()
    {
        foreach (Word word in mLostWords)
        {
            if (mActiveWordBehaviours.ContainsKey(word.ID))
            {
                var wb = mActiveWordBehaviours[word.ID];
                wb.OnTrackerUpdate(TrackableBehaviour.Status.NOT_FOUND);
                ((IEditorWordBehaviour) wb).UnregisterTrackable();
                mActiveWordBehaviours.Remove(word.ID);
            }
        }
    }


    private void UpdateWordBehaviourPoses()
    {
        foreach (var iwb in mActiveWordBehaviours)
        {
            if (mTrackedWords.ContainsKey(iwb.Key))
            {
                var word = mTrackedWords[iwb.Key];
                var pos = word.Position;
                var rot = word.Orientation;
                var size = word.Word.Size;

                iwb.Value.transform.rotation = rot;

                //geometry of word prefab is not centered, because textmesh starts on left top corner
                //therefore we have to translate the mesh manually
                var offset = iwb.Value.transform.rotation * new Vector3(-size.x * 0.5f, 0f, -size.y * 0.5f);
                iwb.Value.transform.position = pos + offset;

                // trigger OnTrackerUpdate event 
                iwb.Value.OnTrackerUpdate(word.CurrentStatus);
            }
        }
    }


    /// <summary>
    /// Associate a word result with a word behaviour
    /// </summary>
    private WordBehaviour AssociateWordBehaviour(WordResult wordResult)
    {
        //find possible word prefabs based on string value of word trackable
        var stringValue = wordResult.Word.StringValue.ToLowerInvariant();
        List<WordBehaviour> wordBehaviours;
        if (mWordBehaviours.ContainsKey(stringValue))
            wordBehaviours = mWordBehaviours[stringValue];
        else if (mWordBehaviours.ContainsKey(TEMPLATE_IDENTIFIER))
            wordBehaviours = mWordBehaviours[TEMPLATE_IDENTIFIER];
        else
        {
            Debug.Log("No prefab available for string value " + stringValue);
            return null;
        }

        //go over all existing word-behaviour in scene
        foreach (var wordBehaviour in wordBehaviours)
        {
            if (wordBehaviour.Trackable == null)
            {
                //found corresponding word-behaviour that is not already used
                return AssociateWordBehaviour(wordResult, wordBehaviour);
            }
        }

        //no word-behaviour could be used, so we instantiate a new one
        if (wordBehaviours.Count < mMaxInstances)
        {
            var wb = InstantiateWordBehaviour(wordBehaviours.First());
            wordBehaviours.Add(wb);

            return AssociateWordBehaviour(wordResult, wb);
        }

        //no word-behaviour available and pool of possible behaviours is full
        return null;
    }

    private WordBehaviour AssociateWordBehaviour(WordResult wordResult, WordBehaviour wordBehaviourTemplate)
    {
        if (mActiveWordBehaviours.Count >= mMaxInstances)
            return null;

        var word = wordResult.Word;

        var wordBehaviour = wordBehaviourTemplate;
        var ewb = (IEditorWordBehaviour) wordBehaviour;

        ewb.SetNameForTrackable(word.StringValue);
        ewb.InitializeWord(word);

        mActiveWordBehaviours.Add(word.ID, wordBehaviour);

        return wordBehaviour;
    }



    /// <summary>
    /// create a new instance of the input word behaviour
    /// </summary>
    private static WordBehaviour InstantiateWordBehaviour(WordBehaviour input)
    {
        var go = Object.Instantiate(input.gameObject) as GameObject;
        return go.GetComponent<WordBehaviour>();
    }

    /// <summary>
    /// creates a game object with a word behaviour
    /// </summary>
    private static WordBehaviour CreateWordBehaviour()
    {
        var wordObject = new GameObject("Word-AutoTemplate");
        var newWb = wordObject.AddComponent<WordBehaviour>();

        Debug.Log("Creating Word Behaviour");

        return newWb;
    }

    #endregion
}

