/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;

/// <summary>
/// Menu that appears on double tap, enables and disables the AutoFocus on the camera.
/// </summary>
public class BackgroundTextureMenu : MonoBehaviour, ITrackerEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    // Check if a menu button has been pressed.
    private bool mButtonPressed = false;

    // If the menu is currently open
    private bool mMenuOpen = false;

    // Contains if the device supports continous autofocus
    private bool mContinousAFSupported = true;

    // Contains the currently set auto focus mode.
    private CameraDevice.FocusMode mFocusMode =
        CameraDevice.FocusMode.FOCUS_MODE_NORMAL;

    // Contains the rectangle for the camera options menu.
    private Rect mAreaRect;

    // this is used to distinguish single and double taps
    private bool mWaitingForSecondTap;
    private Vector3 mFirstTapPosition;
    private DateTime mFirstTapTime;
    // the maximum distance that is allowed between two taps to make them count as a double tap
    // (relative to the screen size)
    private const float MAX_TAP_DISTANCE_SCREEN_SPACE = 0.1f;
    private const int MAX_TAP_MILLISEC = 500;

    // used to determine if if a short tap or a hold screen event was performend
    private DateTime mLastTapDown;
    private bool mTapIsHeldDown;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS

    public void Start()
    {
        // register for the OnInitialized event at the QCARBehaviour
        QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.RegisterTrackerEventHandler(this);
        }

        // Setup position and size of the camera menu.
        ComputePosition();
    }

    
    public void Update()
    {
        // If the touch event results from a button press it is ignored.
        if (!mButtonPressed)
        {
            // if we are returning from a hold screen event, do not consider the touch events further this frame
            if (!WasHoldScreenEvent())
            {
                if (mMenuOpen)
                {
                    // If finger is removed from screen.
                    if (Input.GetMouseButtonUp(0))
                    {
                        HandleSingleTap();
                    }
                }
                else
                {
                    // check if it is a doulbe tap
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (mWaitingForSecondTap)
                        {
                            // check if time and position match:
                            int smallerScreenDimension = Screen.width < Screen.height ? Screen.width : Screen.height;
                            if (DateTime.Now - mFirstTapTime < TimeSpan.FromMilliseconds(MAX_TAP_MILLISEC) &&
                                Vector4.Distance(Input.mousePosition, mFirstTapPosition) <
                                smallerScreenDimension*MAX_TAP_DISTANCE_SCREEN_SPACE)
                            {
                                // it's a double tap
                                HandleDoubleTap();
                            }
                            else
                            {
                                // too late/far to be a double tap, treat it as first tap:
                                mFirstTapPosition = Input.mousePosition;
                                mFirstTapTime = DateTime.Now;
                            }
                        }
                        else
                        {
                            // it's the first tap
                            mWaitingForSecondTap = true;
                            mFirstTapPosition = Input.mousePosition;
                            mFirstTapTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        // time window for second tap has passed, trigger single tap
                        if (mWaitingForSecondTap &&
                            DateTime.Now - mFirstTapTime > TimeSpan.FromMilliseconds(MAX_TAP_MILLISEC))
                        {
                            HandleSingleTap();
                        }
                    }
                }
            }
        }
        else
        {
            mButtonPressed = false;
        }
    }


    // Draw menus.
    public void OnGUI()
    {
        if (mMenuOpen)
        {
            ComputePosition();

            // Setup style for buttons.
            GUIStyle buttonGroupStyle = new GUIStyle(GUI.skin.button);
            buttonGroupStyle.stretchWidth = true;
            buttonGroupStyle.stretchHeight = true;

            GUILayout.BeginArea(mAreaRect);

            GUILayout.BeginHorizontal(buttonGroupStyle);

            if (!mContinousAFSupported)
            {
                if (GUILayout.Button("Cont. Auto Focus not supported on this device", buttonGroupStyle))
                {
                    mMenuOpen = false;
                    mButtonPressed = true;
                }
            }
            else
            {
                // toggle continuous autofocus
                if (mFocusMode == CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO)
                {
                    // button to change to single auto focus:
                    if (GUILayout.Button("Deactivate Cont. Auto Focus", buttonGroupStyle))
                    {
                        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL))
                            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_NORMAL;

                        mMenuOpen = false;
                        mButtonPressed = true;
                    }
                }
                else
                {
                    // button to change to cont. auto focus:
                    if (GUILayout.Button("Activate Cont. Auto Focus", buttonGroupStyle))
                    {
                        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
                            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;

                        mMenuOpen = false;
                        mButtonPressed = true;
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region ITrackerEventHandler_IMPLEMENTATION

    /// <summary>
    /// This method is called when QCAR has finished initializing
    /// </summary>
    public void OnInitialized()
    {
        // try to set continous auto focus as default
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
        {
            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
        }
        else
        {
            Debug.LogError("could not switch to continuous autofocus");
            mContinousAFSupported = false;
        }
    }

    public void OnTrackablesUpdated()
    {
        // not used
    }

    #endregion //ITrackerEventHandler_IMPLEMENTATION



    #region PRIVATE_METHODS

    private void HandleSingleTap()
    {
        mWaitingForSecondTap = false;

        if (mMenuOpen)
            mMenuOpen = false;
        else
        {
            // trigger focus once
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO))
                mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;
        }
    }

    private void HandleDoubleTap()
    {
        mWaitingForSecondTap = false;
        mMenuOpen = true;
    }

    /// Compute the coordinates of the menu depending on the current orientation.
    private void ComputePosition()
    {
        int areaWidth = Screen.width;
        int areaHeight = (Screen.height / 5) * 2;
        int areaLeft = 0;
        int areaTop = Screen.height - areaHeight;
        mAreaRect = new Rect(areaLeft, areaTop, areaWidth, areaHeight);
    }

    /// <summary>
    /// If called each frame, this can be used to determine if we are returning from a hold screen event
    /// </summary>
    private bool WasHoldScreenEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // remember last tap down
            mLastTapDown = DateTime.Now;
        }

        // check if there is a drag event going on -> return 
        if (Input.GetMouseButtonUp(0))
        {
            // remember the hold screen event
            if ((DateTime.Now - mLastTapDown) >
                TimeSpan.FromMilliseconds(NegativeGrayscaleEffect.TAP_DELAY_MILLISEC))
            {
                mTapIsHeldDown = true;
            }
        }

        if (mTapIsHeldDown)
        {
            if (Input.GetMouseButtonUp(0))
            {
                // returning from a hold screen event
                mTapIsHeldDown = false;
                return true;
            }
        }

        return false;
    }

    #endregion // PRIVATE_METHODS
}
