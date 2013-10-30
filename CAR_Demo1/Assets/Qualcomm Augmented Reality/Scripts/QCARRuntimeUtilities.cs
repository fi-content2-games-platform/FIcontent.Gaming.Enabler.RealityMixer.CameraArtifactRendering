/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;
#if UNITY_EDITOR
        using UnityEditor;
#endif
using Object = UnityEngine.Object;

/// <summary>
/// A utility class containing various helper methods
/// </summary>
public class QCARRuntimeUtilities
{
    #region NESTED

    private enum WebCamUsed
    {
        UNKNOWN,
        TRUE,
        FALSE
    }

    #endregion // NESTED



    #region PRIVATE_STATIC_VARIABLES
    
#if UNITY_EDITOR
    private static WebCamUsed sWebCamUsed = WebCamUsed.UNKNOWN;
#endif
    private static ScreenOrientation sScreenOrientation = ScreenOrientation.Unknown;

    #endregion // PRIVATE_STATIC_VARIABLES



    #region PUBLIC_METHODS

    /// <summary>
    /// Returns the file name without the path.
    /// </summary>
    public static string StripFileNameFromPath(string fullPath)
    {
        string[] pathParts = fullPath.Split(new char[] { '/' });
        string fileName = pathParts[pathParts.Length - 1];

        return fileName;
    }


    /// <summary>
    /// Returns the extension without the path.
    /// </summary>
    public static string StripExtensionFromPath(string fullPath)
    {
        string[] pathParts = fullPath.Split(new char[] { '.' });

        // Return empty string if there is no extension.
        if (pathParts.Length <= 1)
        {
            return "";
        }

        string extension = pathParts[pathParts.Length - 1];

        return extension;
    }

    /// <summary>
    /// Wrapper for Screen.orientation to enable correct handling in Editor
    /// and on devices that do not support all orientations.
    /// </summary>
    public static ScreenOrientation ScreenOrientation
    {
        get
        {
            if (QCARRuntimeUtilities.IsPlayMode())
                // if this has not been overwritten to a specific value, return landscape left as default in play mode
                return sScreenOrientation == ScreenOrientation.Unknown ? ScreenOrientation.LandscapeLeft : sScreenOrientation;
            else
            {
                // if this is queried before it has been initialized, ask native:
                if (sScreenOrientation == ScreenOrientation.Unknown)
                    sScreenOrientation = (ScreenOrientation)QCARWrapper.Instance.GetSurfaceOrientation();

                return sScreenOrientation;
            }
        }
    }

    /// <summary>
    /// This method is called from QCARBehaviour whenever the surface orientation changes in native.
    /// The last orientation is remembered here so that it is not queried all the time.
    /// </summary>
    public static void CacheSurfaceOrientation(ScreenOrientation surfaceOrientation)
    {
        sScreenOrientation = surfaceOrientation;
    }

    /// <summary>
    /// returns true of ScreenOrientation is in ANY landscape mode
    /// </summary>
    public static bool IsLandscapeOrientation
    {
        get
        {
            ScreenOrientation screenOrientation = ScreenOrientation;
            return (screenOrientation == ScreenOrientation.Landscape) ||
                   (screenOrientation == ScreenOrientation.LandscapeLeft) ||
                   (screenOrientation == ScreenOrientation.LandscapeRight);
        }
    }

    /// <summary>
    /// returns true if ScreenOrientation is in ANY portrait mode
    /// </summary>
    public static bool IsPortraitOrientation
    {
        get { return !IsLandscapeOrientation; }
    }

    /// <summary>
    /// Disables all TrackableBehaviours. Used when an GL or orientation error is detected in a sample.
    /// </summary>
    public static void ForceDisableTrackables()
    {
        TrackableBehaviour[] tbs = (TrackableBehaviour[])Object.FindObjectsOfType(typeof(TrackableBehaviour));
        if (tbs != null)
        {
            for (int i = 0; i < tbs.Length; ++i)
            {
                tbs[i].enabled = false;
            }
        }
    }


