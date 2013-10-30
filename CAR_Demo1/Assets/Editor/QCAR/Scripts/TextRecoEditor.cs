/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextRecoBehaviour))]
public class TextRecoEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    public void OnEnable()
    {
        var trb = (TextRecoBehaviour)target;

        // We don't want to initialize if this is a prefab.
        if (QCARUtilities.GetPrefabType(trb) == PrefabType.Prefab)
        {
            return;
        }

        // Make sure the scene and config.xml file are synchronized.
        if (!SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }
    }


    // Draws a custom UI for the cloud reco behaviour inspector
    public override void OnInspectorGUI()
    {
        TextRecoBehaviour crb = (TextRecoBehaviour)target;
        IEditorTextRecoBehaviour etrb = (IEditorTextRecoBehaviour) target;
        EditorGUILayout.HelpBox("The list of words the TextTracker can detect and track.\n" +
            "The word list is loaded from a binary file and can be extended by a list of custom words.", MessageType.Info);


        //select word list from all lst-files. if none is found, show link to AR-page
        var textConfig = ConfigDataManager.Instance.GetTextConfigData();

        if (textConfig.NumDictionaries > 1)
        {
            var wordListNames = new string[textConfig.NumDictionaries];
            var wordListFiles = new string[textConfig.NumDictionaries];
            textConfig.CopyDictionaryNamesAndFiles(wordListNames, wordListFiles, 0);

            var currentIdx = QCARUtilities.GetIndexFromString(etrb.WordListFile, wordListFiles);
            if (currentIdx < 0) currentIdx = 0;
            var idx = EditorGUILayout.Popup("Word List", currentIdx, wordListNames);
            etrb.WordListFile = wordListFiles[idx];

            if (idx == 0)
                GUI.enabled = false;
        }
        else
        {
            if (GUILayout.Button("No word list available. \nPlease copy it from the TextRecognition sample app. \n" +
                                 "Press here to go to the download page for sample apps!"))
            {
                SceneManager.Instance.GoToSampleAppPage();
            }

            GUI.enabled = false;
        }

        var numTxtFiles = textConfig.NumWordLists;
        var txtFileNames = new string[numTxtFiles];
        var txtFiles = new string[numTxtFiles];
        textConfig.CopyWordListNamesAndFiles(txtFileNames, txtFiles, 0);

        //select custom word list file from one of the found text-files
        var customWordListIndex = QCARUtilities.GetIndexFromString(etrb.CustomWordListFile, txtFiles);
        if (customWordListIndex < 0) customWordListIndex = 0;
        var newCustomWordListIndex = EditorGUILayout.Popup("Additional Word File", customWordListIndex, txtFileNames);
        if (newCustomWordListIndex != customWordListIndex)
        {
            //test if file is valid when other than default word list is selected
            //for better convenience let the developer select an invalid file, but show an error
            if (newCustomWordListIndex != 0)
                TestValidityOfWordListFile(txtFiles[newCustomWordListIndex]);

            etrb.CustomWordListFile = txtFiles[newCustomWordListIndex];
        }

        //define additional custom words
        EditorGUILayout.LabelField("Additional Words:");
        EditorGUILayout.HelpBox("Write one word per line. Open compound words can be specified using whitespaces.", MessageType.None);
        etrb.AdditionalCustomWords = EditorGUILayout.TextArea(etrb.AdditionalCustomWords);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("The filter list allows to specify subset of words that will be detected and tracked.", MessageType.Info);

        //Edit filter mode by selecting one of the enum-values
        var filterModeNames = new []{"NONE", "BLACK_LIST", "WHITE_LIST"};
        var filterModeValues = new List<WordFilterMode> { WordFilterMode.NONE, WordFilterMode.BLACK_LIST, WordFilterMode.WHITE_LIST };
        var filterModeIndex = filterModeValues.IndexOf(etrb.FilterMode);
        filterModeIndex = EditorGUILayout.Popup("Filter Mode", filterModeIndex, filterModeNames);
        etrb.FilterMode = filterModeValues[filterModeIndex];

        //do not show editor for filter words when FilterMode is set to NONE
        if (etrb.FilterMode != WordFilterMode.NONE)
        {
            //Select filter list file from one of the detected text-files
            var filterListIndex = QCARUtilities.GetIndexFromString(etrb.FilterListFile, txtFiles);
            if (filterListIndex < 0) filterListIndex = 0;

            var newFilterListIndex = EditorGUILayout.Popup("Filter List File", filterListIndex, txtFileNames);
            if (newFilterListIndex != filterListIndex)
            {
                //test if file is valid when other than default word list is selected
                //for better convenience let the developer select an invalid file, but show an error
                if (newFilterListIndex != 0)
                    TestValidityOfWordListFile(txtFiles[newFilterListIndex]);

                etrb.FilterListFile = txtFiles[newFilterListIndex];
            }

            //Define additional filter words
            EditorGUILayout.LabelField("Additional Filter Words:");
            EditorGUILayout.HelpBox("Write one word per line. Open compound words can be specified using whitespaces.", MessageType.None);
            etrb.AdditionalFilterWords = EditorGUILayout.TextArea(etrb.AdditionalFilterWords);
        }

        EditorGUILayout.HelpBox("It is possible to use Word Prefabs to define augmentations for detected words. " +
                                "Each Word Prefab can be instantiated up to a maximum number.", MessageType.Info);

        //Define how word prefabs are used at runtime
        var duplicatePrefabs = EditorGUILayout.Toggle("Use Word Prefabs", etrb.WordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE);
        if (duplicatePrefabs)
        {
            etrb.WordPrefabCreationMode = WordPrefabCreationMode.DUPLICATE;
            etrb.MaximumWordInstances = EditorGUILayout.IntField("Max Simultaneous Words", etrb.MaximumWordInstances);
        }
        else
        {
            etrb.WordPrefabCreationMode = WordPrefabCreationMode.NONE;
        }

        GUI.enabled = true;

        if (GUI.changed)
            EditorUtility.SetDirty(crb);
    }

    // Renders a label to visualize the TextRecoBehaviour
    public void OnSceneGUI()
    {
        TextRecoBehaviour crb = (TextRecoBehaviour)target;
        GUIStyle guiStyle = new GUIStyle { alignment = TextAnchor.LowerRight, fontSize = 18, normal = { textColor = Color.white } };
        Handles.Label(crb.transform.position, "Text\nRecognition", guiStyle);
    }

    #endregion // UNITY_EDITOR_METHODS



    #region PRIVATE_METHODS

    /// <summary>
    /// Test if file is a valid word list file: checks for plaint text file and for maximum line (word) length
    /// </summary>
    private static void TestValidityOfWordListFile(string file)
    {
        const int minWordLength = 2;
        const int maxWordLength = 45;
        const int maxWords = 10000;

        const string header = "UTF-8\n\n";

        var absFile = "Assets/StreamingAssets/" + file;

        var canBeFixed = false;
        var isValid = true;
        var stream = File.OpenText(absFile);
        var bs = stream.BaseStream;
        //test header
        for (var i = 0; i < 7; i++)
        {
            var c = bs.ReadByte();
            if (c != header[i])
            {
                if (i == 0 && c == 0xEF && bs.ReadByte() == 0xBB && bs.ReadByte() == 0xBF)
                {
                    Debug.LogWarning("Only UTF-8 documents without BOM are supported.");
                    isValid = false;
                    canBeFixed = true;
                    break;
                }

                if (i == 5 && c == '\r')
                {
                    Debug.LogWarning("Only UTF-8 documents without CARRIAGE RETURN are supported.");
                    isValid = false;
                    canBeFixed = true;
                    break;
                }

                Debug.LogWarning("Not a valid UTF-8 encoded file. It needs to start with the header \"UTF-8\" followed by an empty line.");
                isValid = false;
                break;
            }
        }

        var wordCount = 0;
        while (isValid && !stream.EndOfStream)
        {
            var line = stream.ReadLine();
            if (line.Length == 0)
            {
                Debug.LogWarning("There is an empty line in your word list.");
                isValid = false;
            }
            else if (line.Length < minWordLength)
            {
                Debug.LogWarning("The word " + line + " is too short for Text-Reco.");
                isValid = false;
            }
            else if (line.Length > maxWordLength)
            {
                Debug.LogWarning("The word " + line + " is too long for Text-Reco.");
                isValid = false;
            }
            else
            {
                foreach (var c in line)
                {
                    if (!((c >= 'A' && c <= 'Z') //uppercase letters A-Z
                       || (c >= 'a' && c <= 'z') //lowercase letters a-z
                       || c == 45 || c == 39 || c == ' ')) //hyphen, apostrophe, or space
                    {
                        Debug.LogWarning("The word " + line + " is not supported because of character " + c);
                        isValid = false;
                    }
                }
            }
            wordCount++;
            if (wordCount > maxWords)
            {
                Debug.LogWarning("The maximum number of words is " + maxWords + ".");
                isValid = false;
            }
        }

        stream.Close();

        if (!isValid && canBeFixed)
        {
            ConvertEOLAndEncodingIfUserWantsTo(absFile);
        }
    }


    private static void ConvertEOLAndEncodingIfUserWantsTo(string file)
    {
        if (EditorUtility.DisplayDialog("Wrong Line Endings",
                                        "Would you like to automatically convert the line endings " +
                                        "and/or remove the BOM of the selected word list file?",
                                        "Yes", "No"))
        {
            var text = File.ReadAllText(file).Replace("\r\n", "\n");
            File.WriteAllText(file, text, new UTF8Encoding(false));
        }
    }

    #endregion //PRIVATE_METHODS
}