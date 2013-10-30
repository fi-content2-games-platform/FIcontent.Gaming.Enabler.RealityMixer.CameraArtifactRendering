/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

// An image - Used to expose the camera frame.
// 
// The image's pixel buffer can have a different size than the
// Wdith and Height properties report. This is e.g. the
// case when an image is used for rendering as a texture without
// non-power-of-two support.
// The real size of the image's pixel buffer can be queried using
// BufferWidth and BufferHeight. 
public class ImageImpl : Image
{
    #region PROPERTIES

    // The width of the image in pixels. Note the pixel buffer can be
    // wider than this.
    public override int Width
    {
        get { return mWidth; }
        set { mWidth = value; }
    }

    // The height of the image in pixels. Note that the pixel buffer
    // can be higher than this.
    public override int Height
    {
        get { return mHeight; }
        set { mHeight = value; }
    }

    // The number bytes from one row of pixels to the next row.
    public override int Stride
    {
        get { return mStride; }
        set { mStride = value; }
    }

    // The number of pixel columns that fit into the pixel buffer.
    public override int BufferWidth
    {
        get { return mBufferWidth; }
        set { mBufferWidth = value; }
    }

    // The number of rows that fit into the pixel buffer.
    public override int BufferHeight
    {
        get { return mBufferHeight; }
        set { mBufferHeight = value; }
    }

    // The pixel format of the image.
    public override PIXEL_FORMAT PixelFormat
    {
        get { return mPixelFormat; }
        set { mPixelFormat = value; }
    }

    // The pixel data.
    public override byte[] Pixels
    {
        get { return mData; }
        set { mData = value; }
    }

    // The unmanaged memory buffer used for marshaling.
    public IntPtr UnmanagedData
    {
        get { return mUnmanagedData; }
        set { mUnmanagedData = value; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    // Header:
    private int mWidth;
    private int mHeight;
    private int mStride;
    private int mBufferWidth;
    private int mBufferHeight;
    private PIXEL_FORMAT mPixelFormat;

    // Data:
    private byte[] mData;
    private IntPtr mUnmanagedData;
    private bool mDataSet;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region CONSTRUCTION

    public ImageImpl()
    {
        mWidth = 0;
        mHeight = 0;
        mStride = 0;
        mBufferWidth = 0;
        mBufferHeight = 0;
        mPixelFormat = PIXEL_FORMAT.UNKNOWN_FORMAT;

        mData = null;
        mUnmanagedData = IntPtr.Zero;
        mDataSet = false;
    }


    ~ImageImpl()
    {
        Marshal.FreeHGlobal(mUnmanagedData);
        mUnmanagedData = IntPtr.Zero;
    }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // Returns true if this image has been fully initiailzed with valid data.
    // False otherwise.
    public override bool IsValid()
    {
        return (mWidth > 0) && (mHeight > 0) && (mStride > 0) &&
            (mBufferWidth > 0) && (mBufferHeight > 0) && (mData != null) &&
            mDataSet; 
    }


    // Called by the QCARBehaviour when the unmanaged data buffer has been
    // filled:
    public void CopyPixelsFromUnmanagedBuffer()
    {
        if (mData == null || mUnmanagedData == IntPtr.Zero)
        {
            Debug.LogError("Image: Cannot copy image image data.");
            return;
        }

        int length;
        switch (mPixelFormat)
        {
            case PIXEL_FORMAT.RGBA8888:
                length = mBufferWidth*mBufferHeight*4;
                break;

            case PIXEL_FORMAT.RGB888:
                length = mBufferWidth*mBufferHeight*3;
                break;

            case PIXEL_FORMAT.RGB565:
                length = mBufferWidth*mBufferHeight*2;
                break;

            default:
                length = mBufferWidth*mBufferHeight;
                break;
        }

        Marshal.Copy(mUnmanagedData, mData, 0, length);
        mDataSet = true;
    }

    public override void CopyToTexture(Texture2D texture2D)
    {
        var format = ConvertPixelFormat(mPixelFormat);
        if (texture2D.width != mWidth || texture2D.height != mHeight || format != texture2D.format)
        {
            texture2D.Resize(mWidth, mHeight, format, false);
        }
        var channels = 1;
        switch (mPixelFormat)
        {
            case PIXEL_FORMAT.RGBA8888:
                channels = 4;
                break;
            case PIXEL_FORMAT.RGB565:
            case PIXEL_FORMAT.RGB888:
                channels = 3;
                break;
        }
        var colors = texture2D.GetPixels();
        var c = 0;
        for (var i = 0; i < colors.Length; i++)
        {
            for (var j = 0; j < channels; j++)
                colors[i][j] = mData[c++]/255.0f;
            for (var j = channels; j < 4; j++)
                colors[i][j] = colors[i][j - 1];
        }

        texture2D.SetPixels(colors);
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    private TextureFormat ConvertPixelFormat(PIXEL_FORMAT input)
    {
        switch (mPixelFormat)
        {
            case PIXEL_FORMAT.RGBA8888:
                return TextureFormat.RGBA32;
            case PIXEL_FORMAT.RGB888:
                return TextureFormat.RGB24;
            case PIXEL_FORMAT.RGB565:
                return TextureFormat.RGB565;
            default:
                return TextureFormat.Alpha8;
        }
    }
    #endregion // PRIVATE_METHODS
}
