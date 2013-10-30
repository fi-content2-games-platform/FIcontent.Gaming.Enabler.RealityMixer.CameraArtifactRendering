/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

/// <summary>
/// Interface for handling trackable state changes.
/// </summary>
public interface ITrackableEventHandler
{
    /// <summary>
    /// Called when the trackable state has changed.
    /// </summary>
    void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus);
}
