using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
/// <summary>
/// Created By: Joshua Bowers - 03/18/19
/// Last Edited By: Joshua Bowers - 03/18/19
///
/// Purpose:
/// <summary>

//TBD
public class DeckData : ScriptableObject
{
    /*
    ShipUIVariable ui;

    public ShipUIVariable UI { get { return ui; } }

    string deckID;
    public string DeckID { get { return deckID; } }

    [ReadOnly]
    [SerializeField]
    ActiveState state;
    public ActiveState State { get { return state; } }

    Sprite deckSprite;

    public Sprite DeckSprite { get { return deckSprite; } }

    public DeckData(ShipUIVariable newUI, string newDeckID, ActiveState newState, Sprite newDeckSprite)
    {
        ui = newUI;
        deckID = newDeckID;
        state = newState;
        deckSprite = newDeckSprite;
    }

    public void StateChange(ActiveState newState)
    {
        if (state != newState)
        {
            DeckArgs newDeckArgs = new DeckArgs
            {
                DeckID = deckID,
                State = state
            };

            switch (state)
            {
                case ActiveState.Disabled:
                    //gameObject.SetActive(false);
                    break;


                case ActiveState.Enabled:
                    //gameObject.SetActive(true);
                    background.color = ui.Settings.DeselectedCol;

                    //DeckManager.Instance.UpdateActiveDecks(newDeckArgs, this);
                    break;


                case ActiveState.Selected:

                    background.color = ui.Settings.SelectedCol;

                    //DeckManager.Instance.UpdateActiveDecks(newDeckArgs, this);
                    break;
            }
        }
    }
    public DeckVariable Data = new DeckVariable(null, "", ActiveState.Disabled, null, null, null);

    void OnEnable()
    {
        DeckManager.OnDeckSelectionChanged += DeckManager_OnDeckSelectionChanged;
        DeckManager.OnDeckIsolationToggled += DeckManager_OnDeckIsolationToggled;
    }

    void OnDisable()
    {
        DeckManager.OnDeckSelectionChanged -= DeckManager_OnDeckSelectionChanged;
        DeckManager.OnDeckIsolationToggled -= DeckManager_OnDeckIsolationToggled;
    }

    void DeckManager_OnDeckSelectionChanged(DeckArgs args)
    {
        if (args.DeckID != Data.DeckID && args.DeckID != "none")
            return;

        Data.StateChange(args.State);
    }

    void DeckManager_OnDeckIsolationToggled(bool isIsolating)
    {

        if (isIsolating)
        {
            Data.StateChange((Data.State == ActiveState.Selected) ? ActiveState.Selected : ActiveState.Disabled);
        }
        else
        {
            Data.StateChange(ActiveState.Enabled);
        }

    }
    */
}
