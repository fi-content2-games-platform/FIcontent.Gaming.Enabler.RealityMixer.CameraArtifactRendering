/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

/// <summary>
/// Interface for handling events regarding the video background
/// </summary>
public interface IVideoBackgroundEventHandler
{
    /// <summary>
    /// Called after the video background config has been changed
    /// </summary>
    void OnVideoBackgroundConfigChanged();
}
