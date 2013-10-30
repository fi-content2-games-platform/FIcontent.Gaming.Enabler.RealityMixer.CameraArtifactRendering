/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


/// <summary>
/// Types of filter modes
/// </summary>
public enum WordFilterMode
{
    /// <summary>
    /// Word filtering is disabled
    /// </summary>
    NONE = 0,
    /// <summary>
    /// Prevent specific words from being detected
    /// </summary>
    BLACK_LIST = 1,
    /// <summary>
    /// Enable selected words only to be detected
    /// </summary>
    WHITE_LIST = 2
}

/// <summary>
/// This class represents a list of words that can be detected and tracked.
/// Custom words and filter lists can be defined for a word list.
/// 
/// The WordList represents the set of detectable Words. This list is
/// loaded from a binary data file using LoadWordListFile.
/// The application may choose to add a small set of additional custom words to
/// the WordList using the APIs below.
/// The filter list allows an application to specify a subset of Words
/// from the WordList that will be detected and tracked. 
/// Note that the TextTracker needs to be stopped prior to making modifications
/// to the WordList.
/// </summary>
public abstract class WordList
{
    #region PUBLIC_METHODS

    /// <summary>
    /// Loads the word list from a binary file at the default 
    /// "Assets/StreamingAssets" directory.
    /// </summary>
    public abstract bool LoadWordListFile(string relativePath);

    /// <summary>
    /// Loads the word list from a binary file at the specified path and storage
    /// location
    /// </summary>
    public abstract bool LoadWordListFile(string relativePath, DataSet.StorageType storageType);

    /// <summary>
    /// Loads a set of custom words from a plain text file at the default 
    /// "Assets/StreamingAssets" directory.
    /// </summary>
    public abstract int AddWordsFromFile(string relativePath);

    /// <summary>
    /// Loads a set of custom words from a plain text file at the specified 
    /// path and storage location
    /// The word list is extended with the custom words in the plain text file.
    /// No more than 10.000 custom words can be added to the word list. Each word
    /// must be between 2-45 characters in length. Returns the
    /// number of loaded custom words. The text file shall be encoded in UTF-8.
    /// </summary>
    /// <returns>Number of words that have been added</returns>
    public abstract int AddWordsFromFile(string path, DataSet.StorageType storageType);

    /// <summary>
    /// Add a single custom word to the word list
    /// Use ContainsWord to check if the word is already in the word list prior
    /// to calling this.
    /// </summary>
    public abstract bool AddWord(string word);

    /// <summary>
    /// Remove a custom word from the word list
    /// </summary>
    public abstract bool RemoveWord(string word);

    /// <summary>
    /// Returns true if the given word is present in the WordList 
    /// </summary>
    public abstract bool ContainsWord(string word);

    /// <summary>
    /// Clears the word list as well as the filter list and releases resources.
    /// </summary>
    public abstract bool UnloadAllLists();

    /// <summary>
    /// Returns the filter mode
    /// </summary>
    public abstract WordFilterMode GetFilterMode();

    /// <summary>
    /// Sets the mode for the filter list
    /// The filter list allows an application to specify a subset of Words
    /// from the word list that will be detected and tracked. It can do this
    /// in two modes of operation. In black list mode, any word in the filter
    /// list will be prevented from being detected. In the white list mode,
    /// only words in the the filter list can be detected.
    /// </summary>
    public abstract bool SetFilterMode(WordFilterMode mode);

    /// <summary>
    /// Loads the filter list from a plain text file at the default 
    /// "Assets/StreamingAssets" directory.
    /// </summary>
    public abstract bool LoadFilterListFile(string relativePath);

    /// <summary>
    /// Loads the filter list from a plain text file at the specified 
    /// path and storage location
    /// </summary>
    public abstract bool LoadFilterListFile(string path, DataSet.StorageType storageType);

    /// <summary>
    /// Add a single word to the filter list 
    /// </summary>
    public abstract bool AddWordToFilterList(string word);

    /// <summary>
    /// Remove a word from the filter list 
    /// </summary>
    public abstract bool RemoveWordFromFilterList(string word);

    /// <summary>
    /// Clear the filter list
    /// </summary>
    public abstract bool ClearFilterList();

    /// <summary>
    /// Query the number of words in the filter list
    /// </summary>
    public abstract int GetFilterListWordCount();

    /// <summary>
    /// Returns the ith element in the filter list
    /// </summary>
    public abstract string GetFilterListWord(int index);

    #endregion // PUBLIC_METHODS
}

