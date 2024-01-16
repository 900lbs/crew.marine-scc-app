// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="DeckSelector.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;
using Zenject;
/// <summary>
/// Main button class for selecting <see cref="Deck"/> objects based on <see cref="DeckID"/>.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
#if SCC_2_5
[RequireComponent(typeof(Button))]
public class DeckSelector : UI_Button, IDynamicShipComponent
{
    #region Injection Construction
    /// <summary>
    /// The UI manager
    /// </summary>
    UIManager uiManager;
    /// <summary>
    /// The XML factory
    /// </summary>
	XMLWriterDynamic.Factory xmlWriterFactory;

    CancellationTokenSource c;

    /// <summary>
    /// Constructs the specified UI man.
    /// </summary>
    /// <param name="uiMan">The UI man.</param>
    /// <param name="writerFact">The writer fact.</param>
    [Inject]
    public void Construct(UIManager uiMan,
    XMLWriterDynamic.Factory writerFact,
    CancellationTokenSource cts)
    {
        uiManager = uiMan;
        xmlWriterFactory = writerFact;
        c = cts;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The deck identifier
    /// </summary>
    public string DeckID;
    /// <summary>
    /// The deck text
    /// </summary>
    public TextMeshProUGUI DeckText;
    /// <summary>
    /// The dashes
    /// </summary>
    public TextMeshProUGUI Dashes;

    /// <summary>
    /// The color profiles
    /// </summary>
    public ColorProfile[] ColorProfiles;

    /// <summary>
    /// The sel brackets
    /// </summary>
    public Image selBrackets;

    XMLWriterDynamic xmlWriter;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    private void OnValidate()
    {
        int colorProfilesCount = ColorProfiles.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            ColorProfiles[i].OnValidate();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [enable].
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        UIManager.OnDestroyShipAssets += UIManager_OnDestroyShipAssets;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnEnable()
    {
        _signalBus.Subscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnDisable()
    {
        _signalBus.TryUnsubscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [disable].
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        UIManager.OnDestroyShipAssets -= UIManager_OnDestroyShipAssets;
        DeckManager.OnDeckSelectionChanged -= DeckManager_OnDeckSelectionChanged;
        DeckManager.OnDeckIsolationToggled -= DeckManager_OnDeckIsolationToggled;
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
    /// Initializes the selector.
    /// </summary>
    /// <param name="deck">The deck.</param>
    /// <returns>Task.</returns>
    public async Task InitializeSelector(string deck)
    {
        if (!c.IsCancellationRequested)
        {

            DeckID = deck;

            try
            {
                DeckText.text = deck;
            }
            catch
            {
                if (DeckText == null)
                    Debug.LogError("Could not find Deck Text.", this);

                if (string.IsNullOrEmpty(deck))
                    Debug.LogError("Deck input was empty on initialize.", this);
            }

            xmlWriter = xmlWriterFactory.Create(gameObject, XMLType.Isolate);

            State = ActiveState.Enabled;

            DeckManager.OnDeckSelectionChanged += DeckManager_OnDeckSelectionChanged;
            DeckManager.OnDeckIsolationToggled += DeckManager_OnDeckIsolationToggled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        DeckManager.DeckSelectionChanged(DeckID, (State == ActiveState.Selected) ? ActiveState.Enabled : ActiveState.Selected);

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

        if (State == ActiveState.Selected && args.State == ActiveState.Disabled)
            return;

        State = args.State;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Decks the manager on deck isolation toggled.
    /// </summary>
    /// <param name="isIsolating">if set to <c>true</c> [is isolating].</param>
    async void DeckManager_OnDeckIsolationToggled(bool isIsolating)
    {

        if (isIsolating)
        {
            if (State == ActiveState.Selected)
            {
                State = ActiveState.Selected;
                await xmlWriter.AttemptCustomSave(DeckID);
                await xmlWriter.Save();
            }
            else
            {
                State = ActiveState.Disabled;
            }
        }
        else
        {
            State = ActiveState.Enabled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange()
    {
        int colorProfilesCount = ColorProfiles.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            ColorProfiles[i].StateChange(State);
        }
    }

    public void OnResetShip()
    {
        Destroy(this.gameObject);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    //Placeholder class for the generic factory, refer to DeckSelectorFactory.cs for initialization logic.
    /// <summary>
    /// Class Factory.
    /// Implements the <see cref="Zenject.PlaceholderFactory{UnityEngine.Object, System.String, DeckSelector}" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{UnityEngine.Object, System.String, DeckSelector}" />
    public new class Factory : PlaceholderFactory<Object, string, DeckSelector>
    { }

}
#endif