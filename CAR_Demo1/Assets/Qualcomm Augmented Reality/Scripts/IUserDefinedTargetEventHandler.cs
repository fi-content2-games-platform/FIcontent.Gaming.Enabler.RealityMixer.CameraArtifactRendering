﻿/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

/// <summary>
/// An interface for handling User Defined Target building events.
/// </summary>
public interface IUserDefinedTargetEventHandler
{
    /// <summary>
    /// called when the UserDefinedTargetBehaviour has been initialized
    /// </summary>
    void OnInitialized(); 

    /// <summary>
    /// called when the UserDefinedTargetBehaviour reports a new frame Quality
    /// </summary>
    void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality);

    /// <summary>
    /// called when an error is reported during initialization
    /// </summary>
    void OnNewTrackableSource(TrackableSource trackableSource);
}
