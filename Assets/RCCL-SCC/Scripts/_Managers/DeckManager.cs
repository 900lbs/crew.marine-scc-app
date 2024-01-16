// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-02-2019
// ***********************************************************************
// <copyright file="DeckManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

/// <summary>
/// Maintains lists of all decks, active decks and alerting listeners about deck events
/// Implements <see cref="Zenject.IInitializable" />
/// </summary>
/// <seealso cref="Zenject.IInitializable" />
#if SCC_2_5
public class DeckManager : IInitializable, IDynamicShipComponent
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DeckManager"/> class.
	/// </summary>
	public DeckManager()
    {
    }

	/// <summary>
	/// The on deck selection changed
	/// </summary>
	public static Action<DeckArgs> OnDeckSelectionChanged;
	/// <summary>
	/// The on active decks updated
	/// </summary>
	public static Action<int> OnActiveDecksUpdated;
	/// <summary>
	/// The on deck isolation toggled
	/// </summary>
	public static Action<bool> OnDeckIsolationToggled;

	/// <summary>
	/// All decks
	/// </summary>
	public Dictionary<string, Deck> AllDecks;
	/// <summary>
	/// The active decks
	/// </summary>
	public Dictionary<string, Deck> ActiveDecks;

	/// <summary>
	/// Gets the get active decks.
	/// </summary>
	/// <value>The get active decks.</value>
	public string[] GetActiveDecks
    {
        get
        {
            string debugDecks = "";
            string[] decks = new string[ActiveDecks.Count];
            int index = 0;
            foreach (var item in ActiveDecks)
            {
                debugDecks += item.Key + "|";
                decks[index] = item.Key;
                index++;
            }
            return decks;
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Standard c# class so we rely on Zenject's IInitializable instead of a UnityEngine.Start()
	/// </summary>
	public void Initialize()
    {
        ActiveDecks = new Dictionary<string, Deck>();
        AllDecks = new Dictionary<string, Deck>();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Add a new successfully created deck to our dictionary
	/// </summary>
	/// <param name="deckID">I.E 08 or CD etc, this is how everything identifies what GA it's on.</param>
	/// <param name="newDeck">The deck itself.</param>
	public void AddDeck(string deckID, Deck newDeck)
    {
        if (!AllDecks.ContainsKey(deckID))
        {
            AllDecks.Add(deckID, newDeck);
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Remove a deck from the dictionary, will need to be called when reloading a different ship etc.
	/// </summary>
	/// <param name="deckID">I.E 08 or CD etc, this is how everything identifies what GA it's on.</param>
	public void RemoveDeck(string deckID)
    {
        if (AllDecks.ContainsKey(deckID))
        {
            AllDecks.Remove(deckID);
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Handles our DeckSelector announcements.
	/// </summary>
	/// <param name="deckID">The deck identifier.</param>
	/// <param name="state">The state.</param>
	public static void DeckSelectionChanged(string deckID, ActiveState state)
    {
        DeckArgs newDeckArgs = new DeckArgs
        {
            DeckID = deckID,
            State = state
        };

        OnDeckSelectionChanged.Invoke(newDeckArgs);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Handles updating our ActiveDecks dictionary, this way we have an always up-to-date reference for which decks are selected.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <param name="deck">The deck.</param>
	public void UpdateActiveDecks(DeckArgs args, Deck deck)
    {
        switch (args.State)
        {
            case ActiveState.Disabled:
            case ActiveState.Enabled:
                if (ActiveDecks.ContainsKey(args.DeckID))
                {
                    ActiveDecks.Remove(args.DeckID);
                }
                break;

            case ActiveState.Selected:
                if (!ActiveDecks.ContainsKey(args.DeckID))
                {
                    ActiveDecks.Add(args.DeckID, deck);
                }
                break;
        }

        OnActiveDecksUpdated?.Invoke(ActiveDecks.Count);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Return a specific deck's holder.
	/// </summary>
	/// <param name="PropLayer">The property layer.</param>
	/// <param name="deckID">The deck identifier.</param>
	/// <returns>RectTransform.</returns>
	public RectTransform GetHolder(ShipRoomProperties PropLayer, string deckID, UserNameID user)
    {
        //Debug.Log("Get Holder query: " + PropLayer.ToString() + " / " + deckID);
        switch (PropLayer)
        {
            case ShipRoomProperties.None:
                //Debug.Log("Property 'None' was queried, so nothing found.");
                return null;

            case ShipRoomProperties.Annotations:
                try
                {
                    return AllDecks[deckID].AnoHandler.UserAnnotationHolders[user].Rect;
                }
                catch
                {
                    return null;
                }

            case ShipRoomProperties.GAOverlay:
                return AllDecks[deckID].GAHolder;
            default:
                return null;
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Returns every annotation under the queried RoomProperty.
	/// </summary>
	/// <param name="searchProp">The query</param>
	/// <returns>List&lt;GameObject&gt;.</returns>
	public List<GameObject> GetAnnotations(ShipRoomProperties searchProp)
    {
        List<GameObject> annotations = new List<GameObject>();

        switch (searchProp)
        {
            case ShipRoomProperties.None:
                return null;

            case ShipRoomProperties.Annotations:

/*                 foreach (var deck in AllDecks)
                {
                    int deckChildren = deck.Value.AnoHandler.UserAnnotationHolders.Count;
                    for (int i = 0; i < deckChildren; i++)
                    {
                        annotations.Add(deck.Value.AnoHandler.UserAnnotationHolders.gameObject);
                    }
                } */

                break;

            case ShipRoomProperties.GAOverlay:

                foreach (var deck in AllDecks)
                {
                    int deckChildren = deck.Value.GAHolder.childCount;
                    for (int i = 0; i < deckChildren ; i++)
                    {
                        annotations.Add(deck.Value.GAHolder.GetChild(i).gameObject);
                    }
                }

                break;
        }

        return annotations;
    }

    public void OnResetShip()
    {
        AllDecks = new Dictionary<string, Deck>();
        ActiveDecks = new Dictionary<string, Deck>();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif