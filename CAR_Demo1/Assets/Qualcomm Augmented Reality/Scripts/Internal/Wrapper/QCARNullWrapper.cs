/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Runtime.InteropServices;

/// <summary>
/// This is the null implementation for the IQCARWrapper interface.
/// It is used for the free Untiy version that does nto support native plugins.
/// </summary>
public class QCARNullWrapper : IQCARWrapper
{
    #region PUBLIC_METHODS

    public int CameraDeviceInitCamera(int camera)
    {
        return 0;
    }

    public int CameraDeviceDeinitCamera()
    {
        return 0;
    }

    public int CameraDeviceStartCamera()
    {
        return 0;
    }

    public int CameraDeviceStopCamera()
    {
        return 0;
    }

    public int CameraDeviceGetNumVideoModes()
    {
        return 0;
    }

    public void CameraDeviceGetVideoMode(int idx,[In, Out]IntPtr videoMode)
    {
    }

    public int CameraDeviceSelectVideoMode(int idx)
    {
        return 0;
    }

    public int CameraDeviceSetFlashTorchMode(int on)
    {
        return 1;
    }

    public int CameraDeviceSetFocusMode(int focusMode)
    {
        return 1;
    }

    public int CameraDeviceSetCameraConfiguration(int width, int height)
    {
        return 0;
    }

    public int QcarSetFrameFormat(int format, int enabled)
    {
        return 0;
    }

    public int DataSetExists(string relativePath, int storageType)
    {
        return 0;
    }

