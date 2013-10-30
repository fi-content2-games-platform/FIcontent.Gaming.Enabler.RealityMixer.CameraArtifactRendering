/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;


/// <summary>
/// A WordResult provides the current state of a Word.
/// It contains all dynamic information that changes frame-to-frame, 
/// i.e. which is based on the current pose of the word
/// </summary>
public abstract class WordResult
{
    #region PROPERTIES

    /// <summary>
    /// Get the corresponding Word-object of this trackable
    /// </summary>
    public abstract Word Word { get; }

    /// <summary>
    /// Get the oriented 2D bounding box of the word in image space
    /// </summary>
    public abstract OrientedBoundingBox Obb { get; }

    /// <summary>
    /// Get the position of the current pose of the trackable
    /// </summary>
    public abstract Vector3 Position { get; }

    /// <summary>
    /// Get the rotation of the current pose of the trackable
    /// </summary>
    public abstract Quaternion Orientation { get; }

    /// <summary>
    /// Get the current status of the trackable
    /// </summary>
    public abstract TrackableBehaviour.Status CurrentStatus { get; }

    #endregion // PROPERTIES
}

