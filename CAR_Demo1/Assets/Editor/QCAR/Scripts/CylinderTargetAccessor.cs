/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEditor;

public class CylinderTargetAccessor : TrackableAccessor
{
    #region CONSTRUCTION

    // The one CylinderTargetBehaviour instance this accessor belongs to is set in
    // the constructor.
    public CylinderTargetAccessor(CylinderTargetBehaviour target)
    {
        mTarget = target;
    }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // This method updates the respective Trackable properties (e.g. size)
    // with data set data.
    public override void ApplyDataSetProperties()
    {
        // Prefabs should not be editable
        if (QCARUtilities.GetPrefabType(mTarget) == PrefabType.Prefab)
        {
            return;
        }
        
        IEditorCylinderTargetBehaviour ctb = (CylinderTargetBehaviour)mTarget;

        ConfigData.CylinderTargetData ctConfig;
        if (TrackableInDataSet(ctb.TrackableName, ctb.DataSetName))
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(ctb.DataSetName);
            dataSetData.GetCylinderTarget(ctb.TrackableName, out ctConfig);
        }
        else
        {
            // If the Trackable has been removed from the data set we reset it to default.
            ConfigData dataSetData =
                ConfigDataManager.Instance.GetConfigData(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            dataSetData.GetCylinderTarget(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME, out ctConfig);
            ctb.SetDataSetPath(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            ctb.SetNameForTrackable(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME);
        }

        CylinderTargetEditor.UpdateScale(ctb, ctConfig.sideLength);
    }


    // This method updates the respective Trackable appearance (e.g.
    // aspect ratio and texture) with data set data.
    public override void ApplyDataSetAppearance()
    {
        // Prefabs should not be editable
        if (QCARUtilities.GetPrefabType(mTarget) == PrefabType.Prefab)
        {
            return;
        }

        IEditorCylinderTargetBehaviour ctb = (CylinderTargetBehaviour)mTarget;

        ConfigData.CylinderTargetData ctConfig;
        if (TrackableInDataSet(ctb.TrackableName, ctb.DataSetName))
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(ctb.DataSetName);
            dataSetData.GetCylinderTarget(ctb.TrackableName, out ctConfig);
        }
        else
        {
            // If the Trackable has been removed from the data set we reset it to default.
            ConfigData dataSetData =
                ConfigDataManager.Instance.GetConfigData(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            dataSetData.GetCylinderTarget(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME, out ctConfig);
            ctb.SetDataSetPath(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            ctb.SetNameForTrackable(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME);
        }

        //update geometry based on properties from dataset
        CylinderTargetEditor.UpdateAspectRatio(ctb, ctConfig);
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private bool TrackableInDataSet(string trackableName, string dataSetName)
    {
        if (ConfigDataManager.Instance.ConfigDataExists(dataSetName))
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(dataSetName);
            return dataSetData.CylinderTargetExists(trackableName);
        }
        return false;
    }

    #endregion // PRIVATE_METHODS
}
