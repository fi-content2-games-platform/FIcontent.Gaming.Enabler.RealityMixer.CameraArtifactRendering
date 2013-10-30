/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The KeepAliveBehaviour allows Vuforia objects to be reused across multiple
/// scenes. This makes it possible to share datasets and targets between scenes.
/// </summary>
[RequireComponent(typeof (QCARBehaviour))]
public class KeepAliveBehaviour : MonoBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    private bool mKeepARCameraAlive;

    [SerializeField]
    [HideInInspector]
    private bool mKeepTrackableBehavioursAlive;

    [SerializeField]
    [HideInInspector]
    private bool mKeepTextRecoBehaviourAlive;

    [SerializeField]
    [HideInInspector]
    private bool mKeepUDTBuildingBehaviourAlive;

    [SerializeField]
    [HideInInspector]
    private bool mKeepCloudRecoBehaviourAlive;
    
    private static KeepAliveBehaviour sKeepAliveBehaviour;

    // a list of registered handlers that will be notified when a level is loaded
    private readonly List<ILoadLevelEventHandler> mHandlers = new List<ILoadLevelEventHandler>();

    #endregion



    #region PROPERTIES

    /// <summary>
    /// If the ARCamera should be kept alive on scene change
    /// Changing this parameter at runtime is not supported.
    /// </summary>
    public bool KeepARCameraAlive
    {
        get { return mKeepARCameraAlive; }
        set
        {
            if (Application.isPlaying)
                return;

            mKeepARCameraAlive = value;
        }

    }

    /// <summary>
    /// If TrackableBehaviours should be kept alive on scene change
    /// Changing this parameter at runtime is not supported.
    /// </summary>
    public bool KeepTrackableBehavioursAlive
    {
        get { return mKeepTrackableBehavioursAlive; }
        set
        {
            if (Application.isPlaying)
                return;

            mKeepTrackableBehavioursAlive = value;
        }

    }

    /// <summary>
    /// If the Text Reco Prefab should be kept alive on scene change
    /// Changing this parameter at runtime is not supported.
    /// </summary>
    public bool KeepTextRecoBehaviourAlive
    {
        get { return mKeepTextRecoBehaviourAlive; }
        set
        {
            if (Application.isPlaying)
                return;

            mKeepTextRecoBehaviourAlive = value;
        }
    }

    /// <summary>
    /// If the Text Reco Prefab should be kept alive on scene change
    /// Changing this parameter at runtime is not supported.
    /// </summary>
    public bool KeepUDTBuildingBehaviourAlive
    {
        get { return mKeepUDTBuildingBehaviourAlive; }
        set
        {
            if (Application.isPlaying)
                return;

            mKeepUDTBuildingBehaviourAlive = value;
        }
    }

    /// <summary>
    /// If the Text Reco Prefab should be kept alive on scene change
    /// Changing this parameter at runtime is not supported.
    /// </summary>
    public bool KeepCloudRecoBehaviourAlive
    {
        get { return mKeepCloudRecoBehaviourAlive; }
        set
        {
            if (Application.isPlaying)
                return;

            mKeepCloudRecoBehaviourAlive = value;
        }
    }

    /// <summary>
    /// Provides singleton-like access to this MonoBehaviour 
    /// </summary>
    public static KeepAliveBehaviour Instance
    {
        get
        {
            if (sKeepAliveBehaviour == null)
                sKeepAliveBehaviour = (KeepAliveBehaviour)FindObjectOfType(typeof (KeepAliveBehaviour));

            return sKeepAliveBehaviour;
        }
    }

    #endregion



    #region PUBLIC_METHODS

    /// <summary>
    /// Registers an event handler with this CloudRecoBehaviour which will be called on events
    /// </summary>
    public void RegisterEventHandler(ILoadLevelEventHandler eventHandler)
    {
        mHandlers.Add(eventHandler);
    }


    /// <summary>
    /// Unregisters an event handler
    /// </summary>
    public bool UnregisterEventHandler(ILoadLevelEventHandler eventHandler)
    {
        return mHandlers.Remove(eventHandler);
    }

    #endregion // PUBLIC_METHODS



    #region UNITY_MONOBEHAVIOUR_METHODS

    /// <summary>
    /// This method will associate active datasets with new trackable behaviours when a new level is loaded
    /// </summary>
    void OnLevelWasLoaded()
    {
        if (mKeepARCameraAlive)
        {
            var stateManager = (StateManagerImpl)TrackerManager.Instance.GetStateManager();

            List<TrackableBehaviour> trackablesKeptAlive;
            if (mKeepTrackableBehavioursAlive)
            {
                // get all trackable behaviours that were in use in the last scene
                trackablesKeptAlive = stateManager.GetTrackableBehaviours().ToList();
                foreach (var wb in stateManager.GetWordManager().GetTrackableBehaviours())
                    trackablesKeptAlive.Add(wb);
            }
            else
            {
                trackablesKeptAlive = new List<TrackableBehaviour>();
            }

            // notify handlers of all kept alive and disabled objects
            foreach (var handler in mHandlers)
                handler.OnLevelLoaded(trackablesKeptAlive);

            // get all currently known trackables in the scene
            TrackableBehaviour[] trackableBehavioursBeforeReinit = (TrackableBehaviour[])FindObjectsOfType(typeof(TrackableBehaviour));

            // state manager might contain trackables that are not kept alive
            stateManager.RemoveDestroyedTrackables();

            stateManager.AssociateMarkerBehaviours();

            var imageTracker = (ImageTracker)TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
            if (imageTracker != null)
            {
                var dataSets = imageTracker.GetDataSets();
                var activeDataSets = imageTracker.GetActiveDataSets().ToList();

                foreach (var dataSet in dataSets)
                {
                    // deactivate datasets fist to allow the registration of new virtual buttons
                    if (activeDataSets.Contains(dataSet))
                        imageTracker.DeactivateDataSet(dataSet);

                    // associate TrackableBehaviours with the trackables in this dataset
                    stateManager.AssociateTrackableBehavioursForDataSet(dataSet);

                    // activate datasets again
                    if (activeDataSets.Contains(dataSet))
                        imageTracker.ActivateDataSet(dataSet);
                }
            }

            // only re-initialize word behaviours if there is a text reco instance
            bool doNotDisableWords = false;
            var textReco = (TextRecoBehaviour)FindObjectOfType(typeof(TextRecoBehaviour));
            if (textReco != null)
            {
                if (!textReco.IsInitialized)
                    doNotDisableWords = true; // the text reco prefab is there, but has not been initialized yet --> do not disable any word instances at this point
                else
                {
                    var wordManager = (WordManagerImpl)stateManager.GetWordManager();
                    wordManager.RemoveDestroyedTrackables();
                    wordManager.InitializeWordBehaviourTemplates();
                }
            }

            // now notify event handlers of all trackable behaviours that were deactivated
            List<TrackableBehaviour> deactivatedTrackableBehaviours = new List<TrackableBehaviour>();

            // go through the TrackableBehaviour and deactivate those that are not used in the statemanager or word manager
            IEnumerable<TrackableBehaviour> trackableBehaviours = stateManager.GetTrackableBehaviours();
            IEnumerable<WordBehaviour> wordBehaviours = stateManager.GetWordManager().GetTrackableBehaviours();

            foreach (TrackableBehaviour previousTB in trackableBehavioursBeforeReinit)
            {
                if (previousTB is WordBehaviour)
                {
                    if (!doNotDisableWords)
                    {
                        // check the word manger
                        if (!wordBehaviours.Contains(previousTB as WordBehaviour))
                        {
                            previousTB.gameObject.SetActiveRecursively(false);
                            deactivatedTrackableBehaviours.Add(previousTB);
                        }
                    }
                }
                else
                {
                    // skip udt and cloud reco image targets - they are not registered anywhere and are used by app logic
                    if (previousTB is ImageTargetBehaviour && ((IEditorImageTargetBehaviour)previousTB).ImageTargetType != ImageTargetType.PREDEFINED)
                        continue;

                    // check the state manager
                    if (!trackableBehaviours.Contains(previousTB))
                    {
                        previousTB.gameObject.SetActiveRecursively(false);
                        deactivatedTrackableBehaviours.Add(previousTB);
                    }
                }
            }

            foreach (var handler in mHandlers)
                handler.OnDuplicateTrackablesDisabled(deactivatedTrackableBehaviours);

        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS
}