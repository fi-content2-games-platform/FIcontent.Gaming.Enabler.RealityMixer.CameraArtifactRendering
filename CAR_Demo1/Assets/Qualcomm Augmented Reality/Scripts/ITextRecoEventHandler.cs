/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


/// <summary>
/// Interface for handling newly detected and lost word results
/// </summary>
public interface ITextRecoEventHandler
{
    /// <summary>
    /// Called when the text reco system has finished initializing
    /// </summary>
    void OnInitialized();

    /// <summary>
    /// Called when a new word has been detected.
    /// </summary>
    /// <param name="word">The newly detected word</param>
    void OnWordDetected(WordResult word);


    /// <summary>
    /// Called when a word is not tracked anymore
    /// </summary>
    void OnWordLost(Word word);

}  

