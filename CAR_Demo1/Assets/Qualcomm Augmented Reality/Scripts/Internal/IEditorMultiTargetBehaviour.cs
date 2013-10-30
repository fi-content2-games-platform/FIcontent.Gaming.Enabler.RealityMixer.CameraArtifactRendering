/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

// The editor interface for all DataSetTrackableBehaviours
// to be implemented explicitly by behaviours
public interface IEditorMultiTargetBehaviour : IEditorDataSetTrackableBehaviour
{
    #region EDITOR_INTERFACE

    void InitializeMultiTarget(MultiTarget multiTarget);

    #endregion // EDITOR_INTERFACE
}