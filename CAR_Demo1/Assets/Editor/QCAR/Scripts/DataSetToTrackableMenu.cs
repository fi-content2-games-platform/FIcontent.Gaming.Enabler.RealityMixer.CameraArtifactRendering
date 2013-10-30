/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEditor;

public class DataSetToTrackableMenu : Editor
{
    #region PUBLIC_METHODS

    [MenuItem("Vuforia/Apply Data Set Properties", false, 2)]
    public static void ApplyDataSetProperties()
    {
        SceneManager.Instance.ApplyDataSetProperties();
    }

    #endregion // PUBLIC_METHODS
}