    /// <summary>
    /// returns ONLY true if we are running in Play Mode
    /// </summary>
    public static bool IsPlayMode()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// returns true if we have access to QCAR (on a mobile device OR in the emulator in Unity Pro with a webcam connected)
    /// </summary>
    public static bool IsQCAREnabled()
    {
#if UNITY_EDITOR
        if (sWebCamUsed == WebCamUsed.UNKNOWN)
        {
            // query the webcam if it should be used
            WebCamBehaviour webcam = (WebCamBehaviour)Object.FindObjectOfType(typeof(WebCamBehaviour));
            sWebCamUsed = webcam.IsWebCamUsed() ? WebCamUsed.TRUE : WebCamUsed.FALSE;
        }

        return sWebCamUsed == WebCamUsed.TRUE;
#else
        return true;
#endif
    }

    /// <summary>
    /// This method forces Play Mode to restart.
    /// It is called when Unity re-compiles the scripts shortly after starting play mode.
    /// </summary>
    public static void RestartPlayMode()
    {
#if UNITY_EDITOR
        // register for the next update call to start play mode again
        EditorApplication.update += CheckToStartPlayMode;
        // stop play mode - will not be executed immediately, but after scripts have been executed.
        EditorApplication.isPlaying = false;
#endif
    }


#if UNITY_EDITOR
    /// <summary>
    /// This restarts Play Mode and unregisters from the editor callback.
    /// </summary>
    private static void CheckToStartPlayMode()
    {
        // if play mode has stopped
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.update -= CheckToStartPlayMode;
            EditorApplication.isPlaying = true;
            Debug.LogWarning("Restarted Play Mode because scripts have been recompiled.");
        }
    }
