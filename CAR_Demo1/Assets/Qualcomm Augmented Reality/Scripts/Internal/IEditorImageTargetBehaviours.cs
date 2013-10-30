/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


using UnityEngine;

// The editor interface for MarkerBehaviours
// to be implemented explicitly by behaviours
public interface IEditorImageTargetBehaviour : IEditorDataSetTrackableBehaviour
{
    #region EDITOR_INTERFACE

    float AspectRatio { get; }
    ImageTargetType ImageTargetType { get; }
    bool SetAspectRatio(float aspectRatio);
    bool SetImageTargetType(ImageTargetType imageTargetType);
    Vector2 GetSize();
    void SetWidth(float width);
    void SetHeight(float height);
    void InitializeImageTarget(ImageTarget imageTarget);
    void CreateMissingVirtualButtonBehaviours();
    bool TryGetVirtualButtonBehaviourByID(int id, out VirtualButtonBehaviour virtualButtonBehaviour);
    void AssociateExistingVirtualButtonBehaviour(VirtualButtonBehaviour virtualButtonBehaviour);

    #endregion // EDITOR_INTERFACE
}