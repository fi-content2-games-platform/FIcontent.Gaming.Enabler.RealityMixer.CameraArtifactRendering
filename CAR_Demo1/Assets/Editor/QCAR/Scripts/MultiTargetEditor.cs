/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiTargetBehaviour))]
public class MultiTargetEditor : Editor
{
    #region PUBLIC_METHODS

    // Updates MultiTarget parts with the values stored in the "prtConfigs"
    // array. Deletes all parts and recreates them.
    public static void UpdateParts(IEditorMultiTargetBehaviour mt,
                                   ConfigData.MultiTargetPartData[] prtConfigs)
    {
        Transform childTargets = mt.transform.Find("ChildTargets");

        if (childTargets != null)
        {
            Object.DestroyImmediate(childTargets.gameObject);
        }

        GameObject newObject = new GameObject();
        newObject.name = "ChildTargets";
        newObject.transform.parent = mt.transform;
        newObject.hideFlags = HideFlags.NotEditable;

        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localRotation = Quaternion.identity;
        newObject.transform.localScale = Vector3.one;

        childTargets = newObject.transform;

        Material maskMaterial =
            (Material)AssetDatabase.LoadAssetAtPath(
                QCARUtilities.GlobalVars.MASK_MATERIAL_PATH,
                typeof(Material));

        ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(mt.DataSetName);
        if (dataSetData == null)
        {
            Debug.LogError("Could not update Multi Target parts. A data set with the given name does not exist.");
        }
        else
        {
            var planeMesh = new Mesh
                {
                    vertices =
                        new[]
                            {
                                new Vector3(-0.5f, 0f, -0.5f), new Vector3(-0.5f, 0f, 0.5f),
                                new Vector3(0.5f, 0f, -0.5f), new Vector3(0.5f, 0f, 0.5f)
                            },
                    uv = new[]
                        {
                            new Vector2(1, 1), new Vector2(1, 0),
                            new Vector2(0, 1), new Vector2(0, 0)
                        },
                    normals = new Vector3[4],
                    triangles = new[] {0, 1, 2, 2, 1, 3}
                };
            planeMesh.RecalculateNormals();

            int numParts = prtConfigs.Length;
            for (int i = 0; i < numParts; ++i)
            {
                if (!dataSetData.ImageTargetExists(prtConfigs[i].name)/* &&
                    prtConfigs[i].name != QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME*/)
                {
                    Debug.LogError("No Image Target named " + prtConfigs[i].name);
                    return;
                }

                ConfigData.ImageTargetData itConfig;
                dataSetData.GetImageTarget(prtConfigs[i].name, out itConfig);

                Vector2 size = itConfig.size;
                Vector3 scale = new Vector3(size.x, 1, size.y);

                GameObject plane = new GameObject(prtConfigs[i].name);
                plane.AddComponent<MeshRenderer>();
                var filter = plane.AddComponent<MeshFilter>();
                filter.sharedMesh = planeMesh;
                plane.transform.parent = childTargets.transform;

                plane.transform.localPosition = prtConfigs[i].translation;
                plane.transform.localRotation = prtConfigs[i].rotation;
                plane.transform.localScale = scale;

                UpdateMaterial(mt, plane);

                plane.hideFlags = HideFlags.NotEditable;

                MaskOutBehaviour script =
                    (MaskOutBehaviour)plane.AddComponent(typeof(MaskOutBehaviour));
                script.maskMaterial = maskMaterial;
            }
        }
    }

    #endregion // PUBLIC_METHODS



    #region UNITY_EDITOR_METHODS

