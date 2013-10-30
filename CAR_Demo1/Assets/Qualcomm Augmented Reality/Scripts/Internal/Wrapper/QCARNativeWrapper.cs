/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Runtime.InteropServices;

/// <summary>
/// This class encapsulates the access to native methods
/// by implementing the IQCARWrapper interface
/// </summary>
public class QCARNativeWrapper : IQCARWrapper
{
    #region PUBLIC_METHODS

    public int CameraDeviceInitCamera(int camera)
    {
        return cameraDeviceInitCamera(camera);
    }

    public int CameraDeviceDeinitCamera()
    {
        return cameraDeviceDeinitCamera();
    }

    public int CameraDeviceStartCamera()
    {
        return cameraDeviceStartCamera();
    }

    public int CameraDeviceStopCamera()
    {
        return cameraDeviceStopCamera();
    }

    public int CameraDeviceGetNumVideoModes()
    {
        return cameraDeviceGetNumVideoModes();
    }

    public void CameraDeviceGetVideoMode(int idx,[In, Out]IntPtr videoMode)
    {
#if !UNITY_EDITOR
        cameraDeviceGetVideoMode(idx, videoMode);
#endif
    }

    public int CameraDeviceSelectVideoMode(int idx)
    {
#if UNITY_EDITOR
        return 0;
#else
        return cameraDeviceSelectVideoMode(idx);
#endif
    }

    public int CameraDeviceSetFlashTorchMode(int on)
    {
#if UNITY_EDITOR
        return 1;
#else
        return cameraDeviceSetFlashTorchMode(on);
#endif
    }

    public int CameraDeviceSetFocusMode(int focusMode)
    {
#if UNITY_EDITOR
        return 1;
#else
        return cameraDeviceSetFocusMode(focusMode);
#endif
    }

    public int CameraDeviceSetCameraConfiguration(int width, int height)
    {
#if UNITY_EDITOR
        return cameraDeviceSetCameraConfiguration(width, height);
#else
        return 0;
#endif
    }

    public int QcarSetFrameFormat(int format, int enabled)
    {
        return qcarSetFrameFormat(format, enabled);
    }

    public int DataSetExists(string relativePath, int storageType)
    {
        return dataSetExists(relativePath, storageType);
    }