    public int DataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr)
    {
        return 0;
    }

    public int DataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr)
    {
        return 0;
    }

    public int DataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray,
                                                         int trackableDataArrayLength, IntPtr dataSetPtr)
    {
        return 0;
    }

    public int DataSetGetTrackableName(IntPtr dataSetPtr, int trackableId,
                                                        System.Text.StringBuilder trackableName,
                                                        int nameMaxLength)
    {
        return 0;
    }

    public int DataSetCreateTrackable(IntPtr dataSetPtr, IntPtr trackableSourcePtr, System.Text.StringBuilder trackableName,
                                                int nameMaxLength, [In, Out] IntPtr trackableData)
    {
        return 0;
    }

    public int DataSetDestroyTrackable(IntPtr dataSetPtr, int trackableId)
    {
        return 0;
    }

    public int DataSetHasReachedTrackableLimit(IntPtr dataSetPtr)
    {
        return 0;
    }

    public int ImageTargetBuilderBuild(string targetName, float sceenSizeWidth)
    {
        return 0;
    }

    public void ImageTargetBuilderStartScan()
    {
    }

    public void ImageTargetBuilderStopScan()
    {
    }

    public int ImageTargetBuilderGetFrameQuality()
    {
        return 1;
    }

    public IntPtr ImageTargetBuilderGetTrackableSource()
    {
        return IntPtr.Zero;
    }

    public int ImageTargetCreateVirtualButton(IntPtr dataSetPtr,
                                              string trackableName, string virtualButtonName,
                                              [In, Out] IntPtr rectData)
    {
        return 0;
    }

    public int ImageTargetDestroyVirtualButton(IntPtr dataSetPtr,
                                               string trackableName, string virtualButtonName)
    {
        return 0;
    }

    public int VirtualButtonGetId(IntPtr dataSetPtr, string trackableName, string virtualButtonName)
    {
        return 0;
    }

    public int ImageTargetGetNumVirtualButtons(IntPtr dataSetPtr, string trackableName)
    {
        return 0;
    }

    public int ImageTargetGetVirtualButtons([In, Out]IntPtr virtualButtonDataArray,
                                            [In, Out]IntPtr rectangleDataArray,
                                            int virtualButtonDataArrayLength,
                                            IntPtr dataSetPtr, string trackableName)
    {
        return 0;
    }

    public int ImageTargetGetVirtualButtonName(IntPtr dataSetPtr,
                                               string trackableName,
                                               int idx,
                                               System.Text.StringBuilder vbName,
                                               int nameMaxLength)
    {
        return 0;
    }

    public int ImageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size)
    {
        return 0;
    }

    public int ImageTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size)
    {
        return 0;
    }

    public int CylinderTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr dimensions)
    {
        return 0;
    }

    public int CylinderTargetSetSideLength(IntPtr dataSetPtr, string trackableName, float sideLength)
    {
        return 0;
    }

    public int CylinderTargetSetTopDiameter(IntPtr dataSetPtr, string trackableName, float topDiameter)
    {
        return 0;
    }

    public int CylinderTargetSetBottomDiameter(IntPtr dataSetPtr, string trackableName, float bottomDiameter)
    {
        return 0;
    }

    public int ImageTrackerStart()
    {
        return 0;
    }

    public void ImageTrackerStop()
    {
    }

    public IntPtr ImageTrackerCreateDataSet()
    {
        return IntPtr.Zero;
    }

    public int ImageTrackerDestroyDataSet(IntPtr dataSetPtr)
    {
        return 0;
    }

    public int ImageTrackerActivateDataSet(IntPtr dataSetPtr)
    {
        return 0;
    }

    public int ImageTrackerDeactivateDataSet(IntPtr dataSetPtr)
    {
        return 0;
    }

    public int MarkerTrackerStart()
    {
        return 0;
    }

    public void MarkerTrackerStop()
    {
    }

    public int MarkerTrackerCreateMarker(int id, String trackableName, float size)
    {
        return 0;
    }

    public int MarkerTrackerDestroyMarker(int trackableId)
    {
        return 0;
    }

    public void InitFrameState([In, Out] IntPtr frameIndex)
    {
    }

    public void DeinitFrameState([In, Out] IntPtr frameIndex)
    {
    }

    public int PausedUpdateQCAR()
    {
        return 1;
    }

    public void UpdateQCAR([In, Out]IntPtr imageHeaderDataArray,
                                    int imageHeaderArrayLength,
                                    [In, Out]IntPtr frameIndex,
                                    int screenOrientation,
                                    int videoModeIdx)
    {
    }

    public void RendererRenderVideoBackground(int bindVideoBackground)
    {
    }

    public void RendererEnd()
    {
    }


    public int QcarGetBufferSize(int width, int height,
                                    int format)
    {
        return 0;
    }

    public void QcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally)
    {
    }

    public void RendererSetVideoBackgroundCfg([In, Out]IntPtr bgCfg)
    {
    }

    public void RendererGetVideoBackgroundCfg([In, Out]IntPtr bgCfg)
    {
    }

    public void RendererGetVideoBackgroundTextureInfo([In, Out]IntPtr texInfo)
    {
    }

    public int RendererSetVideoBackgroundTextureID(int textureID)
    {
        return 0;
    }

    public int RendererIsVideoBackgroundTextureInfoAvailable()
    {
        return 0;
    }

    public int GetInitErrorCode()
    {
        return 0;
    }

    public int IsRendererDirty()
    {
        return 0;
    }

    public int QcarSetHint(int hint, int value)
    {
        return 0;
    }

    public int QcarRequiresAlpha()
    {
        return 0;
    }

    public int GetProjectionGL(float nearClip, float farClip,
                                    [In, Out]IntPtr projMatrix,
                                    int screenOrientation)
    {
        return 0;
    }

    public void SetUnityVersion(int major, int minor, int change)
    {
    }

    public int TargetFinderStartInit(string userKey, string secretKey)
    {
        return 0;
    }

    public int TargetFinderGetInitState()
    {
        return 0;
    }

    public int TargetFinderDeinit()
    {
        return 0;
    }

    public int TargetFinderStartRecognition()
    {
        return 0;
    }

    public int TargetFinderStop()
    {
        return 0;
    }

    public void TargetFinderSetUIScanlineColor(float r, float g, float b)
    {
    }

    public void TargetFinderSetUIPointColor(float r, float g, float b)
    {
    }

    public void TargetFinderUpdate([In, Out] IntPtr targetFinderState)
    {
    }

    public int TargetFinderGetResults([In, Out] IntPtr searchResultArray, int searchResultArrayLength)
    {
        return 0;
    }

    public int TargetFinderEnableTracking(IntPtr searchResult, [In, Out] IntPtr trackableData)
    {
        return 0;
    }

    public void TargetFinderGetImageTargets([In, Out] IntPtr trackableIdArray, int trackableIdArrayLength)
    {
    }

    public void TargetFinderClearTrackables()
    {
    }

    public int TextTrackerStart()
    {
        return 0;
    }

    public void TextTrackerStop()
    {
    }

    public int TextTrackerSetRegionOfInterest(int detectionLeftTopX, int detectionLeftTopY, int detectionRightBottomX, int detectionRightBottomY,
                                              int trackingLeftTopX, int trackingLeftTopY, int trackingRightBottomX, int trackingRightBottomY, int upDirection)
    {
        return 0;
    }

    public void TextTrackerGetRegionOfInterest([In, Out] IntPtr detectionROI, [In, Out] IntPtr trackingROI)
    {
        
    }

    public int WordListLoadWordList(string path, int storageType)
    {
        return 0;
    }

    public int WordListAddWordsFromFile(string path, int storagetType)
    {
        return 0;
    }

    public int WordListAddWordU(IntPtr word)
    {
        return 0;
    }

    public int WordListRemoveWordU(IntPtr word)
    {
        return 0;
    }

    public int WordListContainsWordU(IntPtr word)
    {
        return 0;
    }

    public int WordListUnloadAllLists()
    {
        return 0;
    }

    public int WordListSetFilterMode(int mode)
    {
        return 0;
    }

    public int WordListGetFilterMode()
    {
        return 0;
    }

    public int WordListLoadFilterList(string path, int storageType)
    {
        return 0;
    }

    public int WordListAddWordToFilterListU(IntPtr word)
    {
        return 0;
    }

    public int WordListRemoveWordFromFilterListU(IntPtr word)
    {
        return 0;
    }

    public int WordListClearFilterList()
    {
        return 0;
    }

    public int WordListGetFilterListWordCount()
    {
        return 0;
    }

    public IntPtr WordListGetFilterListWordU(int i)
    {
        return new IntPtr(0);
    }

    public int WordGetLetterMask(int wordID, [In, Out] IntPtr letterMaskImage)
    {
        return 0;
    }
    
    public int WordGetLetterBoundingBoxes(int wordID, [In, Out] IntPtr letterBoundingBoxes)
    {
        return 0;
    }

    public int TrackerManagerInitTracker(int trackerType)
    {
        return 0;
    }

    public int TrackerManagerDeinitTracker(int trackerType)
    {
        return 0;
    }

    public int VirtualButtonSetEnabled(IntPtr dataSetPtr,
                                       string trackableName,
                                       string virtualButtonName,
                                       int enabled)
    {
        return 1;
    }

    public int VirtualButtonSetSensitivity(IntPtr dataSetPtr,
                                           string trackableName,
                                           string virtualButtonName,
                                           int sensitivity)
    {
        return 1;
    }

    public int VirtualButtonSetAreaRectangle(IntPtr dataSetPtr,
                                             string trackableName,
                                             string virtualButtonName,
                                             [In, Out]IntPtr rectData)
    {
        return 1;
    }

    public int GetSurfaceOrientation()
    {
        return 0;
    }

    public int QcarDeinit()
    {
        return 0;
    }

    #endregion
}
