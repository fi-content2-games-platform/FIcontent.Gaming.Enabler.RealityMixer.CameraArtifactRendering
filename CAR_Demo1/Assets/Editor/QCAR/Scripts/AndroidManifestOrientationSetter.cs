/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This class works around a change in Unity 4.2 that does no longer apply orientation settings selected in the Unity editor 
/// </summary>
[InitializeOnLoad]
public class AndroidManifestOrientationSetter
{
    private const string ACTIVITY_NODE_NAME = "activity";
    private const string ACTIVITY_NAME_ATTR = "android:name";
    private const string ACTIVITY_ORIENTATION_ATTR = "android:screenOrientation";

    private static bool mInitialized = false;
    private static UIOrientation mUIOrientation;

    private static readonly Dictionary<UIOrientation, string> OrientationMapping = new Dictionary<UIOrientation, string>
        {
            {UIOrientation.AutoRotation, "sensor"},
            {UIOrientation.LandscapeLeft, "landscape"},
            {UIOrientation.LandscapeRight, "reverseLandscape"},
            {UIOrientation.Portrait, "portrait"},
            {UIOrientation.PortraitUpsideDown, "reversePortrait"},
        };

    private static readonly List<string> ActivitiesToModify = new List<string> 
    {
        "com.qualcomm.QCARUnityPlayer.QCARPlayerProxyActivity",
        "com.qualcomm.QCARUnityPlayer.QCARPlayerActivity",
        "com.qualcomm.QCARUnityPlayer.QCARPlayerNativeActivity",
        "com.unity3d.player.VideoPlayer"
    };

    static AndroidManifestOrientationSetter()
    {
        EditorApplication.update += CheckAndApplyOrientationSettings;
    }

    static void CheckAndApplyOrientationSettings()
    {
        if (!mInitialized || PlayerSettings.defaultInterfaceOrientation != mUIOrientation)
        {
            mInitialized = true;
            mUIOrientation = PlayerSettings.defaultInterfaceOrientation;
            SetOrientationInAndroidManifest(OrientationMapping[mUIOrientation]);
        }
    }

    static void SetOrientationInAndroidManifest(string orientationValue)
    {
        string relXmlPath = "Plugins/Android/AndroidManifest.xml";
        try
        {
            string filePath = Path.Combine(Application.dataPath, relXmlPath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            bool xmlDocModified = false;

            // check all activities in the manifest:
            XmlNodeList activityNodes = xmlDoc.GetElementsByTagName(ACTIVITY_NODE_NAME);
            foreach (XmlNode activityNode in activityNodes)
            {
                if (activityNode.Attributes != null)
                {
                    XmlAttribute activityName = activityNode.Attributes[ACTIVITY_NAME_ATTR];
                    if (activityName != null)
                    {
                        if (ActivitiesToModify.Contains(activityName.Value))
                        {
                            // this is a Vuforia related activity, set the orientation as selected in the editor
                            XmlAttribute activityOrientation = activityNode.Attributes[ACTIVITY_ORIENTATION_ATTR];
                            if (activityOrientation != null)
                            {
                                if (activityOrientation.Value != orientationValue)
                                {
                                    activityOrientation.Value = orientationValue;
                                    xmlDocModified = true;
                                }
                            }
                            else
                            {
                                activityOrientation = xmlDoc.CreateAttribute(":"+ACTIVITY_ORIENTATION_ATTR); // ":" added since string preceding first colon is interpreted as the xml prefix
                                activityOrientation.Value = orientationValue;
                                activityNode.Attributes.Append(activityOrientation);
                                xmlDocModified = true;
                            }
                        }
                    }
                }
            }

            if (xmlDocModified)
                xmlDoc.Save(filePath);
        }
        catch (Exception e)
        {
            string errorMsg = "Exception occurred when trying to parse web cam profile file: " + e.Message;
            Debug.LogError(errorMsg);
            Debug.LogError("The selected orientation could not be set for the Vuforia activities in " + relXmlPath + "\n" +
                           "Make sure to set the required orientation manually.");
        }
    }
}