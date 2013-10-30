/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;

/// <summary>
/// This script sets up the background shader effect and contains the logic
/// to capture longer touch "drag" events that distort the video background. 
/// It also checks for OpenGL ES 2.0 support.
/// The background texture access sample does not support OpenGL ES 1.x
/// </summary>
[RequireComponent(typeof(VideoTextureBehaviour))]
[RequireComponent(typeof(GLErrorHandler))]
public class NegativeGrayscaleEffect : MonoBehaviour
{
    #region CONSTANTS

    /// <summary>
    /// milliseconds before a touch event 
    /// </summary>
    public const int TAP_DELAY_MILLISEC = 200;

    #endregion



    #region PRIVATE_MEMBER_VARIABLES

    // time of last press down event
    private DateTime mLastTapDown;
    
    private bool mErrorOccurred = false;

    private const string ERROR_TEXT = "The BackgroundTextureAccess sample requires OpenGL ES 2.0";
    private const string CHECK_STRING = "OpenGL ES 2.0";

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        // This sample requires OpenGL ES 2.0 otherwise it won't work.
        mErrorOccurred = !IsOpenGLES2();

        if (mErrorOccurred)
        {
            Debug.LogError(ERROR_TEXT);

            // Show a dialog box with an error message.
            GLErrorHandler.SetError(ERROR_TEXT);

            // Turn off renderer to make sure the unsupported shader is not used.
            renderer.enabled = false;

            TrackableBehaviour[] tbs = (TrackableBehaviour[])FindObjectsOfType(typeof(TrackableBehaviour));
            if (tbs != null)
            {
                for (int i = 0; i < tbs.Length; ++i)
                {
                    tbs[i].enabled = false;
                }
            }
        }
    }

    void Update()
    {
        float touchX = 2.0F;
        float touchY = 2.0F;

        if (Input.GetMouseButtonDown(0))
        {
            // remember last tap down
            mLastTapDown = DateTime.Now;
        }

        if (Input.GetMouseButton(0))
        {
            // don't trigger effect immediately to not interfere with menu
            if ((DateTime.Now - mLastTapDown) > TimeSpan.FromMilliseconds(TAP_DELAY_MILLISEC))
            {
                // Adjust the touch point for the current orientation
                if (QCARRuntimeUtilities.ScreenOrientation == ScreenOrientation.Landscape)
                {
                    touchX = (Input.mousePosition.x/Screen.width) - 0.5F;
                    touchY = (Input.mousePosition.y/Screen.height) - 0.5F;
                }
                else if (QCARRuntimeUtilities.ScreenOrientation == ScreenOrientation.Portrait)
                {
                    touchX = ((Input.mousePosition.y/Screen.height) - 0.5F)*-1;
                    touchY = (Input.mousePosition.x/Screen.width) - 0.5F;
                }
                else if (QCARRuntimeUtilities.ScreenOrientation == ScreenOrientation.LandscapeRight)
                {
                    touchX = ((Input.mousePosition.x/Screen.width) - 0.5F)*-1;
                    touchY = ((Input.mousePosition.y/Screen.height) - 0.5F)*-1;
                }
                else if (QCARRuntimeUtilities.ScreenOrientation == ScreenOrientation.PortraitUpsideDown)
                {
                    touchX = (Input.mousePosition.y/Screen.height) - 0.5F;
                    touchY = ((Input.mousePosition.x/Screen.width) - 0.5F)*-1;
                }
            }
        }

        renderer.material.SetFloat("_TouchX", touchX);
        renderer.material.SetFloat("_TouchY", touchY);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

    /// <summary>
    /// This method checks if we are using OpenGL ES 2.0.
    /// </summary>
    private bool IsOpenGLES2()
    {
        // in play mode on a desktop machine, always return true
        if (QCARRuntimeUtilities.IsPlayMode()) return true;

        string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;

        Debug.Log("Sample using " + graphicsDeviceVersion);

        return (graphicsDeviceVersion.IndexOf(CHECK_STRING, StringComparison.Ordinal) >= 0);
    }

    #endregion // PRIVATE_METHODS
}
