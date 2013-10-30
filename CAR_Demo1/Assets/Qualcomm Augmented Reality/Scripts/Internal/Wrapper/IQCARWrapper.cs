/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Runtime.InteropServices;

/// <summary>
/// This interface exposes access to all QCAR methods used by the
/// Unity extension.
/// </summary>
public interface IQCARWrapper
{
    int CameraDeviceInitCamera(int camera);
    int CameraDeviceDeinitCamera();
    int CameraDeviceStartCamera();
    int CameraDeviceStopCamera();
    int CameraDeviceGetNumVideoModes();
    void CameraDeviceGetVideoMode(int idx, [In, Out]IntPtr videoMode);
    int CameraDeviceSelectVideoMode(int idx);
    int CameraDeviceSetFlashTorchMode(int on);
    int CameraDeviceSetFocusMode(int focusMode);
    int CameraDeviceSetCameraConfiguration(int width, int height);
    int QcarSetFrameFormat(int format, int enabled);
    int DataSetExists(string relativePath, int storageType);
    int DataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr);
    int DataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr);

    int DataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray,
                                                   int trackableDataArrayLength, IntPtr dataSetPtr);

    int DataSetGetTrackableName(IntPtr dataSetPtr, int trackableId,
                                                System.Text.StringBuilder trackableName,
                                                int nameMaxLength);

    int DataSetCreateTrackable(IntPtr dataSetPtr, IntPtr trackableSourcePtr, System.Text.StringBuilder trackableName,
                                               int nameMaxLength, [In, Out] IntPtr trackableData);

    int DataSetDestroyTrackable(IntPtr dataSetPtr, int trackableId);
    int DataSetHasReachedTrackableLimit(IntPtr dataSetPtr);
    int ImageTargetBuilderBuild(string targetName, float sceenSizeWidth);
    void ImageTargetBuilderStartScan();
    void ImageTargetBuilderStopScan();
    int ImageTargetBuilderGetFrameQuality();
    IntPtr ImageTargetBuilderGetTrackableSource();

    int ImageTargetCreateVirtualButton(IntPtr dataSetPtr,
                                                       string trackableName, string virtualButtonName,
                                                       [In, Out] IntPtr rectData);

    int ImageTargetDestroyVirtualButton(IntPtr dataSetPtr,
                                                        string trackableName, string virtualButtonName);

    int VirtualButtonGetId(IntPtr dataSetPtr, string trackableName, string virtualButtonName);
    int ImageTargetGetNumVirtualButtons(IntPtr dataSetPtr, string trackableName);

    int ImageTargetGetVirtualButtons([In, Out]IntPtr virtualButtonDataArray,
                                                     [In, Out]IntPtr rectangleDataArray,
                                                     int virtualButtonDataArrayLength,
                                                     IntPtr dataSetPtr, string trackableName);

    int ImageTargetGetVirtualButtonName(IntPtr dataSetPtr,
                                                        string trackableName,
                                                        int idx,
                                                        System.Text.StringBuilder vbName,
                                                        int nameMaxLength);

    int ImageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size);
    int ImageTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size);
    int CylinderTargetGetSize(IntPtr dataSetPtr, string trackableName, [In, Out] IntPtr dimensions);
    int CylinderTargetSetSideLength(IntPtr dataSetPtr, string trackableName, float sideLength);
    int CylinderTargetSetTopDiameter(IntPtr dataSetPtr, string trackableName, float topDiameter);
    int CylinderTargetSetBottomDiameter(IntPtr dataSetPtr, string trackableName, float bottomDiameter); 
    int ImageTrackerStart();
    void ImageTrackerStop();
    IntPtr ImageTrackerCreateDataSet();
    int ImageTrackerDestroyDataSet(IntPtr dataSetPtr);
    int ImageTrackerActivateDataSet(IntPtr dataSetPtr);
    int ImageTrackerDeactivateDataSet(IntPtr dataSetPtr);
    int MarkerTrackerStart();
    void MarkerTrackerStop();
    int MarkerTrackerCreateMarker(int id, String trackableName, float size);
    int MarkerTrackerDestroyMarker(int trackableId);
    void InitFrameState([In, Out] IntPtr frameIndex);
    void DeinitFrameState([In, Out] IntPtr frameIndex);
    int PausedUpdateQCAR();
    void UpdateQCAR([In, Out]IntPtr imageHeaderDataArray,
                                    int imageHeaderArrayLength,
                                    [In, Out]IntPtr frameIndex,
                                    int screenOrientation,
                                    int videoModeIdx);
    void RendererRenderVideoBackground(int bindVideoBackground);
    void RendererEnd();

    int QcarGetBufferSize(int width, int height,
                                          int format);

    void QcarAddCameraFrame(IntPtr pixels, int width, int height, int format, int stride, int frameIdx, int flipHorizontally);
    void RendererSetVideoBackgroundCfg([In, Out]IntPtr bgCfg);
    void RendererGetVideoBackgroundCfg([In, Out]IntPtr bgCfg);
    void RendererGetVideoBackgroundTextureInfo([In, Out]IntPtr texInfo);
    int RendererSetVideoBackgroundTextureID(int textureID);
    int RendererIsVideoBackgroundTextureInfoAvailable();
    int GetInitErrorCode();
    int IsRendererDirty();
    int QcarSetHint(int hint, int value);
    int QcarRequiresAlpha();

    int GetProjectionGL(float nearClip, float farClip,
                                        [In, Out]IntPtr projMatrix,
                                        int screenOrientation);

    void SetUnityVersion(int major, int minor, int change);
    int TargetFinderStartInit(string userKey, string secretKey);
    int TargetFinderGetInitState();
    int TargetFinderDeinit();
    int TargetFinderStartRecognition();
    int TargetFinderStop();
    void TargetFinderSetUIScanlineColor(float r, float g, float b);
    void TargetFinderSetUIPointColor(float r, float g, float b);
    void TargetFinderUpdate([In, Out] IntPtr targetFinderState);
    int TargetFinderGetResults([In, Out] IntPtr searchResultArray, int searchResultArrayLength);
    int TargetFinderEnableTracking(IntPtr searchResult, [In, Out] IntPtr trackableData);
    void TargetFinderGetImageTargets([In, Out] IntPtr trackableIdArray, int trackableIdArrayLength);
    void TargetFinderClearTrackables();
    int TextTrackerStart();
    void TextTrackerStop();

    int TextTrackerSetRegionOfInterest(int detectionLeftTopX, int detectionLeftTopY, int detectionRightBottomX, int detectionRightBottomY,
                                       int trackingLeftTopX, int trackingLeftTopY, int trackingRightBottomX, int trackingRightBottomY, int upDirection);

    void TextTrackerGetRegionOfInterest([In, Out] IntPtr detectionROI, [In, Out] IntPtr trackingROI);
    int WordListLoadWordList(string path, int storageType);
    int WordListAddWordsFromFile(string path, int storagetType);
    int WordListAddWordU(IntPtr word);
    int WordListRemoveWordU(IntPtr word);
    int WordListContainsWordU(IntPtr word);
    int WordListUnloadAllLists();
    int WordListSetFilterMode(int mode);
    int WordListGetFilterMode();
    int WordListLoadFilterList(string path, int storageType);
    int WordListAddWordToFilterListU(IntPtr word);
    int WordListRemoveWordFromFilterListU(IntPtr word);
    int WordListClearFilterList();
    int WordListGetFilterListWordCount();
    IntPtr WordListGetFilterListWordU(int index);
    int WordGetLetterMask(int wordID, [In, Out] IntPtr letterMaskImage);
    int WordGetLetterBoundingBoxes(int wordID, [In, Out] IntPtr letterBoundingBoxes);

    int TrackerManagerInitTracker(int trackerType);
    int TrackerManagerDeinitTracker(int trackerType);

    int VirtualButtonSetEnabled(IntPtr dataSetPtr,
                                                string trackableName,
                                                string virtualButtonName,
                                                int enabled);

    int VirtualButtonSetSensitivity(IntPtr dataSetPtr,
                                                    string trackableName,
                                                    string virtualButtonName,
                                                    int sensitivity);

    int VirtualButtonSetAreaRectangle(IntPtr dataSetPtr,
                                                      string trackableName,
                                                      string virtualButtonName,
                                                      [In, Out]IntPtr rectData);
    int GetSurfaceOrientation();
    int QcarDeinit();
}