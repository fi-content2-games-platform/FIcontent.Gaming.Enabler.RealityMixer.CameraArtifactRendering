/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// This is the main behaviour class that encapsulates text recognition behaviour.
/// It just has to be added to a Vuforia-enabled Unity scene and will initialize the text tracker with the configured word list.
/// Events for newly recognized or lost words will be called on registered ITextRecoEventHandlers
/// </summary> 
public class TextRecoBehaviour : MonoBehaviour, ITrackerEventHandler, IEditorTextRecoBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES

    private bool mHasInitializedOnce = false;


    [SerializeField]
    [HideInInspector]
    private string mWordListFile;

    [SerializeField]
    [HideInInspector]
    private string mCustomWordListFile;

    [SerializeField]
    [HideInInspector]
    private string mAdditionalCustomWords;

    [SerializeField]
    [HideInInspector]
    private WordFilterMode mFilterMode;

    [SerializeField] 
    [HideInInspector] 
    private string mFilterListFile;

    [SerializeField]
    [HideInInspector]
    private string mAdditionalFilterWords;

    [SerializeField]
    [HideInInspector]
    private WordPrefabCreationMode mWordPrefabCreationMode;

    [SerializeField]
    [HideInInspector]
    private int mMaximumWordInstances;

    private List<ITextRecoEventHandler> mTextRecoEventHandlers =
                            new List<ITextRecoEventHandler>();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PROPERTIES

    public bool IsInitialized
    {
        get { return mHasInitializedOnce; }
    }

    #endregion // PROPERTIES



    #region UNITY_MONOBEHAVIOUR_METHODS

    void Awake()
    {
        if (!QCARRuntimeUtilities.IsQCAREnabled())
        {
            return;
        }

        if (QCARRuntimeUtilities.IsPlayMode())
        {
            // initialize QCAR 
            QCARUnity.CheckInitializationError();
        }

        // In Unity 4.x, if the QCARBehaviour is already active from a previous scene,
        // we need to deactivate it briefly to be able to initialize the text tracker
        bool enableQCARBehaviourAgain = false;
        QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
        if (qcarBehaviour && qcarBehaviour.enabled)
        {
            qcarBehaviour.enabled = false;
            enableQCARBehaviourAgain = true;
        }

        if (TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER) == null)
        {
            TrackerManager.Instance.InitTracker(Tracker.Type.TEXT_TRACKER);
        }

        // restart the QCARBehaviour if it was stopped again
        if (enableQCARBehaviourAgain)
        {
            qcarBehaviour.enabled = true;
        }
    }

    // This method registers the TextRecoBehaviour at the QCARBehaviour
    void Start()
    {
        // keeps the text reco object alive across scenes:
        if (KeepAliveBehaviour.Instance != null && KeepAliveBehaviour.Instance.KeepTextRecoBehaviourAlive)
            DontDestroyOnLoad(gameObject);

        QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.RegisterTrackerEventHandler(this);
        }
    }


    // Restart tracker if the Behaviour is reenabled.
    // Note that we check specifically that OnInitialized() has been called once before
    void OnEnable()
    {
        if (mHasInitializedOnce)
        {
            StartTextTracker();
        }
    }


    // OnApplicaitonPause does not work reliably on desktop OS's - on windows it never gets called,
    // on Mac only if the window focus is lost and Play mode was paused (or resumed!) before.
#if !UNITY_EDITOR

    // Stops the tracker when the application is sent to the background.
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopTextTracker();
        }
        else
        {
            StartTextTracker();
        }
    }

