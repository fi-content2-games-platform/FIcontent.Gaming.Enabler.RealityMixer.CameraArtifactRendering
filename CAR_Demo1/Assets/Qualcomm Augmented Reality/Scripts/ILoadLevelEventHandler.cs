/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for handling trackable behaviours when a new level is loaded
/// </summary>
public interface ILoadLevelEventHandler
{
    /// <summary>
    /// Called when a new level is loaded and trackable behaviours are kept alive.
    /// </summary>
    /// <param name="keptAliveTrackables">All trackable behaviours that are kept alive when changing the scene</param>
    void OnLevelLoaded(IEnumerable<TrackableBehaviour> keptAliveTrackables);

    /// <summary>
    /// Called when a new level is loaded and trackable behaviours are kept alive.
    /// </summary>
    /// <param name="disabledTrackables">All trackable behaviours that were disabled because they were duplicates of kept alive trackables</param>
    void OnDuplicateTrackablesDisabled(IEnumerable<TrackableBehaviour> disabledTrackables);
}