    // Initializes the Multi Target when it is drag-dropped into the scene.
    public void OnEnable()
    {
        MultiTargetBehaviour mtb = (MultiTargetBehaviour)target;

        // We don't want to initialize if this is a prefab.
        if (QCARUtilities.GetPrefabType(mtb) == PrefabType.Prefab)
        {
            return;
        }

        // Make sure the scene and config.xml file are synchronized.
        if (!SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }

        IEditorMultiTargetBehaviour editorMtb = mtb;

        // Only setup target if it has not been set up previously.
        if (!editorMtb.InitializedInEditor && !EditorApplication.isPlaying)
        {
            ConfigData.MultiTargetData mtConfig;

            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            dataSetData.GetMultiTarget(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME, out mtConfig);

            editorMtb.SetDataSetPath(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            editorMtb.SetNameForTrackable(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME);

            List<ConfigData.MultiTargetPartData> prtConfigs = mtConfig.parts;

            UpdateParts(editorMtb, prtConfigs.ToArray());
            editorMtb.SetInitializedInEditor(true);
        }
        else if (!EditorApplication.isPlaying)
        {
            CheckMesh(mtb);
        }

        // Cache the current scale of the target:
        editorMtb.SetPreviousScale(mtb.transform.localScale);
    }


    // Checks if the transformation of the Multi Target has been changed by
    // Unity transform-handles in scene view.
    // This is also called when user changes attributes in Inspector.
    public void OnSceneGUI()
    {
        TrackableBehaviour trackableBehaviour = (TrackableBehaviour)target;

        if (trackableBehaviour.transform.localScale.x != 1.0f ||
            trackableBehaviour.transform.localScale.y != 1.0f ||
            trackableBehaviour.transform.localScale.z != 1.0f)
        {
            Debug.LogError("You cannot scale a Multi target in the editor. " +
                           "Please edit the config.xml file to scale this " +
                           "target.");
            trackableBehaviour.transform.localScale =
                new Vector3(1.0f, 1.0f, 1.0f);
        }
    }


    // Lets the user choose a Multi Target from a drop down list. Multi Target
    // must be defined in the "config.xml" file.
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        DrawDefaultInspector();

        MultiTargetBehaviour mtb = (MultiTargetBehaviour)target;
        IEditorMultiTargetBehaviour editorMtb = mtb;

        if (QCARUtilities.GetPrefabType(mtb) ==
            PrefabType.Prefab)
        {
            GUILayout.Label("You can't choose a target for a prefab.");
        }
        else if (ConfigDataManager.Instance.NumConfigDataObjects > 1)
        {
            // Draw list for choosing a data set.
            string[] dataSetList = new string[ConfigDataManager.Instance.NumConfigDataObjects];
            ConfigDataManager.Instance.GetConfigDataNames(dataSetList);
            int currentDataSetIndex =
                QCARUtilities.GetIndexFromString(editorMtb.DataSetName, dataSetList);

            // If name is not in array we automatically choose default name;
            if (currentDataSetIndex < 0)
                currentDataSetIndex = 0;

            int newDataSetIndex = EditorGUILayout.Popup("Data Set",
                                                        currentDataSetIndex,
                                                        dataSetList);

            string chosenDataSet = dataSetList[newDataSetIndex];

            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(chosenDataSet);

            // Draw list for choosing a Trackable.
            string[] namesList = new string[dataSetData.NumMultiTargets];
            dataSetData.CopyMultiTargetNames(namesList, 0);
            int currentTrackableIndex =
                QCARUtilities.GetIndexFromString(editorMtb.TrackableName, namesList);

            // If name is not in array we automatically choose default name;
            if (currentTrackableIndex < 0)
                currentTrackableIndex = 0;
            
            int newTrackableIndex = EditorGUILayout.Popup("Multi Target",
                                                          currentTrackableIndex,
                                                          namesList);

            if (namesList.Length > 0)
            {
                if (newDataSetIndex != currentDataSetIndex || newTrackableIndex != currentTrackableIndex)
                {
                    editorMtb.SetDataSetPath("QCAR/" + dataSetList[newDataSetIndex] + ".xml");

                    editorMtb.SetNameForTrackable(namesList[newTrackableIndex]);
                }
            }
        }
        else
        {
            if (GUILayout.Button("No targets defined. Press here for target " +
                                 "creation!"))
            {
                SceneManager.Instance.GoToTargetManagerPage();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mtb);

            // If name has changed we apply the correct values from the config
            // file.
            TrackableAccessor accessor = AccessorFactory.Create(mtb);
            if (accessor != null)
                accessor.ApplyDataSetProperties();

            SceneManager.Instance.SceneUpdated();
        }
    }

    #endregion // UNITY_EDITOR_METHODS



    #region PRIVATE_METHODS

    private static void CheckMesh(MultiTargetBehaviour mtb)
    {
        // when copy-pasting targets between scenes, the mesh and materials of
        // the game objects get lost. This checks for them and re-creates them if they are found missing.
        bool updateGeometry = false;

        Transform childTargets = mtb.transform.Find("ChildTargets");
        if (childTargets == null)
        {
            updateGeometry = true;
        }
        else
        {
            for (int i = 0; i < childTargets.childCount; i++)
            {
                Transform childTarget = childTargets.GetChild(i);
                MeshFilter meshFilter = childTarget.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = childTarget.GetComponent<MeshRenderer>();
                if (meshFilter == null || meshFilter.sharedMesh == null ||
                    meshRenderer == null || meshRenderer.sharedMaterials.Length == 0 ||
                    meshRenderer.sharedMaterials[0] == null)
                    updateGeometry = true;
            }
        }

        if (updateGeometry)
        {
            TrackableAccessor accessor = AccessorFactory.Create(mtb);
            if (accessor != null)
                accessor.ApplyDataSetProperties();
        }
    }

    private static void UpdateMaterial(IEditorMultiTargetBehaviour mt, GameObject go)
    {
        // Load reference material
        string referenceMaterialPath =
            QCARUtilities.GlobalVars.REFERENCE_MATERIAL_PATH;
        Material referenceMaterial =
            (Material)AssetDatabase.LoadAssetAtPath(referenceMaterialPath,
                                                    typeof(Material));
        if (referenceMaterial == null)
        {
            Debug.LogError("Could not find reference material at " +
                           referenceMaterialPath +
                           " please reimport Unity package.");
            return;
        }

        // Load texture from texture folder
        string pathToTexture = QCARUtilities.GlobalVars.TARGET_TEXTURES_PATH;

        string textureFile = pathToTexture + mt.DataSetName + "/" + go.name + "_scaled";

        if (File.Exists(textureFile + ".png"))
            textureFile += ".png";
        else if (File.Exists(textureFile + ".jpg"))
            textureFile += ".jpg";

        Texture2D targetTexture =
            (Texture2D)AssetDatabase.LoadAssetAtPath(textureFile,
                                                     typeof(Texture2D));
        if (targetTexture == null)
        {
            // If the texture is null we simply assign a default material
            go.renderer.sharedMaterial = referenceMaterial;
            return;
        }

        // We create a new material based on the reference material
        Material materialForTargetTexture = new Material(referenceMaterial);
        materialForTargetTexture.mainTexture = targetTexture;
        materialForTargetTexture.name = targetTexture.name + "Material";
        materialForTargetTexture.mainTextureScale = new Vector2(-1, -1);

        go.renderer.sharedMaterial = materialForTargetTexture;

        // Cleanup assets that have been created temporarily.
        EditorUtility.UnloadUnusedAssets();
    }

    #endregion // PRIVATE_METHODS
}