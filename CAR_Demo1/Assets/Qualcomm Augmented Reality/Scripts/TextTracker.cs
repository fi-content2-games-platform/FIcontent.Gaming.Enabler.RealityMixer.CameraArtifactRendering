/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;

/// <summary>
/// This class handles Text Tracking and defining the detection and tracking area at runtime
/// </summary>
public abstract class TextTracker : Tracker
{
    #region PUBLIC_PROPERTIES

    /// <summary>
    /// Get the word list used for tracking text
    /// </summary>
    public abstract WordList WordList 
    {
        get;
    }


    #endregion // PUBLIC_PROPERTIES



    #region  PUBLIC_METHODS

    /// <summary>
    /// Defines the areas of the image in screen coordinates where text can be detected and tracked.
    /// </summary>
    public abstract bool SetRegionOfInterest(Rect detectionRegion, Rect trackingRegion);

    /// <summary>
    /// Returns the areas of the image in screen coordinates where text can be detected and tracked.
    /// </summary>
    public abstract bool GetRegionOfInterest(out Rect detectionRegion, out Rect trackingRegion);

    #endregion // PUBLIC_METHODS
}