/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


public interface IEditorTextRecoBehaviour
{
    #region EDITOR_INTERFACE

    string WordListFile { get; set; }
    string CustomWordListFile { get; set; }
    string AdditionalCustomWords { get; set; }
    WordFilterMode FilterMode { get; set; }
    string FilterListFile { get; set; }
    string AdditionalFilterWords { get; set; }
    WordPrefabCreationMode WordPrefabCreationMode { get; set; }
    int MaximumWordInstances { get; set; }

    #endregion //EDITOR_INTERFACE
}

