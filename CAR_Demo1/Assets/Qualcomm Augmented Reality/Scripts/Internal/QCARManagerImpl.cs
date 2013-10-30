/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class QCARManagerImpl : QCARManager
{
    #region NESTED

    // This struct stores 3D pose information as a position-vector,
    // orientation-Quaternion pair. The pose is given relatively to the camera.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PoseData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Vector3 position;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Quaternion orientation;
    }

    // This struct stores general data about a trackable result like its 3D pose, its status
    // and its unique id.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TrackableResultData
    {
        public PoseData pose;
        public TrackableBehaviour.Status status;
        public int id;
    }

    // This struct stores Virtual Button data like its current status (pressed
    // or not pressed) and its unique id.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VirtualButtonData
    {
        public int id;
        public int isPressed;
    }

    // This struct stores information for a 2D oriented bounding box
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Obb2D
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector2 center;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector2 halfExtents;
        public float rotation;
    }

    // This struct stores general data about a word trackable result like its 3D pose, 
    // its status, oriented bounding box and its unique id.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WordResultData
    {
        public PoseData pose;
        public TrackableBehaviour.Status status;
        public int id;
        public Obb2D orientedBoundingBox;
    }

    // This struct encapsulates data for a single word, like its string, the size of 
    // its bounding box
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WordData
    {
        public int id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector2 size;
        public IntPtr stringValue;
    }


    // This struct stores data of an image header. It includes the width and
    // height of the image, the byte stride in the buffer, the buffer size
    // (which can differ from the image size e.g. when image is converted to a
    // power of two size) and the format of the image
    // (e.g. RGB565, grayscale, etc.).
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImageHeaderData
    {
        public int width;
        public int height;
        public int stride;
        public int bufferWidth;
        public int bufferHeight;
        public int format;
        public int reallocate;
        public int updated;
        public IntPtr data;
    }

    // This struct stores information about the state of the frame that was last processed by QCAR
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct FrameState
    {
        public int numTrackableResults;
        public int numVirtualButtonResults;
        public int frameIndex;
        public IntPtr trackableDataArray;
        public IntPtr vbDataArray;
        public int numWordResults;
        public IntPtr wordResultArray;
        public int numNewWords;
        public IntPtr newWordDataArray;
        public IntPtr videoModeData;
    }

    private struct AutoRotationState
    {
        public bool setOnPause;
        public bool autorotateToPortrait;
        public bool autorotateToPortraitUpsideDown;
        public bool autorotateToLandscapeLeft;
        public bool autorotateToLandscapeRight;
    }

    #endregion // NESTED



    #region PROPERTIES

    // World Center Mode setting on the ARCamera
    public override QCARBehaviour.WorldCenterMode WorldCenterMode
    {
        set { mWorldCenterMode = value; }
        get { return mWorldCenterMode; }
    }

    // World Center setting on the ARCamera
    public override TrackableBehaviour WorldCenter
    {
        set { mWorldCenter = value; }
        get { return mWorldCenter; }
    }

    // A handle to the ARCamera object
    public override Camera ARCamera
    {
        set { mARCamera = value; }
        get { return mARCamera; }
    }

    // True to have QCAR render the video background image natively
    // False to bind the video background to the texture set in
    // QCARRenderer.SetVideoBackgroundTextureID
    public override bool DrawVideoBackground
    {
        set { mDrawVideobackground = value; }
        get { return mDrawVideobackground; }
    }

    /// <summary>
    /// returns true once the QCARManager has been initialized
    /// </summary>
    public override bool Initialized
    {
        get { return mInitialized; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    private QCARBehaviour.WorldCenterMode mWorldCenterMode;
    private TrackableBehaviour mWorldCenter = null;
    private Camera mARCamera = null;
    private TrackableResultData[] mTrackableResultDataArray = null;
    private WordData[] mWordDataArray = null;
    private WordResultData[] mWordResultDataArray = null;

    private LinkedList<int> mTrackableFoundQueue = new LinkedList<int>();

    private IntPtr mImageHeaderData = IntPtr.Zero;
    private int mNumImageHeaders = 0;

    private bool mDrawVideobackground = true;

    // frame index of the next injected frame when in emulator mode
    private int mInjectedFrameIdx = 0;

    // ptr to index of frame last processed by Vuforia
    private IntPtr mLastProcessedFrameStatePtr = IntPtr.Zero;

    private bool mInitialized = false;
    private bool mPaused = false;

    // the current frame state holding info about trackables results etc.
    private FrameState mFrameState;

    // auto rotation state set when background is frozen and orientation is locked
    private AutoRotationState mAutoRotationState;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // Initialization
    public override bool Init()
    {
        mTrackableResultDataArray = new TrackableResultData[0];
        mWordDataArray = new WordData[0];
        mWordResultDataArray = new WordResultData[0];

        mTrackableFoundQueue = new LinkedList<int>();

        mImageHeaderData = IntPtr.Zero;
        mNumImageHeaders = 0;

        mLastProcessedFrameStatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FrameState)));
        QCARWrapper.Instance.InitFrameState(mLastProcessedFrameStatePtr);

        InitializeTrackableContainer(0);

        mInitialized = true;

        return true;
    }


    // Process the camera image and tracking data for this frame
    public void Update(ScreenOrientation counterRotation, CameraDevice.CameraDeviceMode deviceMode, ref CameraDevice.VideoModeData videoMode)
    {
        if (mPaused)
        {
            // set the last frame again, do not update the state
            QCARWrapper.Instance.PausedUpdateQCAR();
        }
        else
        {
            // enable "fake tracking" if running in the free editor version 
            // that does not support native plugins
            if (!QCARRuntimeUtilities.IsQCAREnabled())
            {
                UpdateTrackablesEditor();
                return;
            }

            // Prepare the camera image container
            UpdateImageContainer();

            if (QCARRuntimeUtilities.IsPlayMode())
            {
                CameraDeviceImpl cameraDeviceImpl = (CameraDeviceImpl) CameraDevice.Instance;
                if (cameraDeviceImpl.WebCam.DidUpdateThisFrame)
                {
                    InjectCameraFrame();
                }
            }

            // Draw the video background or update the video texture
            // Also retrieve registered camera images for this frame
            QCARWrapper.Instance.UpdateQCAR(mImageHeaderData, mNumImageHeaders,
                                            mLastProcessedFrameStatePtr, (int) counterRotation,
                                            (int)deviceMode);

            mFrameState = (FrameState)Marshal.PtrToStructure(mLastProcessedFrameStatePtr, typeof(FrameState));
        }


        // Get video mode data
        if (QCARRuntimeUtilities.IsPlayMode())
            videoMode = CameraDevice.Instance.GetVideoMode(deviceMode);
        else
        {
            var videoModePtr = mFrameState.videoModeData;
            videoMode =
                (CameraDevice.VideoModeData)
                Marshal.PtrToStructure(videoModePtr, typeof (CameraDevice.VideoModeData));
        }
        

        // Reinitialize the trackable data container if required:
        InitializeTrackableContainer(mFrameState.numTrackableResults);

        // Handle the camera image data
        UpdateCameraFrame();

        // Handle the trackable data
        UpdateTrackers(mFrameState);

        if (QCARRuntimeUtilities.IsPlayMode())
        {
            // read out the index of the last processed frame
            CameraDeviceImpl cameraDeviceImpl = (CameraDeviceImpl)CameraDevice.Instance;
            cameraDeviceImpl.WebCam.SetFrameIndex(mFrameState.frameIndex);
        }

        // if native video background rendering is disabled, we need to call the method to draw into the
        // offscreen texture here in the update loop.
        // rendering the video background into the texture in the PrepareRendering callback will result 
        // in the texture updated in the next frame, which results in a lag between augmentations and the 
        // video background
        if (!mDrawVideobackground)
        {
            RenderVideoBackgroundOrDrawIntoTextureInNative();
        }
    }

    /// <summary>
    /// Renders the video background in the native plugin.
    /// Does nothing if internal state is set to draw the background into an offscreen texture
    /// </summary>
    public void PrepareRendering()
    {
        // only call the render function here if native video background rendering is enabled.
        // for background texture access, the texture is drawn into in the update function.
        if (mDrawVideobackground)
        {
            RenderVideoBackgroundOrDrawIntoTextureInNative();
        }
    }

    public void FinishRendering()
    {
        // call renderer.end() in native to invalidate rendering resources
        QCARWrapper.Instance.RendererEnd();
    }


    // Free globally allocated containers
    public override void Deinit()
    {
        if (mInitialized)
        {
            Marshal.FreeHGlobal(mImageHeaderData);
            QCARWrapper.Instance.DeinitFrameState(mLastProcessedFrameStatePtr);
            Marshal.FreeHGlobal(mLastProcessedFrameStatePtr);

            mInitialized = false;
            mPaused = false;
        }
    }

    /// <summary>
    /// Turns pausing on or off.
    /// Pausing will freeze the camera video and all trackables will remain in their current state.
    /// Autorotation will be disabled during video background freezing.
    /// </summary>
    public void Pause(bool pause)
    {
        if (pause)
        {
            // lock orientation
            mAutoRotationState = new AutoRotationState
                {
                    autorotateToLandscapeLeft = Screen.autorotateToLandscapeLeft,
                    autorotateToLandscapeRight = Screen.autorotateToLandscapeRight,
                    autorotateToPortrait = Screen.autorotateToPortrait,
                    autorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown,
                    setOnPause = true
                };

            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
        else
        {
            // enable autorotation again if it was disabled before:
            if (mAutoRotationState.setOnPause)
            {
                Screen.autorotateToLandscapeLeft = mAutoRotationState.autorotateToLandscapeLeft;
                Screen.autorotateToLandscapeRight = mAutoRotationState.autorotateToLandscapeRight;
                Screen.autorotateToPortrait = mAutoRotationState.autorotateToPortrait;
                Screen.autorotateToPortraitUpsideDown = mAutoRotationState.autorotateToPortraitUpsideDown;
            }
        }

        mPaused = pause;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    // Initialize the container for retrieving tracking data from native
    private void InitializeTrackableContainer(int numTrackableResults)
    {
        if (mTrackableResultDataArray.Length != numTrackableResults)
        {
            mTrackableResultDataArray = new TrackableResultData[numTrackableResults];

            Debug.Log("Num trackables detected: " + numTrackableResults);
        }
    }


    // Unmarshal and process the tracking data
    private void UpdateTrackers(FrameState frameState)
    {
        // Unmarshal the trackable data
        // Take our array of unmanaged data from native and create an array of
        // TrackableResultData structures to work with (one per trackable, regardless
        // of whether or not that trackable is visible this frame).
        for (int i = 0; i < frameState.numTrackableResults; i++)
        {
            IntPtr trackablePtr = new IntPtr(frameState.trackableDataArray.ToInt32() + i *
                    Marshal.SizeOf(typeof(TrackableResultData)));
            TrackableResultData trackableResultData = (TrackableResultData)
                    Marshal.PtrToStructure(trackablePtr, typeof(TrackableResultData));
            mTrackableResultDataArray[i] = trackableResultData;
        }

        // Add newly found Trackables to the queue, remove lost ones
        // We keep track of the order in which Trackables become visible for the
        // AUTO World Center mode. This keeps the camera from jumping around in the
        // scene too much.
        foreach (TrackableResultData trackableData in mTrackableResultDataArray)
        {
            // We found a trackable (or set of Trackables) that match this id
            if ((trackableData.status == TrackableBehaviour.Status.DETECTED
                 || trackableData.status ==
                 TrackableBehaviour.Status.TRACKED))
            {
                if (!mTrackableFoundQueue.Contains(trackableData.id))
                {
                    // The trackable just became visible, add it to the queue
                    mTrackableFoundQueue.AddLast(trackableData.id);
                }
            }
            else
            {
                if (mTrackableFoundQueue.Contains(trackableData.id))
                {
                    // The trackable just disappeared, remove it from the queue
                    mTrackableFoundQueue.Remove(trackableData.id);
                }
            }
        }
        
        // now remove those from the TrackableFoundQueue that were not in this frame's results:
        List<int> trackableFoundQueueCopy = new List<int>(mTrackableFoundQueue);
        foreach (int id in trackableFoundQueueCopy)
        {
            // check if the trackable is in this frame's results:
            if (Array.Exists(mTrackableResultDataArray, tr => tr.id == id))
                break;

            // not in the results, remove it from the queue
            mTrackableFoundQueue.Remove(id);
        }

        StateManagerImpl stateManager = (StateManagerImpl)
            TrackerManager.Instance.GetStateManager();

        // The "scene origin" is only used in world center mode auto or user.
        int originTrackableID = -1;

        if (mWorldCenterMode == QCARBehaviour.WorldCenterMode.SPECIFIC_TARGET &&
            mWorldCenter != null)
        {
            originTrackableID = mWorldCenter.Trackable.ID;
        }
        else if (mWorldCenterMode == QCARBehaviour.WorldCenterMode.FIRST_TARGET)
        {
            stateManager.RemoveDisabledTrackablesFromQueue(ref mTrackableFoundQueue);
            if (mTrackableFoundQueue.Count > 0)
            {
                originTrackableID = mTrackableFoundQueue.First.Value;
            }
        }

        // Unmarshal words and word results
        UpdateWordTrackables(frameState);

        // Update the Camera pose before Trackable poses are updated.
        stateManager.UpdateCameraPose(mARCamera, mTrackableResultDataArray, originTrackableID);

        // Update the Trackable poses.
        stateManager.UpdateTrackablePoses(mARCamera, mTrackableResultDataArray, originTrackableID, frameState.frameIndex);

        // Update Word Trackables
        stateManager.UpdateWords(mARCamera, mWordDataArray, mWordResultDataArray);

        // Update Virtual Button states.
        stateManager.UpdateVirtualButtons(frameState.numVirtualButtonResults, frameState.vbDataArray);
    }


    // Simulate tracking in the editor
    private void UpdateTrackablesEditor()
    {
        // When running within the Unity editor without emulation mode:
        TrackableBehaviour[] trackableBehaviours = (TrackableBehaviour[])
                UnityEngine.Object.FindObjectsOfType(typeof(TrackableBehaviour));

        // Simulate all Trackables were tracked successfully:    
        foreach (TrackableBehaviour trackable in trackableBehaviours)
        {
            if (trackable.enabled)
            {
                // Word trackables have to be created for all word behaviours as these are
                // created dynamically otherwise
                if (trackable is WordBehaviour)
                {
                    var ewb = (IEditorWordBehaviour) trackable;
                    ewb.SetNameForTrackable(ewb.IsSpecificWordMode ? ewb.SpecificWord : "AnyWord");
                    ewb.InitializeWord(new WordImpl(0, ewb.TrackableName, new Vector2(500, 100)));
                }
                trackable.OnTrackerUpdate(TrackableBehaviour.Status.TRACKED);
            }
        }
    }

    private void UpdateWordTrackables(FrameState frameState)
    {
        // Unmarshal new word data
        mWordDataArray = new WordData[frameState.numNewWords];
        for (int i = 0; i < frameState.numNewWords; i++)
        {
            IntPtr trackablePtr = new IntPtr(frameState.newWordDataArray.ToInt32() + i *
                    Marshal.SizeOf(typeof(WordData)));
            mWordDataArray[i] = (WordData)Marshal.PtrToStructure(trackablePtr, typeof(WordData));
        }
        // Unmarshal word result data
        mWordResultDataArray = new WordResultData[frameState.numWordResults];
        for (int i = 0; i < frameState.numWordResults; i++)
        {
            IntPtr trackablePtr = new IntPtr(frameState.wordResultArray.ToInt32() + i *
                Marshal.SizeOf(typeof(WordResultData)));
            mWordResultDataArray[i] = (WordResultData)Marshal.PtrToStructure(trackablePtr, typeof(WordResultData));
        }
    }

    // Update the image container for the currently registered formats
    private void UpdateImageContainer()
    {
        CameraDeviceImpl cameraDeviceImpl = (CameraDeviceImpl)CameraDevice.Instance;

        // Reallocate the data container if the number of requested images has
        // changed, or if the container is not allocated
        if (mNumImageHeaders != cameraDeviceImpl.GetAllImages().Count ||
           (cameraDeviceImpl.GetAllImages().Count > 0 && mImageHeaderData == IntPtr.Zero))
        {

            mNumImageHeaders = cameraDeviceImpl.GetAllImages().Count;

            Marshal.FreeHGlobal(mImageHeaderData);
            mImageHeaderData = Marshal.AllocHGlobal(Marshal.SizeOf(
                                typeof(ImageHeaderData)) * mNumImageHeaders);
        }

        // Update the image info:
        int i = 0;
        foreach (ImageImpl image in cameraDeviceImpl.GetAllImages().Values)
        {
            IntPtr imagePtr = new IntPtr(mImageHeaderData.ToInt32() + i *
                   Marshal.SizeOf(typeof(ImageHeaderData)));

            ImageHeaderData imageHeader = new ImageHeaderData();
            imageHeader.width = image.Width;
            imageHeader.height = image.Height;
            imageHeader.stride = image.Stride;
            imageHeader.bufferWidth = image.BufferWidth;
            imageHeader.bufferHeight = image.BufferHeight;
            imageHeader.format = (int)image.PixelFormat;
            imageHeader.reallocate = 0;
            imageHeader.updated = 0;
            imageHeader.data = image.UnmanagedData;

            Marshal.StructureToPtr(imageHeader, imagePtr, false);
            ++i;
        }
    }


    // Unmarshal the camera images for this frame
    private void UpdateCameraFrame()
    {
        // Unmarshal the image data:
        int i = 0;
        CameraDeviceImpl cameraDeviceImpl = (CameraDeviceImpl)CameraDevice.Instance;
        foreach (ImageImpl image in cameraDeviceImpl.GetAllImages().Values)
        {
            IntPtr imagePtr = new IntPtr(mImageHeaderData.ToInt32() + i *
                   Marshal.SizeOf(typeof(ImageHeaderData)));
            ImageHeaderData imageHeader = (ImageHeaderData)
                Marshal.PtrToStructure(imagePtr, typeof(ImageHeaderData));

            // Copy info back to managed Image instance:
            image.Width = imageHeader.width;
            image.Height = imageHeader.height;
            image.Stride = imageHeader.stride;
            image.BufferWidth = imageHeader.bufferWidth;
            image.BufferHeight = imageHeader.bufferHeight;
            image.PixelFormat = (Image.PIXEL_FORMAT) imageHeader.format;

            // Reallocate if required:
            if (imageHeader.reallocate == 1)
            {
                image.Pixels = new byte[QCARWrapper.Instance.QcarGetBufferSize(image.BufferWidth,
                                                    image.BufferHeight,
                                                    (int)image.PixelFormat)];

                Marshal.FreeHGlobal(image.UnmanagedData);

                image.UnmanagedData = Marshal.AllocHGlobal(QCARWrapper.Instance.QcarGetBufferSize(image.BufferWidth,
                                    image.BufferHeight,
                                    (int)image.PixelFormat));

                // Note we don't copy the data this frame as the unmanagedVirtualButtonBehaviour
                // buffer was not filled.
            }
            else if (imageHeader.updated == 1)
            {
                // Copy data:
                image.CopyPixelsFromUnmanagedBuffer();
            }

            ++i;
        }
    }

    // gets a snapshot from the 
    private void InjectCameraFrame()
    {
        CameraDeviceImpl cameraDeviceImpl = (CameraDeviceImpl)CameraDevice.Instance;
        Color32[] pixels = cameraDeviceImpl.WebCam.GetPixels32AndBufferFrame(mInjectedFrameIdx);
        GCHandle pixelHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        IntPtr pixelPointer = pixelHandle.AddrOfPinnedObject();
        int width = cameraDeviceImpl.WebCam.ActualWidth;
        int height = cameraDeviceImpl.WebCam.ActualHeight;

        // add a camera frame - it always has to be rotated and flipped by default
        QCARWrapper.Instance.QcarAddCameraFrame(pixelPointer, width, height, (int)Image.PIXEL_FORMAT.RGBA8888, 4 * width, mInjectedFrameIdx, cameraDeviceImpl.WebCam.FlipHorizontally ? 1 : 0);
        mInjectedFrameIdx++;
        pixelPointer = IntPtr.Zero;
        pixelHandle.Free();
    }

    private void RenderVideoBackgroundOrDrawIntoTextureInNative()
    {
        // Render the video background
        QCARWrapper.Instance.RendererRenderVideoBackground(mDrawVideobackground ? 0 : 1);

        // Tell Unity that we may have changed the OpenGL state behind the scenes
        GL.InvalidateState();
    }

    #endregion // PRIVATE_METHODS
}