#endif

    /// <summary>
    /// Calculates a position in camera frame coordinates based on the current orientation and background config for a given screen-space position
    /// </summary>
    public static QCARRenderer.Vec2I ScreenSpaceToCameraFrameCoordinates(Vector2 screenSpaceCoordinate, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        float viewportOrigX = bgTextureViewPortRect.xMin;
        float viewportOrigY = bgTextureViewPortRect.yMin;
        float viewportSizeX = bgTextureViewPortRect.width;
        float viewportSizeY = bgTextureViewPortRect.height;

        bool isPortrait = false;

        float textureSizeX = videoModeData.width;
        float textureSizeY = videoModeData.height;

        float prefixX = 0.0f;
        float prefixY = 0.0f;

        float inversionMultiplierX = 0.0f;
        float inversionMultiplierY = 0.0f;

        PrepareCoordinateConversion(isTextureMirrored, ref prefixX, ref prefixY, ref inversionMultiplierX, ref inversionMultiplierY, ref isPortrait);

        // normalize the coordinates within viewport between 0 and 1
        float normalizedCoordX = (screenSpaceCoordinate.x - viewportOrigX) / viewportSizeX;
        float normalizedCoordY = (screenSpaceCoordinate.y - viewportOrigY) / viewportSizeY;

        QCARRenderer.Vec2I result;

        // convert from screen coordinates to texture coordinates
        if (isPortrait)
        {
            result = new QCARRenderer.Vec2I(Mathf.RoundToInt((prefixX + (inversionMultiplierX * normalizedCoordY)) * textureSizeX),
                                            Mathf.RoundToInt((prefixY + (inversionMultiplierY * normalizedCoordX)) * textureSizeY));
        }
        else
        {
            result = new QCARRenderer.Vec2I(Mathf.RoundToInt((prefixX + (inversionMultiplierX * normalizedCoordX)) * textureSizeX),
                                            Mathf.RoundToInt((prefixY + (inversionMultiplierY * normalizedCoordY)) * textureSizeY));
        }

        return result;
    }

    /// <summary>
    /// Calculates a position in screen space coordinates based on the current orientation and background config for a given screen-space position
    /// </summary>
    public static Vector2 CameraFrameToScreenSpaceCoordinates(Vector2 cameraFrameCoordinate, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        float viewportOrigX = bgTextureViewPortRect.xMin;
        float viewportOrigY = bgTextureViewPortRect.yMin;
        float viewportSizeX = bgTextureViewPortRect.width;
        float viewportSizeY = bgTextureViewPortRect.height;

        bool isPortrait = false;

        float textureSizeX = videoModeData.width;
        float textureSizeY = videoModeData.height;

        float prefixX = 0.0f;
        float prefixY = 0.0f;

        float inversionMultiplierX = 0.0f;
        float inversionMultiplierY = 0.0f;

        PrepareCoordinateConversion(isTextureMirrored, ref prefixX, ref prefixY, ref inversionMultiplierX, ref inversionMultiplierY, ref isPortrait);

        // normalize the coordinates within viewport between 0 and 1
        float normalizedCoordX = (cameraFrameCoordinate.x/textureSizeX - prefixX)/inversionMultiplierX;
        float normalizedCoordY = (cameraFrameCoordinate.y/textureSizeY - prefixY)/inversionMultiplierY;

        Vector2 result;

        // convert from screen coordinates to texture coordinates
        if (isPortrait)
        {
            result = new Vector2(viewportSizeX * normalizedCoordY + viewportOrigX,
                                 viewportSizeY * normalizedCoordX + viewportOrigY);
        }
        else
        {
             result = new Vector2(viewportSizeX * normalizedCoordX + viewportOrigX,
                                  viewportSizeY * normalizedCoordY + viewportOrigY);
        }

        return result;
    }

    /// <summary>
    /// Calculates the screen space parameters for an oriented bounding box (center, half extents, rotation) specified in camera frame coordinates.
    /// The calculation is based on the current screen orientation.
    /// </summary>
    public static OrientedBoundingBox CameraFrameToScreenSpaceCoordinates(OrientedBoundingBox cameraFrameObb, Rect bgTextureViewPortRect, bool isTextureMirrored, CameraDevice.VideoModeData videoModeData)
    {
        bool isPortrait = false;
        float obbRotation = 0.0f;
        switch (QCARRuntimeUtilities.ScreenOrientation)
        {
            case ScreenOrientation.Portrait:
                obbRotation += 90.0f;
                isPortrait = true;
                break;
            case ScreenOrientation.LandscapeRight:
                obbRotation += 180.0f;
                break;
            case ScreenOrientation.PortraitUpsideDown:
                obbRotation += 270.0f;
                isPortrait = true;
                break;
        }
        
        var scaleX = bgTextureViewPortRect.width / (isPortrait ? videoModeData.height : videoModeData.width);
        var scaleY = bgTextureViewPortRect.height / (isPortrait ? videoModeData.width : videoModeData.height);


        var center = CameraFrameToScreenSpaceCoordinates(cameraFrameObb.Center, bgTextureViewPortRect,
                                                         isTextureMirrored, videoModeData);
        var halfExtents = new Vector2(cameraFrameObb.HalfExtents.x * scaleX, cameraFrameObb.HalfExtents.y * scaleY);

        var rotation = cameraFrameObb.Rotation;
        if (isTextureMirrored) rotation = -rotation;
        rotation = rotation*180.0f/Mathf.PI + obbRotation;

        return new OrientedBoundingBox(center, halfExtents, rotation);
    }


    /// <summary>
    /// Selects the top left and bottom right corners from a rect, where "top", "left", "bottom" and "right" are in respect to landscape left orientation
    /// Used for region of interest calculations for text tracking
    /// </summary>
    public static void SelectRectTopLeftAndBottomRightForLandscapeLeft(Rect screenSpaceRect, bool isMirrored, out Vector2 topLeft, out Vector2 bottomRight)
    {
        // take into account video background mirroring
        if (!isMirrored)
        {
            switch (ScreenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    goto default;

                case ScreenOrientation.LandscapeRight:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    break;

                case ScreenOrientation.Portrait:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    break;

                case ScreenOrientation.PortraitUpsideDown:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    break;

                default:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    break;
            }
        }
        else
        {
            switch (ScreenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    goto default;

                case ScreenOrientation.LandscapeRight:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    break;

                case ScreenOrientation.Portrait:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    break;

                case ScreenOrientation.PortraitUpsideDown:
                    topLeft = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMin);
                    bottomRight = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMax);
                    break;

                default:
                    topLeft = new Vector2(screenSpaceRect.xMax, screenSpaceRect.yMin);
                    bottomRight = new Vector2(screenSpaceRect.xMin, screenSpaceRect.yMax);
                    break;
            }
        }
    }


    /// <summary>
    /// Creates a rect from given corner points, where "top", "left", "bottom" and "right" are in respect to landscape left orientation
    /// Used for region of interest calculations for text tracking
    /// </summary>
    public static Rect CalculateRectFromLandscapeLeftCorners(Vector2 topLeft, Vector2 bottomRight, bool isMirrored)
    {
        Rect rect;

        // take into account video background mirroring
        if (!isMirrored)
        {
            switch (ScreenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    goto default;

                case ScreenOrientation.LandscapeRight:
                    rect = new Rect(bottomRight.x, bottomRight.y,
                                    topLeft.x - bottomRight.x, topLeft.y - bottomRight.y);
                    break;

                case ScreenOrientation.Portrait:
                    rect = new Rect(bottomRight.x, topLeft.y,
                                    topLeft.x - bottomRight.x, bottomRight.y - topLeft.y);
                    break;

                case ScreenOrientation.PortraitUpsideDown:
                    rect = new Rect(topLeft.x, bottomRight.y,
                                    bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
                    break;

                default:
                    rect = new Rect(topLeft.x, topLeft.y,
                                    bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
                    break;
            }
        }
        else
        {
            switch (ScreenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    goto default;

                case ScreenOrientation.LandscapeRight:
                    rect = new Rect(topLeft.x, bottomRight.y,
                                    bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
                    break;

                case ScreenOrientation.Portrait:
                    rect = new Rect(bottomRight.x, bottomRight.y,
                                    topLeft.x - bottomRight.x, topLeft.y - bottomRight.y);
                    break;

                case ScreenOrientation.PortraitUpsideDown:
                    rect = new Rect(topLeft.x, topLeft.y,
                                    bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
                    break;

                default:
                    rect = new Rect(bottomRight.x, topLeft.y,
                                    topLeft.x - bottomRight.x, bottomRight.y - topLeft.y);
                    break;
            }
        }

        return rect;
    }

    /// <summary>
    /// The device screen stays turned on and bright 
    /// </summary>
    public static void DisableSleepMode()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    
    /// <summary>
    /// Reset the sleep mode to the system settings.
    /// </summary>
    public static void ResetSleepMode()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private static void PrepareCoordinateConversion(bool isTextureMirrored, ref float prefixX, ref float prefixY, ref float inversionMultiplierX, ref float inversionMultiplierY, ref bool isPortrait)
    {
        // determine for which orientation the variables should be set up:
        switch (ScreenOrientation)
        {
            case ScreenOrientation.Portrait:
                isPortrait = true;
                if (!isTextureMirrored)
                {
                    prefixX = 0.0f;
                    prefixY = 1.0f;

                    inversionMultiplierX = 1.0f;
                    inversionMultiplierY = -1.0f;
                }
                else
                {
                    prefixX = 1.0f;
                    prefixY = 1.0f;

                    inversionMultiplierX = -1.0f;
                    inversionMultiplierY = -1.0f;
                }

                break;

            case ScreenOrientation.PortraitUpsideDown:
                isPortrait = true;
                if (!isTextureMirrored)
                {
                    prefixX = 1.0f;
                    prefixY = 0.0f;

                    inversionMultiplierX = -1.0f;
                    inversionMultiplierY = 1.0f;
                }
                else
                {
                    prefixX = 0.0f;
                    prefixY = 0.0f;

                    inversionMultiplierX = 1.0f;
                    inversionMultiplierY = 1.0f;
                }

                break;

            case ScreenOrientation.LandscapeRight:
                isPortrait = false;
                if (!isTextureMirrored)
                {
                    prefixX = 1.0f;
                    prefixY = 1.0f;

                    inversionMultiplierX = -1.0f;
                    inversionMultiplierY = -1.0f;
                }
                else
                {
                    prefixX = 0.0f;
                    prefixY = 1.0f;

                    inversionMultiplierX = 1.0f;
                    inversionMultiplierY = -1.0f;
                }

                break;

            default:
                isPortrait = false;
                if (!isTextureMirrored)
                {
                    prefixX = 0.0f;
                    prefixY = 0.0f;

                    inversionMultiplierX = 1.0f;
                    inversionMultiplierY = 1.0f;
                }
                else
                {
                    prefixX = 1.0f;
                    prefixY = 0.0f;

                    inversionMultiplierX = -1.0f;
                    inversionMultiplierY = 1.0f;
                }

                break;
        }
    }

    #endregion // PRIVATE_METHODS

}