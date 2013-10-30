/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QCARBehaviour))]
public class TrackerEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    // OnInspectorGUI exposes public Tracker settings in Inspector
    // WorldCenterMode: Defines how the relative transformation that is returned
    //                  by the QCAR Tracker is applied. Either the camera is
    //                  moved in the scene with respect to a "world center" or
    //                  all the targets are moved with respect to the camera.
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        QCARBehaviour tb = (QCARBehaviour) target;

        DrawDefaultInspector();

        tb.SetWorldCenterMode((QCARBehaviour.WorldCenterMode)
                EditorGUILayout.EnumPopup("World Center Mode",
                tb.WorldCenterModeSetting));

        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        if (tb.WorldCenterModeSetting == QCARBehaviour.WorldCenterMode.SPECIFIC_TARGET)
        {
            var trackable = (TrackableBehaviour)
                            EditorGUILayout.ObjectField("World Center", tb.WorldCenter,
                            typeof (TrackableBehaviour),
                            allowSceneObjects);


            // Word Behaviours cannot be selected as world center
            if (trackable is WordBehaviour)
            {
                trackable = null;
                EditorWindow.focusedWindow.ShowNotification(
                    new GUIContent("Word behaviours cannot be selected as world center."));
            }

            tb.SetWorldCenter(trackable);
        }

        if (GUI.changed)
        {
            // Let Unity know that there is new data for serialization.
            EditorUtility.SetDirty(tb);
        }
    }

    #endregion // UNITY_EDITOR_METHODS
}
