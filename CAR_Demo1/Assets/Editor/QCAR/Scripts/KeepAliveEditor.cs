/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (KeepAliveBehaviour))]
public class KeepAliveEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        DrawDefaultInspector();

        KeepAliveBehaviour kab = (KeepAliveBehaviour)target;

        EditorGUILayout.HelpBox("By keeping the objects checked below alive, they can be rused across multiple scenes.\n" +
                                "Keeping the ARCamera alive will result in keeping all loaded datasets available when a new scene is loaded, incluing user defined targets.\n" +
                                "You can also keep Trackable prefabs like Image Targets alive, as well as advanced prefabs like e.g. CloudRecognition.", MessageType.Info);

        if (Application.isPlaying)
            GUI.enabled = false;

        kab.KeepARCameraAlive = EditorGUILayout.Toggle("Keep AR Camera Alive", kab.KeepARCameraAlive);
        if (kab.KeepARCameraAlive)
        {
            // Keep all trackables when changing to another scene
            kab.KeepTrackableBehavioursAlive = EditorGUILayout.Toggle("Keep Trackable Prefabs Alive", kab.KeepTrackableBehavioursAlive);

            // keep the text reco behaviour when changing to another scene
            kab.KeepTextRecoBehaviourAlive = EditorGUILayout.Toggle("Keep Text Reco Prefab Alive", kab.KeepTextRecoBehaviourAlive);

            // keep the udt behaviour when changing to another scene
            kab.KeepUDTBuildingBehaviourAlive = EditorGUILayout.Toggle("Keep UDT building Prefab Alive", kab.KeepUDTBuildingBehaviourAlive);

            // keep the cloud reco behaviour when changing to another scene
            kab.KeepCloudRecoBehaviourAlive = EditorGUILayout.Toggle("Keep Cloud Reco Prefab Alive", kab.KeepCloudRecoBehaviourAlive);
        }
        else
        {
            kab.KeepTrackableBehavioursAlive = false;
            kab.KeepTextRecoBehaviourAlive = false;
            kab.KeepUDTBuildingBehaviourAlive = false;
            kab.KeepCloudRecoBehaviourAlive = false;
        }

        GUI.enabled = true;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(kab);
        }
    }

    #endregion // UNITY_EDITOR_METHODS
}