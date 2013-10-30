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
public class WordResultImpl : WordResult
{
    #region PRIVATE_MEMBER_VARIABLES

    private OrientedBoundingBox mObb;
    private Vector3 mPosition;
    private Quaternion mOrientation;
    private readonly Word mWord;
    private TrackableBehaviour.Status mStatus = TrackableBehaviour.Status.UNKNOWN;

    #endregion //PRIVATE_MEMBER_VARIABLES



    #region CONSTRUCTION

    public WordResultImpl(Word word)
    {
        mWord = word;
    }

    #endregion // CONSTRUCTION



    #region PROPERTIES

    /// <summary>
    /// Get the corresponding Word-object of this trackable
    /// </summary>
    public override Word Word
    {
        get { return mWord; }
    }

    /// <summary>
    /// Get the position of the current pose of the trackable
    /// </summary>
    public override Vector3 Position
    {
        get { return mPosition; }
    }

    /// <summary>
    /// Get the rotation of the current pose of the trackable
    /// </summary>
    public override Quaternion Orientation
    {
        get { return mOrientation; }
    }

    /// <summary>
    /// Get the oriented 2D bounding box of the word in image space
    /// </summary>
    public override OrientedBoundingBox Obb
    {
        get { return mObb; }
    }

    /// <summary>
    /// Get the current status of the trackable
    /// </summary>
    public override TrackableBehaviour.Status CurrentStatus
    {
        get { return mStatus; }
    }

    #endregion // PROPERTIES



    #region PUBLIC_METHODS

    public void SetPose(Vector3 position, Quaternion orientation)
    {
        mPosition = position;
        mOrientation = orientation;
    }

    public void SetObb(OrientedBoundingBox obb)
    {
        mObb = obb;
    }

    public void SetStatus(TrackableBehaviour.Status status)
    {
        mStatus = status;
    }

    #endregion
}

