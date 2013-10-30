/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// This struct defines the 2D coordinates of a rectangle. 
/// The struct is internally used for setting Virtual Buttons or getting bounding boxes of letters.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RectangleData
{
    public float leftTopX;
    public float leftTopY;
    public float rightBottomX;
    public float rightBottomY;
}


/// <summary>
/// This struct defines the 2D coordinates of a rectangle using integers. 
/// The struct is internally used for querying the region of interest for text tracking.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RectangleIntData
{
    public int leftTopX;
    public int leftTopY;
    public int rightBottomX;
    public int rightBottomY;
}

/// <summary>
/// This struct defines an oriented rectangle.
/// It is internally used for defining the oriented bounding box of tword trackables
/// </summary>
public struct OrientedBoundingBox
{
    public OrientedBoundingBox(Vector2 center, Vector2 halfExtents, float rotation)
        : this()
    {
        Center = center;
        HalfExtents = halfExtents;
        Rotation = rotation;
    }

    /// <summary>
    /// Get the center of the box
    /// </summary>
    public Vector2 Center { get; private set; }

    /// <summary>
    /// Get half width and height of the box
    /// </summary>
    public Vector2 HalfExtents { get; private set; }

    /// <summary>
    /// Get the counter clock wise rotation of the box in degrees
    /// with respect to the x axis
    /// </summary>
    public float Rotation { get; private set; }
}