#endif


    // Stop the tracker when the Behaviour is disabled.
    void OnDisable()
    {
        StopTextTracker();
    }

    // Deinitialize the tracker when the Behaviour is destroyed.
    void OnDestroy()
    {

        // unregister from the QCARBehaviour
        QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.UnregisterTrackerEventHandler(this);
        }

        // unload word list which was specific to this behaviour
        var tracker = TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER);
        if (tracker != null)
        {
            var wordList = ((TextTracker) tracker).WordList;
            wordList.UnloadAllLists();
        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    /// <summary>
    /// This method registers a new TextReco event handler.
    /// These handlers are called after all trackables have been updated for this frame.
    /// </summary>
    public void RegisterTextRecoEventHandler(ITextRecoEventHandler trackableEventHandler)
    {
        mTextRecoEventHandlers.Add(trackableEventHandler);
        // call initialized callback immediately if already initialized
        if (mHasInitializedOnce)
            trackableEventHandler.OnInitialized();
    }


    /// <summary>
    /// This method unregisters a TextReco event handler.
    /// Returns "false" if event handler does not exist.
    /// </summary>
    public bool UnregisterTextRecoEventHandler(ITextRecoEventHandler trackableEventHandler)
    {
        return mTextRecoEventHandlers.Remove(trackableEventHandler);
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void StartTextTracker()
    {
        Debug.Log("Starting Text Tracker");

        if (TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER) != null)
        {
            TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER).Start();
        }
    }

    private void StopTextTracker()
    {
        Debug.Log("Stopping Text Tracker");

        if (TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER) != null)
        {
            TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER).Stop();
        }
    }

    /// <summary>
    /// Setup word list, custom word list, and filter list based on information from editor inspector
    /// </summary>
    private void SetupWordList()
    {
        //get all wordlist-information from editor and forward it to QCAR
        var tracker = TrackerManager.Instance.GetTracker(Tracker.Type.TEXT_TRACKER);
        if (tracker != null && tracker is TextTracker)
        {
            var wordList = ((TextTracker) tracker).WordList;

            wordList.LoadWordListFile(mWordListFile);
            if(mCustomWordListFile != "")
                wordList.AddWordsFromFile(mCustomWordListFile);
            if (mAdditionalCustomWords != null)
            {
                var customWords = mAdditionalCustomWords.Split('\r', '\n');
                foreach (var word in customWords)
                    if(word.Length > 0) wordList.AddWord(word);
            }

            wordList.SetFilterMode(mFilterMode);
            if (mFilterMode != WordFilterMode.NONE)
            {
                if (mFilterListFile != "")
                    wordList.LoadFilterListFile(mFilterListFile);

                if (mAdditionalFilterWords != null)
                {
                    var filterWords = mAdditionalFilterWords.Split('\n');
                    foreach (var word in filterWords)
                        if (word.Length > 0) wordList.AddWordToFilterList(word);
                }

            }
        }
    }

    /// <summary>
    /// Notifies all registered text reco event handlers about new and lost words
    /// </summary>
    private void NotifyEventHandlersOfChanges(IEnumerable<Word> lostWords, IEnumerable<WordResult> newWords)
    {
        foreach (Word lostWord in lostWords)
        {
            foreach (var handler in mTextRecoEventHandlers)
                handler.OnWordLost(lostWord);
        }

        foreach (WordResult newWordResult in newWords)
        {
            foreach (var handler in mTextRecoEventHandlers)
                handler.OnWordDetected(newWordResult);
        }
    }

    #endregion // PRIVATE_METHODS



    #region ITrackerEventHandler_IMPLEMENTATION

    public void OnInitialized()
    {
        // QCAR has initialized, now setup the wordlist based on user-defined properties and start the text tracker
        SetupWordList();
        StartTextTracker();

        mHasInitializedOnce = true;

        var wordManager = TrackerManager.Instance.GetStateManager().GetWordManager();
        ((WordManagerImpl)wordManager).InitializeWordBehaviourTemplates(mWordPrefabCreationMode, mMaximumWordInstances);

        // tell handlers we finished initializing
        foreach (var handler in mTextRecoEventHandlers)
            handler.OnInitialized();
    }

    /// <summary>
    /// This method is called after all trackables have been updated
    /// </summary>
    public void OnTrackablesUpdated()
    {
        var wordManager = (WordManagerImpl)TrackerManager.Instance.GetStateManager().GetWordManager();

        //pull information about new and lost words from word manager
        //the word manager has already been updated as all trackables have been updated
        var newWords = wordManager.GetNewWords();
        var lostWords = wordManager.GetLostWords();

        //call event handlers for new/lost words
        NotifyEventHandlersOfChanges(lostWords, newWords);
    }

    #endregion // ITrackerEventHandler_IMPLEMENTATION



    #region EDITOR_INTERFACE_IMPLEMENTATION

    string IEditorTextRecoBehaviour.WordListFile
    {
        get { return mWordListFile; }
        set { mWordListFile = value; }
    }

    string IEditorTextRecoBehaviour.CustomWordListFile
    {
        get { return mCustomWordListFile; }
        set { mCustomWordListFile = value; }
    }

    string IEditorTextRecoBehaviour.AdditionalCustomWords
    {
        get { return mAdditionalCustomWords; }
        set { mAdditionalCustomWords = value; }
    }

    WordFilterMode IEditorTextRecoBehaviour.FilterMode
    {
        get { return mFilterMode; }
        set { mFilterMode = value; }
    }

    string IEditorTextRecoBehaviour.FilterListFile
    {
        get { return mFilterListFile; }
        set { mFilterListFile = value; }
    }

    string IEditorTextRecoBehaviour.AdditionalFilterWords
    {
        get { return mAdditionalFilterWords; }
        set { mAdditionalFilterWords = value; }
    }

    WordPrefabCreationMode IEditorTextRecoBehaviour.WordPrefabCreationMode
    {
        get { return mWordPrefabCreationMode; }
        set { mWordPrefabCreationMode = value; }
    }

    int IEditorTextRecoBehaviour.MaximumWordInstances
    {
        get { return mMaximumWordInstances; }
        set { mMaximumWordInstances = value; }
    }

    #endregion //EDITOR_INTERFACE_IMPLEMENTATION

}