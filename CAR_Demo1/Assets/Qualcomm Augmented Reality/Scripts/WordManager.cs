/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Collections.Generic;


/// <summary>
/// This class is used to manage all word trackables
/// </summary>
public abstract class WordManager
{
    #region PUBLIC_METHODS

    /// <summary>
    /// Returns all currently tracked WordResults
    /// </summary>
    public abstract IEnumerable<WordResult> GetActiveWordResults();

    /// <summary>
    /// Returns all words that have been newly detected in the last frame
    /// </summary>
    public abstract IEnumerable<WordResult> GetNewWords();

    /// <summary>
    /// Returns all words that have been lost in the last frame and won't be tracked anymore
    /// </summary>
    public abstract IEnumerable<Word> GetLostWords();

    /// <summary>
    /// Get the word behaviour that is associated with a currently tracked word
    /// </summary>
    /// <param name="word">trackable</param>
    /// <param name="behaviour">resulting word behaviour, might be null if specified word is not associated to a behaviour</param>
    /// <returns>returns true if word behaviour exists for specified word trackable</returns>
    public abstract bool TryGetWordBehaviour(Word word, out WordBehaviour behaviour);

    /// <summary>
    /// Returns all currently instantiated word behaviours
    /// </summary>
    public abstract IEnumerable<WordBehaviour> GetTrackableBehaviours();


    /// <summary>
    /// Remove a specific word behaviour from list and optionally destroy game object.
    /// This might also remove a template, i.e. no further trackables will be augmented for a specific word or for a template
    /// </summary>
    public abstract void DestroyWordBehaviour(WordBehaviour behaviour, bool destroyGameObject = true);

    #endregion // PUBLIC_METHODS
}

