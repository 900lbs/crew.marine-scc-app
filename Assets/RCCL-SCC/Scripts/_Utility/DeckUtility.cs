// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="DeckUtility.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Zenject;

/// <summary>
/// Class DeckUtility.
/// </summary>
#if SCC_2_5
public class DeckUtility
{
	#region Injection Construction
	/// <summary>
	/// The deck manager
	/// </summary>
	readonly DeckManager deckManager;

	/// <summary>
	/// Initializes a new instance of the <see cref="DeckUtility"/> class.
	/// </summary>
	/// <param name="deckMan">The deck man.</param>
	public DeckUtility(DeckManager deckMan)
    {
        deckManager = deckMan;
    }
	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Simple string method that return the deck assignment
	/// based on the text after the last "_"
	/// of the inquirer's name (i.e this_overlay_04a == 04a)
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>System.String.</returns>
	public static string GetDeckIndex(string value)
    {
        int indexOfLastDelimeter = value.LastIndexOf("_") + 1;
        string s = value.Substring(indexOfLastDelimeter);
        return s;
    }
    
/*----------------------------------------------------------------------------------------------------------------------------*/

/*     /// <summary>
    /// Method used to find the correct deck transform.
    /// </summary>
    /// <param name="transList">List of all current decks</param>
    /// <param name="value">Inquirer's deck assignment</param>
    /// <returns></returns>
    public static Deck FindCorrectDeckTransform(string value)
    {
        Deck deckTrans = deckManager.AllDecks[GetDeckIndex(value)];
        Debug.Log(value + "requested, found deck: " + deckTrans.name);
        return deckTrans;
    }  */
}
#endif