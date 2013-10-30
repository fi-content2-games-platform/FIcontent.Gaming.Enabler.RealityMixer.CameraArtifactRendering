/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/


using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CylinderTargetImpl : TrackableImpl, CylinderTarget
{
    #region PRIVATE_MEMBER_VARIABLES

    private float mSideLength;
    private float mTopDiameter;
    private float mBottomDiameter;
    private readonly DataSetImpl mDataSet;

    #endregion

    #region CONSTRUCTION

    public CylinderTargetImpl(string name, int id, DataSet dataSet)
        : base(name, id)
    {
        Type = TrackableType.CYLINDER_TARGET;
        mDataSet = (DataSetImpl)dataSet;

        // read size from native:
        var dimensions = new float[3];

        IntPtr sizePtr = Marshal.AllocHGlobal(3 * Marshal.SizeOf(typeof(float)));
        QCARWrapper.Instance.CylinderTargetGetSize(mDataSet.DataSetPtr, Name, sizePtr);
        Marshal.Copy(sizePtr, dimensions, 0, 3);
        Marshal.FreeHGlobal(sizePtr);

        mSideLength = dimensions[0];
        mTopDiameter = dimensions[1];
        mBottomDiameter = dimensions[2];

    }

    #endregion // CONSTRUCTION

    #region PUBLIC_METHODS

    public float GetSideLength()
    {
        return mSideLength;
    }

    public float GetTopDiameter()
    {
        return mTopDiameter;
    }

    public float GetBottomDiameter()
    {
        return mBottomDiameter;
    }

    public bool SetSideLength(float sideLength)
    {
        ScaleCylinder(sideLength/mSideLength);

        // set size in native:
        return QCARWrapper.Instance.CylinderTargetSetSideLength(mDataSet.DataSetPtr, Name, sideLength) == 1;
    }


    public bool SetTopDiameter(float topDiameter)
    {
        ScaleCylinder(topDiameter / mTopDiameter);

        // set size in native:
        return QCARWrapper.Instance.CylinderTargetSetTopDiameter(mDataSet.DataSetPtr, Name, topDiameter) == 1;
    }


    public bool SetBottomDiameter(float bottomDiameter)
    {
        ScaleCylinder(bottomDiameter / mBottomDiameter);

        // set size in native:
        return QCARWrapper.Instance.CylinderTargetSetBottomDiameter(mDataSet.DataSetPtr, Name, bottomDiameter) == 1;
    }

    #endregion


    #region PRIVATE_METHODS

    private void ScaleCylinder(float scale)
    {
        mSideLength *= scale;
        mTopDiameter *= scale;
        mBottomDiameter *= scale;
    }

    #endregion // PRIVATE_METHODS
}