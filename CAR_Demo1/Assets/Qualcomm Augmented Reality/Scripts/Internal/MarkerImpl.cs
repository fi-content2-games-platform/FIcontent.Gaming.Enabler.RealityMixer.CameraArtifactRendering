/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;

public class MarkerImpl : TrackableImpl, Marker
{
    private readonly float mSize;

    public MarkerImpl(string name, int id, float size, int markerID)
        : base(name, id)
    {
        Type = TrackableType.MARKER;
        mSize = size;
        MarkerID = markerID;
    }

    public float GetSize()
    {
        return mSize;
    }

    public int MarkerID 
    { get; private set; }
}