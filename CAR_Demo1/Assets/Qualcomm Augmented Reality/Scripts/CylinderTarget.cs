/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

/// <summary>
/// A trackable that represents a cylinder or conical object
/// </summary>
public interface CylinderTarget : Trackable
{
    /// <summary>
    /// Returns the side length (or slanted height) of the cylinder target in 3D scene units
    /// </summary>
    float GetSideLength();

    /// <summary>
    /// Returns the top diameter of the cylinder target in 3D scene units
    /// </summary>
    float GetTopDiameter();

    /// <summary>
    /// Returns the bottom diameter of the cylinder target in 3D scene units
    /// </summary>
    float GetBottomDiameter();

    /// <summary>
    /// Define a new side length of the cylinder target.
    /// This will uniformly scale the cylinder and thus also update top and bottom diameter.
    /// This is only allowed when the dataset is not active!
    /// </summary>
    bool SetSideLength(float sideLength);

    /// <summary>
    /// Define a new top diameter of the cylinder target.
    /// This will uniformly scale the cylinder and thus also update side length and bottom diameter.
    /// This is only allowed when the dataset is not active!
    /// </summary>
    bool SetTopDiameter(float topDiameter);

    /// <summary>
    /// Define a new bottom diameter of the cylinder target.
    /// This will uniformly scale the cylinder and thus also update side length and top diameter.
    /// This is only allowed when the dataset is not active!
    /// </summary>
    bool SetBottomDiameter(float bottomDiameter);
}