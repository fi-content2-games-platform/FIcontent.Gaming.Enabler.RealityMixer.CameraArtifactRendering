/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WordImpl : TrackableImpl, Word
{
    #region PRIVATE_MEMBER_VARIABLES

    private string mText;
    private Vector2 mSize;
    private Image mLetterMask;
    private QCARManagerImpl.ImageHeaderData mLetterImageHeader;
    private RectangleData[] mLetterBoundingBoxes;

    #endregion // PRIVATE_MEMBER_VARIABLES
    


    #region CONSTRUCTION

    public WordImpl(int id, string text, Vector2 size)
        : base(text, id)
    {
        Type = TrackableType.WORD;

        mText = text;
        mSize = size;
    }

    #endregion // CONSTRUCTION



    #region PROPERTIES

    /// <summary>
    /// Get the string value of the tracked word
    /// </summary>
    public string StringValue
    {
        get { return mText; }
    }


    /// <summary>
    /// Returns the size of the tracked word in 3D scene units
    /// </summary>
    public Vector2 Size
    {
        get { return mSize; }
    }

    #endregion // PROPERTIES



    #region PUBLIC_METHODS

    /// <summary>
    /// Returns an image representing the bit mask of the letters in the word.
    /// Each pixel in the image is represented by a byte (8-bit value).
    /// A value of 255 represents an empty area, i.e. a pixel not covered 
    /// by any letter of the word.
    /// If a pixel is covered by a letter, then the pixel value represents 
    /// the position of that letter in the word, i.e. 0 for the first character,
    /// 1 for the second, 2 for the third, and so on.
    /// </summary>
    public Image GetLetterMask()
    {
        if (!QCARRuntimeUtilities.IsQCAREnabled())
            return null;

        if (mLetterMask == null)
        {
            CreateLetterMask();
        }
        return mLetterMask;
    }

    /// <summary>
    /// Returns the axis-aligned bounding boxes for all letters of the word. These are defined in the range of [0, 1] which corresponds to the whole bounding box of the word.
    /// </summary>
    public RectangleData[] GetLetterBoundingBoxes()
    {
        if (!QCARRuntimeUtilities.IsQCAREnabled())
            return new RectangleData[0];

        if (mLetterBoundingBoxes == null)
        {
            var length = mText.Length;
            mLetterBoundingBoxes = new RectangleData[length];

            var rectPtr = Marshal.AllocHGlobal(length * Marshal.SizeOf(
                                    typeof(RectangleData)));
            QCARWrapper.Instance.WordGetLetterBoundingBoxes(ID, rectPtr);

            var c = new IntPtr(rectPtr.ToInt32());
            for (var i = 0; i < length; i++)
            {
                mLetterBoundingBoxes[i] = (RectangleData)Marshal.PtrToStructure(c, typeof(RectangleData));
                c = new IntPtr(c.ToInt32() + Marshal.SizeOf(
                    typeof(RectangleData)));
            }
            Marshal.FreeHGlobal(rectPtr);
        }
        return mLetterBoundingBoxes;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void InitImageHeader()
    {
        mLetterImageHeader = new QCARManagerImpl.ImageHeaderData();
        mLetterImageHeader.width = mLetterImageHeader.bufferWidth = (int)(Size.x + 1);
        mLetterImageHeader.height = mLetterImageHeader.bufferHeight = (int) (Size.y + 1);
        mLetterImageHeader.format = (int)Image.PIXEL_FORMAT.GRAYSCALE;

        mLetterMask = new ImageImpl();
    }

    private void CreateLetterMask()
    {
        
        InitImageHeader();
        var image = (ImageImpl)mLetterMask;
        SetImageValues(mLetterImageHeader, image);
        AllocateImage(image);
        mLetterImageHeader.data = image.UnmanagedData;

        var imageHeaderPtr = Marshal.AllocHGlobal(Marshal.SizeOf(
            typeof(QCARManagerImpl.ImageHeaderData)));
        Marshal.StructureToPtr(mLetterImageHeader, imageHeaderPtr, false);
        QCARWrapper.Instance.WordGetLetterMask(ID, imageHeaderPtr);
        mLetterImageHeader = (QCARManagerImpl.ImageHeaderData)Marshal.PtrToStructure(imageHeaderPtr, typeof(QCARManagerImpl.ImageHeaderData));

        if (mLetterImageHeader.reallocate == 1)
        {
            Debug.LogWarning("image wasn't allocated correctly");
            return;
        }

        // Copy data:
        image.CopyPixelsFromUnmanagedBuffer();
        mLetterMask = image;

        Marshal.FreeHGlobal(imageHeaderPtr);
    }

    private static void SetImageValues(QCARManagerImpl.ImageHeaderData imageHeader, ImageImpl image)
    {
        image.Width = imageHeader.width;
        image.Height = imageHeader.height;
        image.Stride = imageHeader.stride;
        image.BufferWidth = imageHeader.bufferWidth;
        image.BufferHeight = imageHeader.bufferHeight;
        image.PixelFormat = (Image.PIXEL_FORMAT)imageHeader.format;
    }

    private static void AllocateImage(ImageImpl image)
    {
        image.Pixels = new byte[QCARWrapper.Instance.QcarGetBufferSize(image.BufferWidth,
                                                               image.BufferHeight,
                                                               (int)image.PixelFormat)];

        Marshal.FreeHGlobal(image.UnmanagedData);

        image.UnmanagedData = Marshal.AllocHGlobal(QCARWrapper.Instance.QcarGetBufferSize(image.BufferWidth,
                                                                                          image.BufferHeight,
                                                                                          (int)image.PixelFormat));
    }

    #endregion // PRIVATE_METHODS
}