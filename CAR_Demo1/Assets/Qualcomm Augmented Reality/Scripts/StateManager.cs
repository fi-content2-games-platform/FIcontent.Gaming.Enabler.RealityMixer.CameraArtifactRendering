/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to manage the state of all TrackableBehaviours, create them,
/// associate them with Trackables, update their pose, etc.
/// </summary>
public abstract class StateManager
{
    #region PUBLIC_METHODS

    /// <summary>
    /// Returns the TrackableBehaviours currently being tracked
    /// </summary>
    public abstract IEnumerable<TrackableBehaviour> GetActiveTrackableBehaviours();

    /// <summary>
    /// Returns all currently instantiated TrackableBehaviours
    /// </summary>
    public abstract IEnumerable<TrackableBehaviour> GetTrackableBehaviours();

    /// <summary>
    /// Destroys all the TrackableBehaviours for the given Trackable
    /// </summary>
    public abstract void DestroyTrackableBehavioursForTrackable(Trackable trackable, bool destroyGameObjects = true);

    /// <summary>
    /// Returns the word manager instance that can be used to access
    /// all currently tracked words from TextRecognition
    /// </summary>
    public abstract WordManager GetWordManager();

    #endregion // PUBLIC_METHODS
}