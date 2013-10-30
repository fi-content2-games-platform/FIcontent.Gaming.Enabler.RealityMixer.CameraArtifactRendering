/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;

/// <summary>
/// A utility behaviour to disable rendering of a word behaviour at run time.
/// </summary>
public class TurnOffWordBehaviour : MonoBehaviour
{

    #region UNITY_MONOBEHAVIOUR_METHODS

    void Awake()
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            // We remove the renderer at run-time only, but keep it for
            // visualization when running in the editor
            // We keep the MeshFilter for retreiving the size of the Word-prefab
            MeshRenderer targetMeshRenderer = this.GetComponent<MeshRenderer>();
            Destroy(targetMeshRenderer);
            //The child object for visualizing text is removed at runtime
            var text = transform.FindChild("Text");
            if(text != null)
                Destroy(text.gameObject);
        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

}