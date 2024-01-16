using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Zenject;
public class DeckFactory : IFactory<UnityEngine.Object, string, Sprite, Task<Deck>>
{
    readonly DiContainer container;
    readonly Deck.Factory deckFactory;

    [Inject]
    public DeckFactory(DiContainer contain, Deck.Factory deckFact)
    {
        container = contain;
    }
    [Inject(Id = "CurrentShip")]
    readonly ShipVariable CurrentShip;
    public async Task<Deck> Create(UnityEngine.Object prefab, string deckID, Sprite deck)
    {
        //Debug.Log("<color=yellow>Spinning up DeckFactory</color>");
        Deck newDeck = container.InstantiatePrefabForComponent<Deck>(prefab);

        await new WaitForUpdate();
        try
        {
            if (newDeck.Background == null)
                newDeck.Background = newDeck.GetComponent<Image>();


            newDeck.State = ActiveState.Enabled;


            if (newDeck.DeckText == null)
            {
                newDeck.DeckText = newDeck.GetComponentInChildren<TextMeshProUGUI>();
            }

            newDeck.DeckID = deckID;
            newDeck.name = "DeckMap_" + deckID;
            newDeck.DeckText.text = deckID;
            newDeck.DeckImage.sprite = deck;

            return newDeck;
        }
        catch (System.Exception)
        {
            return null;

        }
    }
}
