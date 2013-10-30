/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using UnityEngine;

/// <summary>
/// This class serves both as an augmentation definition for a CylinderTarget in the editor
/// as well as a tracked CylinderTarget result at runtime
/// </summary>
public class CylinderTargetBehaviour : DataSetTrackableBehaviour, IEditorCylinderTargetBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES

    private CylinderTarget mCylinderTarget;

    [SerializeField]
    [HideInInspector]
    private float mTopDiameterRatio;

    [SerializeField]
    [HideInInspector]
    private float mBottomDiameterRatio;


    //the following members are used for synchronizing Unity scale updates with native
    private int mFrameIndex = -1;
    private int mUpdateFrameIndex = -1;
    private float mFutureScale;
 
    #endregion // PRIVATE_MEMBER_VARIABLES


    #region PROPERTIES

    /// <summary>
    /// The CylinderTarget that this CylinderTargetBehaviour augments
    /// </summary>
    public CylinderTarget CylinderTarget
    {
        get { return mCylinderTarget; }
    }

    /// <summary>
    /// Get or set the sidelength (slanted height). Changing this value uniformly scales the target.
    /// </summary>
    public float SideLength
    {
        get { return GetScale(); }
    }

    /// <summary>
    /// Get or set the top diameter. Changing this value uniformly scales the target.
    /// </summary>
    public float TopDiameter
    {
        get { return mTopDiameterRatio * GetScale(); }
    }

    /// <summary>
    /// Get or set the bottom diameter. Changing this value uniformly scales the target.
    /// </summary>
    public float BottomDiameter
    {
        get { return mBottomDiameterRatio * GetScale(); }
    }
    
    #endregion // PROPERTIES

    #region PUBLIC_METHODS

    public bool SetSideLength(float value)
    {
        return SetScale(value);
    }

    public bool SetTopDiameter(float value)
    {
        //only apply the scale if top diameter is not set to zero (for a real cone)
        if (Math.Abs(mTopDiameterRatio) > 1e-5f)
            return SetScale(value/mTopDiameterRatio);
        return false;
    }

    public bool SetBottomDiameter(float value)
    {
        //only apply the scale if bottom diameter is not set to zero (for a real cone)
        if (Math.Abs(mBottomDiameterRatio) > 1e-5f)
            return SetScale(value / mBottomDiameterRatio);
        return false;
    }


    public override void OnFrameIndexUpdate(int newFrameIndex)
    {
        if (mUpdateFrameIndex >= 0 && mUpdateFrameIndex != newFrameIndex)
        {
            ApplyScale(mFutureScale);
            mUpdateFrameIndex = -1;
        }
        mFrameIndex = newFrameIndex;
    }
    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    /// <summary>
    /// Scales the Trackable uniformly
    /// </summary>
    protected override bool CorrectScaleImpl()
    {
        bool scaleChanged = false;

        for (int i = 0; i < 3; ++i)
        {
            // Force uniform scale:
            if (transform.localScale[i] != mPreviousScale[i])
            {
                transform.localScale =
                    new Vector3(transform.localScale[i],
                                transform.localScale[i],
                                transform.localScale[i]);

                mPreviousScale = transform.localScale;
                scaleChanged = true;
                break;
            }
        }

        return scaleChanged;
    }

    /// <summary>
    /// This method disconnects the TrackableBehaviour from it's associated trackable.
    /// Use it only if you know what you are doing - e.g. when you want to destroy a trackable, but reuse the TrackableBehaviour.
    /// </summary>
    protected override void InternalUnregisterTrackable()
    {
        mTrackable = mCylinderTarget = null;
    }

    #endregion // PROTECTED_METHODS

    #region PRIVATE_METHODS

    private float GetScale()
    {
        return transform.localScale.x; 
    }

    private bool SetScale(float value)
    {
        if (transform.localScale.x == value)
            return true;

        if (mCylinderTarget != null)
        {
            //during runtime: update for native
            if (!mCylinderTarget.SetSideLength(value))
                return false;

            //we defer the scaling in unity until to the next camera frame from native
            //otherwise Unity is always ahead of native and the size of the cylinder is flickering
            mUpdateFrameIndex = mFrameIndex;
            mFutureScale = value;
        }
        else
        {
            ApplyScale(value);
        }
        return true;
    }

    private void ApplyScale(float value)
    {
        transform.localScale = new Vector3(value, value, value);
    }

    #endregion


    #region EDITOR_INTERFACE_IMPLEMENTATION

    void IEditorCylinderTargetBehaviour.InitializeCylinderTarget(CylinderTarget cylinderTarget)
    {
        mTrackable = mCylinderTarget = cylinderTarget;

        //scale cylinder target according to values defined in Unity
        cylinderTarget.SetSideLength(this.SideLength);
    }

    void IEditorCylinderTargetBehaviour.SetAspectRatio(float topRatio, float bottomRatio)
    {
        mTopDiameterRatio = topRatio;
        mBottomDiameterRatio = bottomRatio;
    }


    #endregion // EDITOR_INTERFACE_IMPLEMENTATION
}
