/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CylinderTargetBehaviour))]
public class CylinderTargetEditor : Editor
{
    #region CONSTANTS

    private const int NUM_PERIMETER_VERTICES = 32;

    private const bool INSIDE_MATERIAL = true;

    #endregion // CONSTANTS



    #region PUBLIC_METHODS

    /// <summary>
    /// Define the ratio between sidelength, top diameter, and bottom diameter.
    /// Geometry and materials are updated according to the new parameters.
    /// </summary>
    public static void UpdateAspectRatio(IEditorCylinderTargetBehaviour ct, ConfigData.CylinderTargetData ctConfig)
    {
        var topRatio = ctConfig.topDiameter/ctConfig.sideLength;
        var bottomRatio = ctConfig.bottomDiameter/ctConfig.sideLength;

        ct.SetAspectRatio(topRatio, bottomRatio);
        UpdateGeometry(ct, 1.0f, topRatio, bottomRatio, ctConfig.hasTopGeometry, ctConfig.hasBottomGeometry);

        //assign materials
        UpdateMaterials(ct, ctConfig.hasBottomGeometry, ctConfig.hasTopGeometry, INSIDE_MATERIAL);
    }

    /// <summary>
    /// Define a new scale for the cylinder target, which corresponds to the sidelength.
    /// If PreserveChildSize is true, all child objects will be scaled inversely in order to keep their original size.
    /// </summary>
    /// <param name="ct">Local scale of this cylinder target will be updated</param>
    /// <param name="scale">Uniform scale of the target, corresponds to the sidelength</param>
    public static void UpdateScale(IEditorCylinderTargetBehaviour ct, float scale)
    {
        var childScaleFactor = ct.transform.localScale.x / scale;

        ct.transform.localScale = new Vector3(scale, scale, scale);

        // Check if 3D content should keep its size or if it should be scaled
        // with the target.
        if (ct.PreserveChildSize)
        {
            foreach (Transform child in ct.transform)
            {
                child.localPosition =
                    new Vector3(child.localPosition.x*childScaleFactor,
                                child.localPosition.y*childScaleFactor,
                                child.localPosition.z*childScaleFactor);

                child.localScale =
                    new Vector3(child.localScale.x*childScaleFactor,
                                child.localScale.y*childScaleFactor,
                                child.localScale.z*childScaleFactor);
            }
        }
    }
    
    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private static void CheckMesh(IEditorCylinderTargetBehaviour editorCtb)
    {
        // when copy-pasting targets between scenes, the mesh and materials of
        // the game objects get lost. This checks for them and re-creates them if they are found missing.
        GameObject itObject = editorCtb.gameObject;

        MeshFilter meshFilter = itObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = itObject.GetComponent<MeshRenderer>();
        if (meshFilter == null || meshFilter.sharedMesh == null ||
            meshRenderer == null || meshRenderer.sharedMaterials.Length == 0 || meshRenderer.sharedMaterials[0] == null)
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(editorCtb.DataSetName);

            ConfigData.CylinderTargetData ctConfig;
            dataSetData.GetCylinderTarget(editorCtb.TrackableName, out ctConfig);

            //geometry has to be updated when a new target has been selected
            UpdateAspectRatio(editorCtb, ctConfig);
        }
    }


    /// <summary>
    /// Updates CylinderTarget. Deletes all parts and recreates them.
    /// Creates a mesh with vertices, normals, and texture coordinates. 
    /// Top and bottom geometry are represented as separate submeshes, 
    /// i.e. resulting mesh contains 1, 2, or 3 submeshes.
    /// </summary>
    private static void UpdateGeometry(IEditorCylinderTargetBehaviour ct,
                                       float sideLength, float topDiameter, float bottomDiameter,
                                       bool hasTopGeometry, bool hasBottomGeometry)
    {
        GameObject gameObject = ct.gameObject;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //disable the editing functionality of meshRenderer in inspector UI
        meshRenderer.hideFlags = HideFlags.NotEditable;


        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        //disable the editing functionality of meshFilter in inspector UI
        meshFilter.hideFlags = HideFlags.NotEditable;

        Mesh cylinderMesh = CylinderMeshFactory.CreateCylinderMesh(sideLength, topDiameter, bottomDiameter,
                                                                   NUM_PERIMETER_VERTICES, hasTopGeometry,
                                                                   hasBottomGeometry, INSIDE_MATERIAL);
        meshFilter.sharedMesh = cylinderMesh;

        //create and attach mask-out material (if not existing yet)
        MaskOutBehaviour script = gameObject.GetComponent<MaskOutBehaviour>();
        if (!script)
        {
            Material maskMaterial =
                (Material) AssetDatabase.LoadAssetAtPath(
                    QCARUtilities.GlobalVars.MASK_MATERIAL_PATH,
                    typeof (Material));

            script = gameObject.AddComponent<MaskOutBehaviour>();
            script.maskMaterial = maskMaterial;
        }
        //disable the editing functionality of the script in inspector UI
        script.hideFlags = HideFlags.NotEditable;


        // Cleanup assets that have been created temporarily.
        EditorUtility.UnloadUnusedAssets();
    }


    /// <summary>
    /// Create and assign materials for cylinder targets. The newly created materials
    /// are based on the default material.
    /// </summary>
    private static void UpdateMaterials(IEditorCylinderTargetBehaviour ct, bool hasBottomGeometry, bool hasTopGeometry, bool insideMaterial)
    {
        // Load reference material.
        var referenceMaterialPath =
            QCARUtilities.GlobalVars.REFERENCE_MATERIAL_PATH;
        var referenceMaterial =
            (Material)AssetDatabase.LoadAssetAtPath(referenceMaterialPath,
                                                    typeof(Material));
        if (referenceMaterial == null)
        {
            Debug.LogError("Could not find reference material at " +
                           referenceMaterialPath +
                           " please reimport Unity package.");
            return;
        }


        // Load texture from texture folder. Textures have per convention the
        // same name as Image Targets + "_scaled" as postfix.
        var pathToTextures = QCARUtilities.GlobalVars.CYLINDER_TARGET_TEXTURES_PATH + ct.DataSetName + "/" + ct.TrackableName;
        var textureFiles = new List<string> { pathToTextures + ".Body_scaled" };
        if (hasBottomGeometry) textureFiles.Add(pathToTextures + ".Bottom_scaled");
        if (hasTopGeometry) textureFiles.Add(pathToTextures + ".Top_scaled");

        var numSubMeshes = textureFiles.Count;
        var materials = new Material[insideMaterial ? numSubMeshes * 2 : numSubMeshes];

        for(var i = 0; i < textureFiles.Count; i++)
        {
            var textureFile = textureFiles[i];
            if (File.Exists(textureFile + ".png"))
                textureFile += ".png";
            else if (File.Exists(textureFile + ".jpg"))
                textureFile += ".jpg";
            else if (File.Exists(textureFile + ".jpeg"))
                textureFile += ".jpeg";
            else
            {
                materials[i] = referenceMaterial;
                if (insideMaterial)
                    materials[i + numSubMeshes] = referenceMaterial;
                continue;
            }

            var targetTexture =
                (Texture2D)AssetDatabase.LoadAssetAtPath(textureFile,
                                                         typeof(Texture2D));

            // We create a new material that is based on the reference material but
            // also contains a texture.
            var materialForTargetTexture = new Material(referenceMaterial);
            if (targetTexture != null)
            {
                materialForTargetTexture.mainTexture = targetTexture;
                materialForTargetTexture.name = targetTexture.name + "Material";
                materialForTargetTexture.mainTextureScale = new Vector2(-1, -1);
            }
            materials[i] = materialForTargetTexture;

            if (insideMaterial)
            {
                var brightMaterial = new Material(materialForTargetTexture);
                brightMaterial.shader = Shader.Find("Custom/BrightTexture");
                materials[i + numSubMeshes] = brightMaterial;
            }
        }

        ct.renderer.sharedMaterials = materials;

        Resources.UnloadUnusedAssets();
    }

    #endregion // PRIVATE_METHODS



    #region UNITY_EDITOR_METHODS

    // Initializes the Cylinder Target when it is drag-dropped into the scene.
    public void OnEnable()
    {
        var ctb = (CylinderTargetBehaviour)target;

        // We don't want to initialize if this is a prefab.
        if (QCARUtilities.GetPrefabType(ctb) == PrefabType.Prefab)
        {
            return;
        }

        // Make sure the scene and config.xml file are synchronized.
        if (!SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }

        IEditorCylinderTargetBehaviour editorCtb = ctb;

        // Only setup target if it has not been set up previously.
        if (!editorCtb.InitializedInEditor && !EditorApplication.isPlaying)
        {
            ConfigData.CylinderTargetData ctConfig;

            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            dataSetData.GetCylinderTarget(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME, out ctConfig);

            editorCtb.SetDataSetPath(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            editorCtb.SetNameForTrackable(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME);

            UpdateAspectRatio(editorCtb, ctConfig);
            UpdateScale(editorCtb, ctConfig.sideLength); 
            editorCtb.SetInitializedInEditor(true);
        }
        else if (!EditorApplication.isPlaying)
        {
            CheckMesh(editorCtb);
        }

        // Cache the current scale of the target:
        editorCtb.SetPreviousScale(ctb.transform.localScale);
    }


    // Lets the user choose a Cylinder Target from a drop down list. Cylinder Target
    // must be defined in the "config.xml" file.
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        DrawDefaultInspector();

        CylinderTargetBehaviour ctb = (CylinderTargetBehaviour)target;
        IEditorCylinderTargetBehaviour editorCtb = ctb;

        if (QCARUtilities.GetPrefabType(ctb) ==
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
                QCARUtilities.GetIndexFromString(editorCtb.DataSetName, dataSetList);

            // If name is not in array we automatically choose default name;
            if (currentDataSetIndex < 0)
                currentDataSetIndex = 0;

            int newDataSetIndex = EditorGUILayout.Popup("Data Set",
                                                        currentDataSetIndex,
                                                        dataSetList);

            string chosenDataSet = dataSetList[newDataSetIndex];

            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(chosenDataSet);

            // Draw list for choosing a Trackable.
            string[] namesList = new string[dataSetData.NumCylinderTargets];
            dataSetData.CopyCylinderTargetNames(namesList, 0);
            int currentTrackableIndex =
                QCARUtilities.GetIndexFromString(editorCtb.TrackableName, namesList);

            // If name is not in array we automatically choose default name;
            if (currentTrackableIndex < 0)
                currentTrackableIndex = 0;

            int newTrackableIndex = EditorGUILayout.Popup("Cylinder Target",
                                                          currentTrackableIndex,
                                                          namesList);

            if (namesList.Length > 0)
            {
                if (newDataSetIndex != currentDataSetIndex || newTrackableIndex != currentTrackableIndex)
                {
                    editorCtb.SetDataSetPath("QCAR/" + dataSetList[newDataSetIndex] + ".xml");

                    editorCtb.SetNameForTrackable(namesList[newTrackableIndex]);


                    ConfigData.CylinderTargetData ctConfig;
                    dataSetData.GetCylinderTarget(ctb.TrackableName, out ctConfig);

                    //geometry has to be updated when a new target has been selected
                    UpdateAspectRatio(editorCtb, ctConfig);
                    UpdateScale(editorCtb, ctConfig.sideLength);
                }
            }

            //Expose editors for setting sideLength and diameterss
            //these values are not really stored, but instead the scale-factor is updated
            var sideLength = EditorGUILayout.FloatField("Side Length", ctb.SideLength);
            if (sideLength != ctb.SideLength)
                ctb.SetSideLength(sideLength);

            float topDiameter = EditorGUILayout.FloatField("Top Diameter ", ctb.TopDiameter);
            if (topDiameter != ctb.TopDiameter)
                ctb.SetTopDiameter(topDiameter);

            float bottomDiameter = EditorGUILayout.FloatField("Bottom Diameter", ctb.BottomDiameter);
            if (bottomDiameter != ctb.BottomDiameter)
                ctb.SetBottomDiameter(bottomDiameter);

            // Draw check box to de-/activate "preserve child size" mode.
            editorCtb.SetPreserveChildSize(EditorGUILayout.Toggle("Preserve child size", editorCtb.PreserveChildSize));
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
            EditorUtility.SetDirty(ctb);
            SceneManager.Instance.SceneUpdated();
        }
    }

    #endregion // UNITY_EDITOR_METHODS
}