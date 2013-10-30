/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;


public class TextConfigData
{
    #region NESTED

    /// <summary>
    /// A dictionary contains a binary vwl-file
    /// </summary>
    public struct DictionaryData
    {
        public string BinaryFile;
    }

    /// <summary>
    /// A word list contains a text file, can be used as custom word list or filter list
    /// </summary>
    public struct WordListData
    {
        public string TextFile;
    }
    
    #endregion



    #region PROPERTIES

    /// <summary>
    /// Get the number of available dictionaries
    /// </summary>
    public int NumDictionaries
    {
        get { return mDictionaries.Count; }
    }

    /// <summary>
    /// Get the number of available filter lists / custom word lists
    /// </summary>
    public int NumWordLists
    {
        get { return mWordLists.Count; }
    }

    #endregion // PROPERTIES
    


    #region PRIVATE_MEMBER_VARIABLES

    private Dictionary<string, DictionaryData> mDictionaries;
    private Dictionary<string, WordListData> mWordLists;

    #endregion // PRIVATE_MEMBER_VARIABLES
    


    #region CONSTRUCTION

    // Constructor of TextConfigData class.
    // Creates initializes internal collections.
    public TextConfigData()
    {
        mDictionaries = new Dictionary<string, DictionaryData>();
        mWordLists = new Dictionary<string, WordListData>();
    }
    
    #endregion



    #region PUBLIC_METHODS

    /// <summary>
    /// Define data for dictionary (word list) with given name
    /// </summary>
    public void SetDictionaryData(DictionaryData data, string name)
    {
        mDictionaries[name] = data;
    }

    /// <summary>
    /// Define data for a filter list or custom word list with given name
    /// </summary>
    public void SetWordListData(WordListData data, string name)
    {
        mWordLists[name] = data;
    }

    /// <summary>
    /// Get dictionary with given name
    /// </summary>
    public DictionaryData GetDictionaryData(string name)
    {
        return mDictionaries[name];
    }

    /// <summary>
    /// Get word list with given name
    /// </summary>
    public WordListData GetWordListData(string name)
    {
        return mWordLists[name];
    }
    
    // Copy all Dictionary names into the given string array.
    // The index defines at which location to start copying.
    public void CopyDictionaryNames(string[] arrayToFill, int index)
    {
        mDictionaries.Keys.CopyTo(arrayToFill, index);
    }

    // Copy all Dictionary names and lst-files into the given string arrays.
    // The index defines at which location to start copying.
    public void CopyDictionaryNamesAndFiles(string[] namesArray, string[] filesArray, int index)
    {
        foreach (var dict in mDictionaries)
        {
            namesArray[index] = dict.Key;
            filesArray[index] = dict.Value.BinaryFile;
            index++;
        }
    }
    
    // Copy all FilterList names into the given string array.
    // The index defines at which location to start copying.
    public void CopyWordListNames(string[] arrayToFill, int index)
    {
        mWordLists.Keys.CopyTo(arrayToFill, index);
    }

    // Copy all FilterList names and txt-files into the given string arrays.
    // The index defines at which location to start copying.
    public void CopyWordListNamesAndFiles(string[] namesArray, string[] filesArray, int index)
    {
        foreach (var word in mWordLists)
        {
            namesArray[index] = word.Key;
            filesArray[index] = word.Value.TextFile;
            index++;
        }
    }
    
    #endregion // PUBLIC_METHODS

}

