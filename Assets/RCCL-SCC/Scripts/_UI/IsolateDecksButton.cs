// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="IsolateDecksButton.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;
using Zenject;

/// <summary>
/// Handles isolating all current <see cref="ActiveState.Selected"/> <see cref="Deck"/>
/// Implements <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />

#if SCC_2_5
public class IsolateDecksButton : UI_Button
{
    #region Injection Construction
    /// <summary>
    /// The annotation manager
    /// </summary>
    AnnotationManager annotationManager;
    /// <summary>
    /// The zoom out
    /// </summary>
    ZoomOut zoomOut;

    DeckManager deckManager;

    XMLWriterDynamic.Factory xmlWriterFactory;

    /// <summary>
    /// Constructs the specified ano man.
    /// </summary>
    /// <param name="anoMan">The ano man.</param>
    /// <param name="zoom">The zoom.</param>
    [Inject]
    public void Construct(AnnotationManager anoMan, ZoomOut zoom, DeckManager deckMan, XMLWriterDynamic.Factory xmlWriterFact)
    {
        annotationManager = anoMan;
        zoomOut = zoom;
        deckManager = deckMan;
        xmlWriterFactory = xmlWriterFact;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The is isolating
    /// </summary>
    public bool IsIsolating;

    XMLWriterDynamic xmlWriter;

    /// <summary>
    /// The color change objects
    /// </summary>
    public ColorProfile[] ColorChangeObjects;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [enable].
    /// </summary>
    protected override void Awake() { base.Awake(); }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        DeckManager.OnActiveDecksUpdated += DeckManager_OnActiveDecksUpdated;
        DeckManager.OnDeckIsolationToggled += DeckManager_OnDeckIsolationToggled;

        xmlWriter = xmlWriterFactory.Create(gameObject, XMLType.Isolate);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Assigning here so that we don't lose our listener when it's turned off.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        DeckManager.OnActiveDecksUpdated -= DeckManager_OnActiveDecksUpdated;
        DeckManager.OnDeckIsolationToggled -= DeckManager_OnDeckIsolationToggled;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    void OnValidate()
    {
        int colorProfilesCount = ColorChangeObjects.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            ColorChangeObjects[i].OnValidate();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        //base.BTN_OnClick();

        if (State == ActiveState.Disabled)
            return;

        IsIsolating = !IsIsolating;

        //State = (IsIsolating) ? ActiveState.Selected : ActiveState.Enabled;

        DeckManager.OnDeckIsolationToggled?.Invoke(IsIsolating);

        //if(IsIsolating)
        //SendAnalytics();

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange()
    {
        button.interactable = (State == ActiveState.Disabled) ? false : true;
        int colorProfilesCount = ColorChangeObjects.Length;

        switch (State)
        {
            case ActiveState.Disabled:
                button.interactable = false;

                for (int i = 0; i < colorProfilesCount; ++i)
                {
                    ColorChangeObjects[i].StateChange(ActiveState.Disabled);
                }
                break;

            case ActiveState.Enabled:
                button.interactable = true;

                for (int i = 0; i < colorProfilesCount; ++i)
                {
                    ColorChangeObjects[i].StateChange(ActiveState.Enabled);
                }
                break;

            case ActiveState.Selected:
                zoomOut.EnableZoomOut();
                button.interactable = true;
    
                for (int i = 0; i < colorProfilesCount; ++i)
                {
                    ColorChangeObjects[i].StateChange(ActiveState.Enabled);
                }
                break;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void DeckManager_OnDeckIsolationToggled(bool value)
    {
        IsIsolating = value;
        State = (value) ? ActiveState.Selected : ActiveState.Enabled;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Listens for DeckManager's active decks updated action.
    /// </summary>
    /// <param name="value">The value.</param>
    void DeckManager_OnActiveDecksUpdated(int value)
    {
        if (State == ActiveState.Selected)
            return;


        if (value == 0)
        {
            State = ActiveState.Disabled;
            return;
        }
        else
        {
            State = ActiveState.Enabled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    async void SendAnalytics()
    {
        string decks = "";
		int activeDeckCount = deckManager.GetActiveDecks.Length;
        for (int i = 0; i < activeDeckCount; ++i)
        {
            decks += deckManager.GetActiveDecks[i];
            if (i < deckManager.GetActiveDecks.Length - 1)
                decks += "|";
        }

        await xmlWriter.AttemptCustomSave(decks);
        await xmlWriter.Save();
    }
}
#endif