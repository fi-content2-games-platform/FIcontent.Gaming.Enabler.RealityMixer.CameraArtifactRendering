/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEditor;
using UnityEngine;


[CustomEditor(typeof (WordBehaviour))]
public class WordEditor : Editor
{
    #region PUBLIC_METHODS

    /// <summary>
    /// Test if more than one word-target are set to template mode or if more than one word-target have the same specific word assigned.
    /// If duplicate word-targets are found, a warning will be displayed to the developer.
    /// </summary>
    /// <returns>true if duplicate word-targets have been found</returns>
    public static bool CheckForDuplicates()
    {
        var duplicatesFound = false;
        var wordBehaviours = (WordBehaviour[])FindObjectsOfType(typeof(WordBehaviour));
        for (int i = 0; i < wordBehaviours.Length; i++)
        {
            IEditorWordBehaviour ewb1 = wordBehaviours[i];
            for (int j = i + 1; j < wordBehaviours.Length; j++)
            {
                IEditorWordBehaviour ewb2 = wordBehaviours[j];
                if (ewb1.IsTemplateMode && ewb2.IsTemplateMode)
                {
                    Debug.LogWarning("Duplicate template word target found. Only one of " +
                                     "the Trackables and its respective Augmentation will be selected for " +
                                     "use at runtime - that selection is indeterminate her.");
                    duplicatesFound = true;
                }
                else if (!ewb1.IsTemplateMode && !ewb2.IsTemplateMode && ewb1.SpecificWord == ewb2.SpecificWord)
                {
                    Debug.LogWarning("Duplicate word target \"" + ewb1.SpecificWord + "\"found. Only one of " +
                                     "the Trackables and its respective Augmentation will be selected for " +
                                     "use at runtime - that selection is indeterminate her.");
                    duplicatesFound = true;
                }
            }
        }
        return duplicatesFound;
    }

    #endregion //PUBLIC_METHODS



    #region UNITY_EDITOR_METHODS

    /// <summary>
    /// Initializes the Word-Behaviour when it is drag-dropped into the scene
    /// </summary>
    public void OnEnable()
    {
        var behaviour = (WordBehaviour) target;

        // We don't want to initialize if this is a prefab.
        if (QCARUtilities.GetPrefabType(behaviour) == PrefabType.Prefab)
        {
            return;
        }

        // Initialize scene manager
        if (!SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }

        IEditorWordBehaviour editorBehaviour = behaviour;
        if (!editorBehaviour.InitializedInEditor && !EditorApplication.isPlaying)
        {
            //default values
            editorBehaviour.SetMode(WordTemplateMode.Template);
            editorBehaviour.SetSpecificWord("Word");

            //define appearance
            UpdateMesh(behaviour);

            editorBehaviour.SetInitializedInEditor(true);

            // Inform Unity that the behaviour properties have changed.
            EditorUtility.SetDirty(behaviour);
        }
    }


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        DrawDefaultInspector();

        var behaviour = (WordBehaviour)target;
        IEditorWordBehaviour editorBehaviour = behaviour;

        if (QCARUtilities.GetPrefabType(behaviour) == PrefabType.Prefab)
        {
            GUILayout.Label("You can't choose a target for a prefab.");
        }
        else
        {
            var templateMode = EditorGUILayout.Toggle("Template", editorBehaviour.IsTemplateMode);
            if (templateMode)
            {
                editorBehaviour.SetMode(WordTemplateMode.Template);
            }
            else
            {
                editorBehaviour.SetMode(WordTemplateMode.SpecificWord);

                var newWord = EditorGUILayout.TextField("Specific Word", editorBehaviour.SpecificWord);
                if (newWord != editorBehaviour.SpecificWord)
                {
                    if(newWord.Length == 0)
                        Debug.LogWarning("Empty string used as word: This trackable and its augmentation will never be selected at runtime.");
                    editorBehaviour.SetSpecificWord(newWord);
                }
            }
        }

