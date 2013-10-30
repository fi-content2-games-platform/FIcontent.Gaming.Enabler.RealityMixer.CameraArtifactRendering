/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

// The editor interface for all DataSetTrackableBehaviours
// to be implemented explicitly by behaviours

using UnityEngine;

public interface IEditorDataSetTrackableBehaviour:IEditorTrackableBehaviour
{
    #region EDITOR_INTERFACE

    string DataSetName { get; }
    string DataSetPath { get; }
    bool SetDataSetPath(string dataSetPath);

    #endregion // EDITOR_INTERFACE
}