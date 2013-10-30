/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;


/// <summary>
/// A word represents a single element of writing that has been detected.
/// It only contains static information about the word which does not change per frame
/// </summary>
public interface Word : Trackable
{
    /// <summary>
    /// Get the string value of the tracked word
    /// </summary>
    string StringValue { get; }

    /// <summary>
    /// Returns the size of the tracked word in 3D scene units
    /// </summary>
    Vector2 Size { get; }

    /// <summary>
    /// Returns an image representing the bit mask of the letters in the word.
    /// Each pixel in the image is represented by a byte (8-bit value).
    /// A value of 255 represents an empty area, i.e. a pixel not covered 
    /// by any letter of the word.
    /// If a pixel is covered by a letter, then the pixel value represents 
    /// the position of that letter in the word, i.e. 0 for the first character,
    /// 1 for the second, 2 for the third, and so on.
    /// </summary>
    Image GetLetterMask();

    /// <summary>
    /// Returns the axis-aligned bounding boxes for all letters of the word. These are defined in the range of [0, 1] which corresponds to the whole bounding box of the word.
    /// </summary>
    RectangleData[] GetLetterBoundingBoxes();
}