    public int DataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr)
    {
        return dataSetLoad(relativePath, storageType, dataSetPtr);
    }

    public int DataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr)
    {
        return dataSetGetNumTrackableType(trackableType, dataSetPtr);
    }

    public int DataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray,
                                                         int trackableDataArrayLength, IntPtr dataSetPtr)
    {
        return dataSetGetTrackablesOfType(trackableType, trackableDataArray, trackableDataArrayLength, dataSetPtr);
    }

    public int DataSetGetTrackableName(IntPtr dataSetPtr, int trackableId,
                                                        System.Text.StringBuilder trackableName,
                                                        int nameMaxLength)
    {
        return dataSetGetTrackableName(dataSetPtr, trackableId,trackableName, nameMaxLength);
    }

    public int DataSetCreateTrackable(IntPtr dataSetPtr, IntPtr trackableSourcePtr, System.Text.StringBuilder trackableName,
                                                int nameMaxLength, [In, Out] IntPtr trackableData)
    {
        return dataSetCreateTrackable(dataSetPtr, trackableSourcePtr, trackableName, nameMaxLength, trackableData);
    }

    public int DataSetDestroyTrackable(IntPtr dataSetPtr, int trackableId)
    {
        return dataSetDestroyTrackable(dataSetPtr, trackableId);
    }

    public int DataSetHasReachedTrackableLimit(IntPtr dataSetPtr)
    {
        return dataSetHasReachedTrackableLimit(dataSetPtr);
    }

    public int ImageTargetBuilderBuild(string targetName, float sceenSizeWidth)
    {
        return imageTargetBuilderBuild(targetName, sceenSizeWidth);
    }

    public void ImageTargetBuilderStartScan()
    {
        imageTargetBuilderStartScan();
    }

    public void ImageTargetBuilderStopScan()
    {
        imageTargetBuilderStopScan();
    }

    public int ImageTargetBuilderGetFrameQuality()
    {
        return imageTargetBuilderGetFrameQuality();
    }

    public IntPtr ImageTargetBuilderGetTrackableSource()
    {
        return imageTargetBuilderGetTrackableSource();
    }

    public int ImageTargetCreateVirtualButton(IntPtr dataSetPtr,
                                              string trackableName, string virtualButtonName,
                                              [In, Out] IntPtr rectData)
    {
        return imageTargetCreateVirtualButton(dataSetPtr, trackableName, virtualButtonName, rectData);
    }

    public int ImageTargetDestroyVirtualButton(IntPtr dataSetPtr,
                                               string trackableName, string virtualButtonName)
    {
        return imageTargetDestroyVirtualButton(dataSetPtr, trackableName, virtualButtonName);
    }

    public int VirtualButtonGetId(IntPtr dataSetPtr, string trackableName, string virtualButtonName)
    {
        return virtualButtonGetId(dataSetPtr, trackableName, virtualButtonName);
    }

    public int ImageTargetGetNumVirtualButtons(IntPtr dataSetPtr, string trackableName)
    {
        return imageTargetGetNumVirtualButtons(dataSetPtr, trackableName);
    }

    public int ImageTargetGetVirtualButtons([In, Out]IntPtr virtualButtonDataArray,
                                            [In, Out]IntPtr rectangleDataArray,
                                            int virtualButtonDataArrayLength,
                                            IntPtr dataSetPtr, string trackableName)
    {
        return imageTargetGetVirtualButtons(virtualButtonDataArray, rectangleDataArray, virtualButtonDataArrayLength,
                                            dataSetPtr, trackableName);
    }

    public int ImageTargetGetVirtualButtonName(IntPtr dataSetPtr,
                                               string trackableName,
                                               int idx,
                                               System.Text.StringBuilder vbName,
                                               int nameMaxLength)
    {
        return imageTargetGetVirtualButtonName(dataSetPtr, trackableName, idx, vbName, nameMaxLength);
    }

    public int ImageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size)
    {
        return imageTargetSetSize(dataSetPtr, trackableName, size);
    }

    public int ImageTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size)
    {
        return imageTargetGetSize(dataSetPtr, trackableName, size);
    }

    public int CylinderTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr dimensions)
    {
        return cylinderTargetGetSize(dataSetPtr, trackableName, dimensions);
    }

    public int CylinderTargetSetSideLength(IntPtr dataSetPtr, string trackableName, float sideLength)
    {
        return cylinderTargetSetSideLength(dataSetPtr, trackableName, sideLength);
    }

    public int CylinderTargetSetTopDiameter(IntPtr dataSetPtr, string trackableName, float topDiameter)
    {
        return cylinderTargetSetTopDiameter(dataSetPtr, trackableName, topDiameter);
    }

    public int CylinderTargetSetBottomDiameter(IntPtr dataSetPtr, string trackableName, float bottomDiameter)
    {
        return cylinderTargetSetBottomDiameter(dataSetPtr, trackableName, bottomDiameter);
    }

    public int ImageTrackerStart()
    {
        return imageTrackerStart();
    }

    public void ImageTrackerStop()
    {
        imageTrackerStop();
    }

    public IntPtr ImageTrackerCreateDataSet()
    {
        return imageTrackerCreateDataSet();
    }

    public int ImageTrackerDestroyDataSet(IntPtr dataSetPtr)
    {
        return imageTrackerDestroyDataSet(dataSetPtr);
    }

    public int ImageTrackerActivateDataSet(IntPtr dataSetPtr)
    {
        return imageTrackerActivateDataSet(dataSetPtr);
    }

    public int ImageTrackerDeactivateDataSet(IntPtr dataSetPtr)
    {
        return imageTrackerDeactivateDataSet(dataSetPtr);
    }

    public int MarkerTrackerStart()
    {
        return markerTrackerStart();
    }

    public void MarkerTrackerStop()
    {
        markerTrackerStop();
    }

    public int MarkerTrackerCreateMarker(int id, String trackableName, float size)
    {
        return markerTrackerCreateMarker(id, trackableName, size);
    }

    public int MarkerTrackerDestroyMarker(int trackableId)
    {
        return markerTrackerDestroyMarker(trackableId);
    }

    public void InitFrameState([In, Out] IntPtr frameIndex)
    {
        initFrameState(frameIndex);
    }

    public void DeinitFrameState([In, Out] IntPtr frameIndex)
    {
        deinitFrameState(frameIndex);
    }

    public int PausedUpdateQCAR()
    {
        return pausedUpdateQCAR();
    }

    public void UpdateQCAR([In, Out]IntPtr imageHeaderDataArray,
                                    int imageHeaderArrayLength,
                                    [In, Out]IntPtr frameIndex,
                                    int screenOrientation,
                                    int videoModeIdx)
    {
        updateQCAR(imageHeaderDataArray, imageHeaderArrayLength, frameIndex, screenOrientation, videoModeIdx);
    }

    public void RendererRenderVideoBackground(int bindVideoBackground)
    {
        rendererRenderVideoBackground(bindVideoBackground);
    }

    public void RendererEnd()
    {
        rendererEnd();
    }

    public int QcarGetBufferSize(int width, int height,
                                    int format)
    {
        return qcarGetBufferSize(width, height, format);
    }

    public void QcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally)
    {
#if UNITY_EDITOR
        qcarAddCameraFrame(pixels, width, height, format, stride, frameIdx, flipHorizontally);
#endif
    }

    public void RendererSetVideoBackgroundCfg([In, Out]IntPtr bgCfg)
    {
        rendererSetVideoBackgroundCfg(bgCfg);
    }

    public void RendererGetVideoBackgroundCfg([In, Out]IntPtr bgCfg)
    {
        rendererGetVideoBackgroundCfg(bgCfg);
    }

    public void RendererGetVideoBackgroundTextureInfo([In, Out]IntPtr texInfo)
    {
        rendererGetVideoBackgroundTextureInfo(texInfo);
    }

    public int RendererSetVideoBackgroundTextureID(int textureID)
    {
        return rendererSetVideoBackgroundTextureID(textureID);
    }

    public int RendererIsVideoBackgroundTextureInfoAvailable()
    {
        return rendererIsVideoBackgroundTextureInfoAvailable();
    }

    public int GetInitErrorCode()
    {
        return getInitErrorCode();
    }

    public int IsRendererDirty()
    {
        return isRendererDirty();
    }

    public int QcarSetHint(int hint, int value)
    {
        return qcarSetHint(hint, value);
    }

    public int QcarRequiresAlpha()
    {
        return qcarRequiresAlpha();
    }

    public int GetProjectionGL(float nearClip, float farClip,
                                    [In, Out]IntPtr projMatrix,
                                    int screenOrientation)
    {
        return getProjectionGL(nearClip, farClip, projMatrix, screenOrientation);
    }

    public void SetUnityVersion(int major, int minor, int change)
    {
        setUnityVersion(major, minor, change);
    }

    public int TargetFinderStartInit(string userKey, string secretKey)
    {
        return targetFinderStartInit(userKey, secretKey);
    }

    public int TargetFinderGetInitState()
    {
        return targetFinderGetInitState();
    }

    public int TargetFinderDeinit()
    {
        return targetFinderDeinit();
    }

    public int TargetFinderStartRecognition()
    {
        return targetFinderStartRecognition();
    }

    public int TargetFinderStop()
    {
        return targetFinderStop();
    }

    public void TargetFinderSetUIScanlineColor(float r, float g, float b)
    {
        targetFinderSetUIScanlineColor(r, g, b);
    }

    public void TargetFinderSetUIPointColor(float r, float g, float b)
    {
        targetFinderSetUIPointColor(r, g, b);
    }

    public void TargetFinderUpdate([In, Out] IntPtr targetFinderState)
    {
        targetFinderUpdate(targetFinderState);
    }

    public int TargetFinderGetResults([In, Out] IntPtr searchResultArray, int searchResultArrayLength)
    {
        return targetFinderGetResults(searchResultArray, searchResultArrayLength);
    }

    public int TargetFinderEnableTracking(IntPtr searchResult, [In, Out] IntPtr trackableData)
    {
        return targetFinderEnableTracking(searchResult, trackableData);
    }

    public void TargetFinderGetImageTargets([In, Out] IntPtr trackableIdArray, int trackableIdArrayLength)
    {
        targetFinderGetImageTargets(trackableIdArray, trackableIdArrayLength);
    }

    public void TargetFinderClearTrackables()
    {
        targetFinderClearTrackables();
    }

    public int TextTrackerStart()
    {
        return textTrackerStart();
    }

    public void TextTrackerStop()
    {
        textTrackerStop();
    }

    public int TextTrackerSetRegionOfInterest(int detectionLeftTopX, int detectionLeftTopY, int detectionRightBottomX, int detectionRightBottomY,
                                              int trackingLeftTopX, int trackingLeftTopY, int trackingRightBottomX, int trackingRightBottomY, int upDirection)
    {
        return textTrackerSetRegionOfInterest(detectionLeftTopX, detectionLeftTopY, detectionRightBottomX, detectionRightBottomY,
                                              trackingLeftTopX, trackingLeftTopY, trackingRightBottomX, trackingRightBottomY, upDirection);
    }

    public void TextTrackerGetRegionOfInterest([In, Out] IntPtr detectionROI, [In, Out] IntPtr trackingROI)
    {
        textTrackerGetRegionOfInterest(detectionROI, trackingROI);
    }

    public int WordListLoadWordList(string path, int storageType)
    {
        return wordListLoadWordList(path, storageType);
    }

    public int WordListAddWordsFromFile(string path, int storagetType)
    {
        return wordListAddWordsFromFile(path, storagetType);
    }

    public int WordListAddWordU(IntPtr word)
    {
        return wordListAddWordU(word);
    }

    public int WordListRemoveWordU(IntPtr word)
    {
        return wordListRemoveWordU(word);
    }

    public int WordListContainsWordU(IntPtr word)
    {
        return wordListContainsWordU(word);
    }

    public int WordListUnloadAllLists()
    {
        return wordListUnloadAllLists();
    }

    public int WordListSetFilterMode(int mode)
    {
        return wordListSetFilterMode(mode);
    }

    public int WordListGetFilterMode()
    {
        return wordListGetFilterMode();
    }

    public int WordListLoadFilterList(string path, int storageType)
    {
        return wordListLoadFilterList(path, storageType);
    }

    public int WordListAddWordToFilterListU(IntPtr word)
    {
        return wordListAddWordToFilterListU(word);
    }

    public int WordListRemoveWordFromFilterListU(IntPtr word)
    {
        return wordListRemoveWordFromFilterListU(word);
    }

    public int WordListClearFilterList()
    {
        return wordListClearFilterList();
    }

    public int WordListGetFilterListWordCount()
    {
        return wordListGetFilterListWordCount();
    }

    public IntPtr WordListGetFilterListWordU(int i)
    {
        return wordListGetFilterListWordU(i);
    }

    public int WordGetLetterMask(int wordID, [In, Out] IntPtr letterMaskImage)
    {
        return wordGetLetterMask(wordID, letterMaskImage);
    }

    public int WordGetLetterBoundingBoxes(int wordID, [In, Out] IntPtr letterBoundingBoxes)
    {
        return wordGetLetterBoundingBoxes(wordID, letterBoundingBoxes);
    }

    public int TrackerManagerInitTracker(int trackerType)
    {
        return trackerManagerInitTracker(trackerType);
    }

    public int TrackerManagerDeinitTracker(int trackerType)
    {
        return trackerManagerDeinitTracker(trackerType);
    }

    public int VirtualButtonSetEnabled(IntPtr dataSetPtr,
                                       string trackableName,
                                       string virtualButtonName,
                                       int enabled)
    {
        return virtualButtonSetEnabled(dataSetPtr, trackableName, virtualButtonName, enabled);
    }

    public int VirtualButtonSetSensitivity(IntPtr dataSetPtr,
                                           string trackableName,
                                           string virtualButtonName,
                                           int sensitivity)
    {
        return virtualButtonSetSensitivity(dataSetPtr, trackableName, virtualButtonName, sensitivity);
    }

    public int VirtualButtonSetAreaRectangle(IntPtr dataSetPtr,
                                             string trackableName,
                                             string virtualButtonName,
                                             [In, Out]IntPtr rectData)
    {
        return virtualButtonSetAreaRectangle(dataSetPtr, trackableName, virtualButtonName, rectData);
    }

    public int GetSurfaceOrientation()
    {
        return getSurfaceOrientation();
    }

    public int QcarDeinit()
    {
        return qcarDeinit();
    }

    #endregion



    #region NATIVE_FUNCTIONS

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceInitCamera(int camera);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceDeinitCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceStartCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceStopCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceGetNumVideoModes();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void cameraDeviceGetVideoMode(int idx,
                                    [In, Out]IntPtr videoMode);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSelectVideoMode(int idx);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSetFlashTorchMode(int on);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSetFocusMode(int focusMode);
    
