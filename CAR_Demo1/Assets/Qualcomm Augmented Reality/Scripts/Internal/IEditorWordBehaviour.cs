/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

/// <summary>
/// Specifies if a word prefab is used as generic template or only for specific words
/// </summary>
public enum WordTemplateMode
{
    Template,
    SpecificWord
}


public interface IEditorWordBehaviour : IEditorTrackableBehaviour
{

    #region EDITOR_INTERFACE

    string SpecificWord { get; }
    void SetSpecificWord(string word);
    WordTemplateMode Mode { get; }
    bool IsTemplateMode { get; }
    bool IsSpecificWordMode { get; }
    void SetMode(WordTemplateMode mode);
    void InitializeWord(Word word);

    #endregion // EDITOR_INTERFACE

}