        if (GUI.changed)
        {
            UpdateMesh(behaviour);
            EditorUtility.SetDirty(behaviour);
            SceneManager.Instance.SceneUpdated();
        }
    }

    #endregion //UNITY_EDITOR_METHODS



    #region PRIVATE_METHODS

    /// <summary>
    /// Creates a text-mesh and a rectangular mesh. The size of the rectangle depends on the size of the text.
    /// </summary>
    private static void UpdateMesh(WordBehaviour behaviour)
    {
        var gameObject = behaviour.gameObject;
        var editorBehaviour = (IEditorWordBehaviour) behaviour;


        //get existing child-object with name "Text" or create new one
        GameObject childGameObject;
        var child = gameObject.transform.FindChild("Text");
        if (!child)
        {
            childGameObject = new GameObject("Text");
            childGameObject.transform.parent = gameObject.transform;
        }
        else
        {
            childGameObject = child.gameObject;
        }
        childGameObject.transform.localScale = Vector3.one;
        childGameObject.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.right);
        childGameObject.transform.localPosition = Vector3.zero;

        //disable editor-functionality including the transform-handles in the 3d editor
        //childGameObject.hideFlags = HideFlags.NotEditable;

        //get or create text-mesh
        var textMesh = childGameObject.GetComponent<TextMesh>();
        if (!textMesh)
            textMesh = childGameObject.AddComponent<TextMesh>();

        //load default font from resources
        var fontPath = QCARUtilities.GlobalVars.FONT_PATH;
        var font = (Font)AssetDatabase.LoadAssetAtPath(fontPath + "SourceSansPro.ttf", typeof(Font));
        if (font != null)
        {
            textMesh.fontSize = 0;
            textMesh.font = font;
        }
        else
        {
            //fallback when font is not available: use built-in font from unity, won't work on Android and Unity 3.5
            Debug.LogWarning("Standard font for Word-prefabs were not found. You might not be able to use it during runtime.");
            textMesh.font = Resources.Load("Arial", typeof(Font)) as Font;
            textMesh.fontSize = 36;
        }

        //get or create renderer and assign material for rendering texture and set font color to black
        var renderer = childGameObject.GetComponent<MeshRenderer>();
        if (!renderer)
            renderer = childGameObject.AddComponent<MeshRenderer>();

        //copy font-material to get persistent color and shader
        var material = new Material(textMesh.font.material);
        material.color = Color.black;
        material.shader = Shader.Find("Custom/Text3D");
        renderer.sharedMaterial = material;

        //define text for mesh
        textMesh.text = editorBehaviour.IsTemplateMode ? "\"AnyWord\"" : editorBehaviour.SpecificWord;
        //textMesh.transform.hideFlags = HideFlags.NotEditable;

        //set transformation to identity for computing correct axis-aligned bounding box
        var tempRotation = gameObject.transform.localRotation;
        var tempScale = gameObject.transform.localScale;
        var tempPosition = gameObject.transform.localPosition;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localPosition = Vector3.zero;

        //compute axis-aligned bounding box for resizing rectangle
        var bounds = GetBoundsForAxisAlignedTextMesh(textMesh);

        UpdateRectangleMesh(gameObject, bounds);
        UpdateRectangleMaterial(gameObject);

        //set transformation to original value
        gameObject.transform.localRotation = tempRotation;
        gameObject.transform.localScale = tempScale;
        gameObject.transform.localPosition = tempPosition;
    }

    private static Bounds GetBoundsForAxisAlignedTextMesh(TextMesh textMesh)
    {
        //this works well for Unity 3.5.7 but not for Unity 4.0 or higher
        //var bounds = textMesh.renderer.bounds;

        //we compute the bounds ourselves by adjusting the pivot point of the textmesh
        textMesh.anchor = TextAnchor.UpperRight;
        var upperRight = textMesh.renderer.bounds;
        textMesh.anchor = TextAnchor.LowerLeft;
        var lowerLeft = textMesh.renderer.bounds;

        var size = lowerLeft.min - upperRight.min;
        var center = new Vector3(lowerLeft.min.x, lowerLeft.min.z, 0.0f) + 0.5f * size;

        var bounds = new Bounds(center, size);
        return bounds;
    }

    private static void UpdateRectangleMesh(GameObject gameObject, Bounds bounds)
    {
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.hideFlags = HideFlags.NotEditable;

        var vertices = new[]
        {
            new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), new Vector3(bounds.max.x, bounds.max.y, bounds.max.z)
        };

        var normals = new Vector3[vertices.Length];
        var coords = new[]
        {
            new Vector2(0, 0), new Vector2(0, 1),
            new Vector2(1, 0), new Vector2(1, 1)
        };

        var mesh = new Mesh
        {
            vertices = vertices,
            normals = normals,
            uv = coords,
            triangles = new[] {0, 1, 2, 2, 1, 3}
        };

        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        
        // Cleanup assets that have been created temporarily.
        EditorUtility.UnloadUnusedAssets();
    }

    private static void UpdateRectangleMaterial(GameObject gameObject)
    {
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.hideFlags = HideFlags.NotEditable;

        string referenceMaterialPath =
            QCARUtilities.GlobalVars.REFERENCE_MATERIAL_PATH;
        Material referenceMaterial =
            (Material) AssetDatabase.LoadAssetAtPath(referenceMaterialPath,
                                                     typeof (Material));

        Material wordMaterial = new Material(referenceMaterial);
        wordMaterial.name = "Text";
        wordMaterial.shader = Shader.Find("Unlit/Texture");
        wordMaterial.SetColor("_Color", Color.white);
        meshRenderer.sharedMaterial = wordMaterial;
    }

    #endregion //PRIVATE_METHODS
}