#if UNITY_EDITOR
    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSetCameraConfiguration(int width, int height);
#endif

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarSetFrameFormat(int format, int enabled);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetExists(string relativePath, int storageType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray,
                                                         int trackableDataArrayLength, IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetTrackableName(IntPtr dataSetPtr, int trackableId,
                                                        System.Text.StringBuilder trackableName,
                                                        int nameMaxLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetCreateTrackable(IntPtr dataSetPtr, IntPtr trackableSourcePtr, System.Text.StringBuilder trackableName,
                                                int nameMaxLength, [In, Out] IntPtr trackableData);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetDestroyTrackable(IntPtr dataSetPtr, int trackableId);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetHasReachedTrackableLimit(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetBuilderBuild(string targetName, float sceenSizeWidth);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void imageTargetBuilderStartScan();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void imageTargetBuilderStopScan();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetBuilderGetFrameQuality();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern IntPtr imageTargetBuilderGetTrackableSource();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetCreateVirtualButton(IntPtr dataSetPtr,
                                                             string trackableName, string virtualButtonName,
                                                             [In, Out] IntPtr rectData);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetDestroyVirtualButton(IntPtr dataSetPtr,
        string trackableName, string virtualButtonName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonGetId(IntPtr dataSetPtr,
                                                 string trackableName, string virtualButtonName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetNumVirtualButtons(IntPtr dataSetPtr,
        string trackableName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetVirtualButtons(
        [In, Out]IntPtr virtualButtonDataArray,
        [In, Out]IntPtr rectangleDataArray,
        int virtualButtonDataArrayLength,
        IntPtr dataSetPtr, string trackableName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetVirtualButtonName(
        IntPtr dataSetPtr,
        string trackableName,
        int idx,
        System.Text.StringBuilder vbName,
        int nameMaxLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cylinderTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr dimensions);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cylinderTargetSetSideLength(IntPtr dataSetPtr, string trackableName, float sideLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cylinderTargetSetTopDiameter(IntPtr dataSetPtr, string trackableName, float topDiameter);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cylinderTargetSetBottomDiameter(IntPtr dataSetPtr, string trackableName, float bottomDiameter);

    

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerStart();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void imageTrackerStop();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern IntPtr imageTrackerCreateDataSet();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerDestroyDataSet(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerActivateDataSet(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerDeactivateDataSet(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerStart();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void markerTrackerStop();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerCreateMarker(int id, String trackableName, float size);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerDestroyMarker(int trackableId);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void initFrameState([In, Out] IntPtr frameIndex);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void deinitFrameState([In, Out] IntPtr frameIndex);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int pausedUpdateQCAR();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void updateQCAR([In, Out]IntPtr imageHeaderDataArray,
                                    int imageHeaderArrayLength,
                                    [In, Out]IntPtr frameIndex,
                                    int screenOrientation,
                                    int videoModeIdx);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererRenderVideoBackground(int bindVideoBackground);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererEnd();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarGetBufferSize(int width, int height,
                                    int format);

#if UNITY_EDITOR

    // only used in play mode
    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void qcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally);

#else

    private static void qcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally){}

#endif

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererSetVideoBackgroundCfg(
                                    [In, Out]IntPtr bgCfg);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererGetVideoBackgroundCfg(
                                    [In, Out]IntPtr bgCfg);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererGetVideoBackgroundTextureInfo(
                                    [In, Out]IntPtr texInfo);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int rendererSetVideoBackgroundTextureID(
                            int textureID);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int rendererIsVideoBackgroundTextureInfoAvailable();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int getInitErrorCode();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int isRendererDirty();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarSetHint(int hint, int value);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarRequiresAlpha();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int getProjectionGL(float nearClip, float farClip,
                                    [In, Out]IntPtr projMatrix,
                                    int screenOrientation);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void setUnityVersion(int major, int minor,
                                    int change);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderStartInit(string userKey, string secretKey);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderGetInitState();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderDeinit();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderStartRecognition();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderStop();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void targetFinderSetUIScanlineColor(float r, float g, float b);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void targetFinderSetUIPointColor(float r, float g, float b);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void targetFinderUpdate([In, Out] IntPtr targetFinderState);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderGetResults([In, Out] IntPtr searchResultArray, int searchResultArrayLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int targetFinderEnableTracking(IntPtr searchResult, [In, Out] IntPtr trackableData);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void targetFinderGetImageTargets([In, Out] IntPtr trackableIdArray, int trackableIdArrayLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void targetFinderClearTrackables();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int textTrackerStart();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void textTrackerStop();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int textTrackerSetRegionOfInterest(int detectionLeftTopX, int detectionLeftTopY, int detectionRightBottomX, int detectionRightBottomY,
                                                             int trackingLeftTopX, int trackingLeftTopY, int trackingRightBottomX, int trackingRightBottomY, int upDirection);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int textTrackerGetRegionOfInterest([In, Out] IntPtr detectionROI, [In, Out] IntPtr trackingROI);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListLoadWordList(string path, int storageType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListAddWordsFromFile(string path, int storageType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListAddWordU(IntPtr word);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListRemoveWordU(IntPtr word);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListContainsWordU(IntPtr word);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListUnloadAllLists();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListSetFilterMode(int mode);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListGetFilterMode();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListAddWordToFilterListU(IntPtr word);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListRemoveWordFromFilterListU(IntPtr word);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListClearFilterList();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListLoadFilterList(string path, int storageType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordListGetFilterListWordCount();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern IntPtr wordListGetFilterListWordU(int i);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordGetLetterMask(int wordID, [In, Out] IntPtr letterMaskImage);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int wordGetLetterBoundingBoxes(int wordID, [In, Out] IntPtr letterBoundingBoxes);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int trackerManagerInitTracker(int trackerType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int trackerManagerDeinitTracker(int trackerType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonSetEnabled(IntPtr dataSetPtr,
                                                      string trackableName,
                                                      string virtualButtonName,
                                                      int enabled);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonSetSensitivity(IntPtr dataSetPtr,
                                                        string trackableName,
                                                        string virtualButtonName,
                                                        int sensitivity);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonSetAreaRectangle(IntPtr dataSetPtr,
                                                    string trackableName,
                                                    string virtualButtonName,
                                                    [In, Out]IntPtr rectData);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int getSurfaceOrientation();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarDeinit();

    #endregion // NATIVE_FUNCTIONS
}
