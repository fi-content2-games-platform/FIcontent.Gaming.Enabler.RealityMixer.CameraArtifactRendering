/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// This class represents a list of words that can be detected and tracked.
/// Custom words and filter lists can be defined for a word list.
/// </summary>
class WordListImpl : WordList
{
    #region PUBLIC_METHODS


    /// <summary>
    /// Loads the word list from a binary file at the default 
    /// "Assets/StreamingAssets" directory.
    /// </summary>
    public override bool LoadWordListFile(string relativePath)
    {
        return LoadWordListFile(relativePath, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    /// <summary>
    /// Loads the word list from a binary file at the specified path and storage
    /// location
    /// </summary>
    public override bool LoadWordListFile(string path, DataSet.StorageType storageType)
    {
        if (storageType == DataSet.StorageType.STORAGE_APPRESOURCE && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return QCARWrapper.Instance.WordListLoadWordList(path, (int)storageType) == 1;
    }

    /// <summary>
    /// Loads a set of custom words from a plain text file at the default 
    /// "Assets/StreamingAssets" directory.
    /// </summary>
    public override int AddWordsFromFile(string relativePath)
    {
        return AddWordsFromFile(relativePath, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    /// <summary>
    /// Loads a set of custom words from a plain text file at the specified 
    /// path and storage location
    /// The word list is extended with the custom words in the plain text file.
    /// No more than 10.000 custom words can be added to the word list. Each word
    /// must be between 2-45 characters in length. Returns the
    /// number of loaded custom words. The text file shall be encoded in UTF-8.
    /// </summary>
    /// <returns>Number of words that have been added</returns>
    public override int AddWordsFromFile(string path, DataSet.StorageType storageType)
    {
        if (storageType == DataSet.StorageType.STORAGE_APPRESOURCE && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return QCARWrapper.Instance.WordListAddWordsFromFile(path, (int)storageType);
    }

    /// <summary>
    /// Add a single custom word to the word list
    /// Use ContainsWord to check if the word is already in the word list prior
    /// to calling this.
    /// </summary>
    public override bool AddWord(string word)
    {
        var ptr = Marshal.StringToHGlobalUni(word);
        var result = QCARWrapper.Instance.WordListAddWordU(ptr) == 1;

        Marshal.FreeHGlobal(ptr);
        return result;
    }

    /// <summary>
    /// Remove a custom word from the word list
    /// </summary>
    public override bool RemoveWord(string word)
    {
        var ptr = Marshal.StringToHGlobalUni(word);
        var result = QCARWrapper.Instance.WordListRemoveWordU(ptr) == 1;

        Marshal.FreeHGlobal(ptr);
        return result;
    }

    /// <summary>
    /// Returns true if the given word is present in the WordList 
    /// </summary>
    public override bool ContainsWord(string word)
    {
        var ptr = Marshal.StringToHGlobalUni(word);
        var result = QCARWrapper.Instance.WordListContainsWordU(ptr) == 1;

        Marshal.FreeHGlobal(ptr);
        return result;
    }

    /// <summary>
    /// Clears the word list as well as the filter list and releases resources.
    /// </summary>
    public override bool UnloadAllLists()
    {
        return QCARWrapper.Instance.WordListUnloadAllLists() == 1;
    }

    /// <summary>
    /// Returns the filter mode
    /// </summary>
    public override WordFilterMode GetFilterMode()
    {
        return (WordFilterMode)QCARWrapper.Instance.WordListGetFilterMode();
    }

    /// <summary>
    /// Sets the mode for the filter list
    /// The filter list allows an application to specify a subset of Words
    /// from the word list that will be detected and tracked. It can do this
    /// in two modes of operation. In black list mode, any word in the filter
    /// list will be prevented from being detected. In the white list mode,
    /// only words in the the filter list can be detected.
    /// </summary>
    public override bool SetFilterMode(WordFilterMode mode)
    {
        return QCARWrapper.Instance.WordListSetFilterMode((int)mode) == 1;
    }

    /// <summary>
    /// Loads the filter list from a plain text file at the default 
    /// "Assets/StreamingAssets" directory.
    /// </summary>
    public override bool LoadFilterListFile(string relativePath)
    {
        return LoadFilterListFile(relativePath, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    /// <summary>
    /// Loads the filter list from a plain text file at the specified 
    /// path and storage location
    /// </summary>
    public override bool LoadFilterListFile(string path, DataSet.StorageType storageType)
    {
        if (storageType == DataSet.StorageType.STORAGE_APPRESOURCE && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return QCARWrapper.Instance.WordListLoadFilterList(path, (int)storageType) == 1;
    }

    /// <summary>
    /// Add a single word to the filter list 
    /// </summary>
    public override bool AddWordToFilterList(string word)
    {
        var ptr = Marshal.StringToHGlobalUni(word);
        var result = QCARWrapper.Instance.WordListAddWordToFilterListU(ptr) == 1;

        Marshal.FreeHGlobal(ptr);
        return result;
    }

    /// <summary>
    /// Remove a word from the filter list 
    /// </summary>
    public override bool RemoveWordFromFilterList(string word)
    {
        var ptr = Marshal.StringToHGlobalUni(word);
        var result = QCARWrapper.Instance.WordListRemoveWordFromFilterListU(ptr) == 1;

        Marshal.FreeHGlobal(ptr);
        return result;
    }

    /// <summary>
    /// Clear the filter list
    /// </summary>
    public override bool ClearFilterList()
    {
        return QCARWrapper.Instance.WordListClearFilterList() == 1;
    }

    /// <summary>
    /// Query the number of words in the filter list
    /// </summary>
    public override int GetFilterListWordCount()
    {
        return QCARWrapper.Instance.WordListGetFilterListWordCount();
    }

    /// <summary>
    /// Returns the ith element in the filter list
    /// </summary>
    public override string GetFilterListWord(int index)
    {
        var ptr = QCARWrapper.Instance.WordListGetFilterListWordU(index);
        var word = Marshal.PtrToStringUni(ptr);
        return word;
    }

    #endregion
}