/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

// The editor interface for all DataSetTrackableBehaviours
// to be implemented explicitly by behaviours
public interface IEditorCylinderTargetBehaviour : IEditorDataSetTrackableBehaviour
{
    #region EDITOR_INTERFACE

    void InitializeCylinderTarget(CylinderTarget cylinderTarget);

    void SetAspectRatio(float topRatio, float bottomRatio);

    #endregion // EDITOR_INTERFACE
}