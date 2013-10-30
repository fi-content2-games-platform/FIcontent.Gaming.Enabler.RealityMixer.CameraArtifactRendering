/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;

/// <summary>
/// This class serves both as an augmentation definition for a Word in the editor
/// as well as a tracked Word result at runtime
/// </summary>
public class WordBehaviour : TrackableBehaviour, IEditorWordBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES

    [SerializeField] 
    [HideInInspector] 
    private WordTemplateMode mMode;

    [SerializeField]
    [HideInInspector]
    private string mSpecificWord;

    private Word mWord;

    #endregion //PRIVATE_MEMBER_VARIABLES



    #region PROTECTED_METHODS

    /// <summary>
    /// This method disconnects the TrackableBehaviour from its associated trackable.
    /// Use it only if you know what you are doing - e.g. when you want to destroy a trackable, but reuse the TrackableBehaviour.
    /// </summary>
    protected override void InternalUnregisterTrackable()
    {
        mTrackable = mWord = null;
    }

    #endregion //PROTECTED_METHODS



    #region PROPERTIES
    /// <summary>
    /// The word that this word behaviour augments
    /// </summary>
    public Word Word
    {
        get { return mWord; }
    }

    #endregion // PROPERTIES



    #region EDITOR_INTERFACE_IMPLEMENTATION

    string IEditorWordBehaviour.SpecificWord
    {
        get { return mSpecificWord; }
    }

    void IEditorWordBehaviour.SetSpecificWord(string word)
    {
        mSpecificWord = word;
    }

    WordTemplateMode IEditorWordBehaviour.Mode
    {
        get { return mMode; }
    }

    bool IEditorWordBehaviour.IsTemplateMode
    {
        get { return mMode == WordTemplateMode.Template; }
    }

    bool IEditorWordBehaviour.IsSpecificWordMode
    {
        get { return mMode == WordTemplateMode.SpecificWord; }
    }

    void IEditorWordBehaviour.SetMode(WordTemplateMode mode)
    {
        mMode = mode;
    }
    
    void IEditorWordBehaviour.InitializeWord(Word word)
    {
        mTrackable = mWord = word;

        //set the size of the target to the value returned from text reco
        //apply only uniform scaling according to height
        var size = word.Size;

        //compute size of underlying geometry to compute the correct scale factor
        var geometrySize = Vector3.one;
        var meshFilter = GetComponent<MeshFilter>();
        if(meshFilter != null)
            geometrySize = meshFilter.sharedMesh.bounds.size;

        var scale = size.y / geometrySize.z;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    #endregion //EDITOR_INTERFACE_IMPLEMENTATION
}