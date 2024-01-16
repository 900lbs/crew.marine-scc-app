// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-24-2019
// ***********************************************************************
// <copyright file="Deck.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;
using Zenject;

/// <summary>
/// Main class for each deck created in the scene, holds all necessary information pertaining to decks and their objects.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />

#if SCC_2_5

public class Deck : MonoBehaviour, IDynamicShipComponent
{
    #region Injection Construction
    /// <summary>
    /// The deck manager
    /// </summary>
    DeckManager deckManager;
    /// <summary>
    /// The UI manager
    /// </summary>
    UIManager uiManager;

    SignalBus _signalBus;


    /// <summary>
    /// Constructs the specified deck man.
    /// </summary>
    /// <param name="deckMan">The deck man.</param>
    /// <param name="uiMan">The UI man.</param>
    [Inject]
    public void Construct(DeckManager deckMan, UIManager uiMan, SignalBus signal)
    {
        deckManager = deckMan;
        uiManager = uiMan;
        _signalBus = signal;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The deck identifier
    /// </summary>
    public string DeckID;

    /// <summary>
    /// The state
    /// </summary>
    [ReadOnly]
    [SerializeField]
    ActiveState state;
    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>The state.</value>
    public ActiveState State
    {
        get
        {
            return state;
        }

        set
        {
            if (state != value)
            {
                state = value;
                StateChange();
            }
        }
    }
    /// <summary>
    /// The deck image
    /// </summary>
    public Image DeckImage;

    /// <summary>
    /// The background
    /// </summary>
    public Image Background;

    /// <summary>
    /// The deck text
    /// </summary>
    public TextMeshProUGUI DeckText;
    /// <summary>
    /// The ga holder
    /// </summary>
    public RectTransform GAHolder;
    /// <summary>
    /// The ano holder
    /// </summary>
    public DeckAnnotationHandler AnoHandler;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnEnable()
    {
        _signalBus.Subscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    void OnDisable()
    {
        _signalBus.TryUnsubscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }


    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [destroy].
    /// </summary>
    private void OnDestroy()
    {
        DeckManager.OnDeckSelectionChanged -= DeckManager_OnDeckSelectionChanged;
        DeckManager.OnDeckIsolationToggled -= DeckManager_OnDeckIsolationToggled;
        UIManager.OnDestroyShipAssets -= UIManager_OnDestroyShipAssets;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// UIs the manager on destroy ship assets.
    /// </summary>
    void UIManager_OnDestroyShipAssets()
    {
        Destroy(gameObject);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Decks the manager on deck selection changed.
    /// </summary>
    /// <param name="args">The arguments.</param>
    void DeckManager_OnDeckSelectionChanged(DeckArgs args)
    {
        if (args.DeckID != DeckID && args.DeckID != "none")
            return;

        State = args.State;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Decks the manager on deck isolation toggled.
    /// </summary>
    /// <param name="isIsolating">if set to <c>true</c> [is isolating].</param>
    void DeckManager_OnDeckIsolationToggled(bool isIsolating)
    {

        if (isIsolating)
        {
            State = (State == ActiveState.Selected) ? ActiveState.Selected : ActiveState.Disabled;
        }
        else
        {
            State = ActiveState.Enabled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// States the change.
    /// </summary>
    void StateChange()
    {
        DeckArgs newDeckArgs = new DeckArgs
        {
            DeckID = DeckID,
            State = state
        };

        switch (State)
        {
            case ActiveState.Disabled:
                gameObject.SetActive(false);
                break;


            case ActiveState.Enabled:

                gameObject.SetActive(true);
                Background.color = uiManager.UI.Settings.DeselectedCol;
                deckManager.UpdateActiveDecks(newDeckArgs, this);

                break;


            case ActiveState.Selected:

                Background.color = uiManager.UI.Settings.SelectedCol;
                deckManager.UpdateActiveDecks(newDeckArgs, this);

                break;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes the deck.
    /// </summary>
    /// <param name="deckID">The deck identifier.</param>
    /// <param name="deck">The deck.</param>
    /// <returns>Task.</returns>
    public async Task InitializeDeck(string deckID, Sprite deck)
    {
        try
        {
            //Since this event triggers OnEnable and OnDisable, stick the listener here since we only have one scene to ever worry about
            DeckManager.OnDeckSelectionChanged += DeckManager_OnDeckSelectionChanged;
            DeckManager.OnDeckIsolationToggled += DeckManager_OnDeckIsolationToggled;
            UIManager.OnDestroyShipAssets += UIManager_OnDestroyShipAssets;

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            transform.localScale = Vector3.one;
            await new WaitForUpdate();

            await AnoHandler.SpawnHolders();
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the local holder.
    /// </summary>
    /// <param name="props">The props.</param>
    /// <returns>RectTransform.</returns>
    public async Task<RectTransform> GetLocalHolder(ShipRoomProperties props, UserNameID user)
    {
        switch (props)
        {
            case ShipRoomProperties.Annotations:
                return AnoHandler.UserAnnotationHolders[user].Rect;

            case ShipRoomProperties.GAOverlay:
                return GAHolder;

            default:
                return GetComponent<RectTransform>();
        }
    }

    public void OnResetShip()
    {
        Destroy(gameObject);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public class Factory : PlaceholderFactory<UnityEngine.Object, string, Sprite, Task<Deck>>
    { }
}
#endif