/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextTrackerImpl : TextTracker
{
    #region NESTED

    /// <summary>
    /// Defines the up direction for tracking text in relation to landscape left.
    /// </summary>
    private enum UpDirection
    {
        TEXTTRACKER_UP_IS_0_HRS = 1,
        TEXTTRACKER_UP_IS_3_HRS = 2,
        TEXTTRACKER_UP_IS_6_HRS = 3,
        TEXTTRACKER_UP_IS_9_HRS = 4
    }; 

    #endregion // NESTED



    #region PRIVATE_MEMBER_VARIABLES

    private readonly WordList mWordList = new WordListImpl();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_PROPERTIES

    /// <summary>
    /// Get the word list used for tracking text
    /// </summary>
    public override WordList WordList
    {
        get { return mWordList; }
    }

    #endregion



    #region PUBLIC_METHODS

    // Starts the tracker.
    public override bool Start()
    {
        if (QCARWrapper.Instance.TextTrackerStart() == 0)
        {
            Debug.LogError("Could not start tracker.");
            return false;
        }

        return true;
    }


    // Stops the tracker.
    public override void Stop()
    {
        QCARWrapper.Instance.TextTrackerStop();

        var wordManager = (WordManagerImpl)TrackerManager.Instance.GetStateManager().GetWordManager();

        // Mark all word as not found:
        wordManager.SetWordBehavioursToNotFound();
    }


    /// <summary>
    /// Defines the areas of the image in screen coordinates where text can be detected and tracked.
    /// </summary>
    public override bool SetRegionOfInterest(Rect detectionRegion, Rect trackingRegion)
    {
        QCARBehaviour qcarbehaviour = (QCARBehaviour)Object.FindObjectOfType(typeof(QCARBehaviour));
        if (qcarbehaviour == null)
        {
            Debug.LogError("QCAR Behaviour could not be found");
            return false;
        }

        // required information to transform screen space coordinates into camera frame coordinates:
        Rect bgTextureViewPortRect = qcarbehaviour.GetViewportRectangle();
        bool isMirrored = qcarbehaviour.VideoBackGroundMirrored;
        CameraDevice.VideoModeData videoModeData = CameraDevice.Instance.GetVideoMode(qcarbehaviour.CameraDeviceMode);

        // depending on the current orientation, different corner points of the rect have to be taken
        // - they need to span a rectangle in the camera frame coordinate system
        Vector2 detectionLeftTop, detectionRightBottom, trackingLeftTop, trackingRightBottom;
        QCARRuntimeUtilities.SelectRectTopLeftAndBottomRightForLandscapeLeft(detectionRegion, isMirrored, out detectionLeftTop, out detectionRightBottom);
        QCARRuntimeUtilities.SelectRectTopLeftAndBottomRightForLandscapeLeft(trackingRegion, isMirrored, out trackingLeftTop, out trackingRightBottom);

        // transform the coordinates into camera frame coord system
        QCARRenderer.Vec2I camFrameDetectionLeftTop = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(detectionLeftTop, bgTextureViewPortRect, isMirrored, videoModeData);
        QCARRenderer.Vec2I camFrameDetectionRightBottom = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(detectionRightBottom, bgTextureViewPortRect, isMirrored, videoModeData);
        QCARRenderer.Vec2I camFrameTrackingLeftTop = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(trackingLeftTop, bgTextureViewPortRect, isMirrored, videoModeData);
        QCARRenderer.Vec2I camFrameTrackingRightBottom = QCARRuntimeUtilities.ScreenSpaceToCameraFrameCoordinates(trackingRightBottom, bgTextureViewPortRect, isMirrored, videoModeData);

        if (QCARWrapper.Instance.TextTrackerSetRegionOfInterest(camFrameDetectionLeftTop.x, camFrameDetectionLeftTop.y, camFrameDetectionRightBottom.x, camFrameDetectionRightBottom.y,
                                                                camFrameTrackingLeftTop.x, camFrameTrackingLeftTop.y, camFrameTrackingRightBottom.x, camFrameTrackingRightBottom.y, (int)CurrentUpDirection) == 0)
        {
            Debug.LogError(string.Format("Could not set region of interest: ({0}, {1}, {2}, {3}) - ({4}, {5}, {6}, {7})", 
                                         detectionRegion.x, detectionRegion.y, detectionRegion.width, detectionRegion.height,
                                         trackingRegion.x, trackingRegion.y, trackingRegion.width, trackingRegion.height));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns the areas of the image in screen coordinates where text can be detected and tracked.
    /// </summary>
    public override bool GetRegionOfInterest(out Rect detectionRegion, out Rect trackingRegion)
    {
        QCARBehaviour qcarbehaviour = (QCARBehaviour)Object.FindObjectOfType(typeof(QCARBehaviour));
        if (qcarbehaviour == null)
        {
            Debug.LogError("QCAR Behaviour could not be found");
            detectionRegion = new Rect();
            trackingRegion = new Rect();
            return false;
        }

        // required information to transform camera frame to screen space coordinates:
        Rect bgTextureViewPortRect = qcarbehaviour.GetViewportRectangle();
        bool isMirrored = qcarbehaviour.VideoBackGroundMirrored;
        CameraDevice.VideoModeData videoModeData = CameraDevice.Instance.GetVideoMode(qcarbehaviour.CameraDeviceMode);

        IntPtr detectionROIptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RectangleIntData)));
        IntPtr trackingROIptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RectangleIntData)));
        
        // get current region of interest from native
        QCARWrapper.Instance.TextTrackerGetRegionOfInterest(detectionROIptr, trackingROIptr);

        RectangleIntData detectionROIcamSpace = (RectangleIntData)Marshal.PtrToStructure(detectionROIptr, typeof(RectangleIntData));
        RectangleIntData trackingROIcamSpace = (RectangleIntData)Marshal.PtrToStructure(trackingROIptr, typeof(RectangleIntData));
        Marshal.FreeHGlobal(detectionROIptr);
        Marshal.FreeHGlobal(trackingROIptr);

        // calculate screen space rect for detection and tracking regions:
        detectionRegion = ScreenSpaceRectFromCamSpaceRectData(detectionROIcamSpace, bgTextureViewPortRect, isMirrored, videoModeData);
        trackingRegion = ScreenSpaceRectFromCamSpaceRectData(trackingROIcamSpace, bgTextureViewPortRect, isMirrored, videoModeData);

        return true;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private Rect ScreenSpaceRectFromCamSpaceRectData(RectangleIntData camSpaceRectData, Rect bgTextureViewPortRect,
                                                     bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        Vector2 topLeftSSLandscape = QCARRuntimeUtilities.CameraFrameToScreenSpaceCoordinates(new Vector2(camSpaceRectData.leftTopX, camSpaceRectData.leftTopY),
                                                                                              bgTextureViewPortRect, isTextureMirrored, videoModeData);
        Vector2 bottomRightSSLandscape = QCARRuntimeUtilities.CameraFrameToScreenSpaceCoordinates(new Vector2(camSpaceRectData.rightBottomX, camSpaceRectData.rightBottomY),
                                                                                              bgTextureViewPortRect, isTextureMirrored, videoModeData);
        
        return QCARRuntimeUtilities.CalculateRectFromLandscapeLeftCorners(topLeftSSLandscape, bottomRightSSLandscape, isTextureMirrored);
    }

    #endregion // PRIVATE_METHODS



    #region PRIVATE_PROPERTIES

    private UpDirection CurrentUpDirection
    {
        get
        {
            UpDirection upDirection = UpDirection.TEXTTRACKER_UP_IS_0_HRS;
            // mapping from screen orientation to up direction defined in native API (TextTracker.UP_DIRECTION) 
            switch (QCARRuntimeUtilities.ScreenOrientation)
            {
                case ScreenOrientation.Portrait:
                    upDirection = UpDirection.TEXTTRACKER_UP_IS_9_HRS;
                    break;

                case ScreenOrientation.PortraitUpsideDown:
                    upDirection = UpDirection.TEXTTRACKER_UP_IS_3_HRS;
                    break;

                case ScreenOrientation.LandscapeRight:
                    upDirection = UpDirection.TEXTTRACKER_UP_IS_6_HRS;
                    break;

                default:
                    upDirection = UpDirection.TEXTTRACKER_UP_IS_0_HRS;
                    break;
            }

            return upDirection;
        }
    }


    #endregion // PRIVATE_PROPERTIES
}