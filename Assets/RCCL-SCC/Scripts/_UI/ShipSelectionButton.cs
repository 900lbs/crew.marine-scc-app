// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="ShipSelectionButton.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using TMPro;
using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5
/// <summary>
/// Handles selecting a specific ship and alerting all listeners as to which one was selected.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class ShipSelectionButton : UI_Button
{
    #region Injection Construction
    /// <summary>
    /// The ship data
    /// </summary>
    [Inject(Id = "CurrentShip")]
    ShipVariable shipData;

    MainMenu mainMenu;

    /// <summary>
    /// Constructs the specified net client.
    /// </summary>
    /// <param name="netClient">The net client.</param>
    /// <param name="signal">The signal.</param>
    [Inject]
    public void Construct(MainMenu menu)
    {
        mainMenu = menu;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The profile
    /// </summary>
    public ShipProfile Profile;

    ShipSelectionHandler optionalShipSelectionHandler;

    //public ShipVariable shipData;

    /// <summary>
    /// The ship
    /// </summary>
    public ShipID Ship;
    /// <summary>
    /// The button text
    /// </summary>
    public TextMeshProUGUI ButtonText;

    /// <summary>
    /// The profiles
    /// </summary>
    public ColorProfile[] Profiles;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    void OnValidate()
    {
        int profilesLength = Profiles.Length;
        for (int i = 0; i < profilesLength; ++i)
        {
            Profiles[i].OnValidate();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        OnStateChange();
        State = ActiveState.Enabled;

        if (GetComponentInParent<ShipSelectionHandler>())
        {
            optionalShipSelectionHandler = GetComponentInParent<ShipSelectionHandler>();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        int profilesLength = Profiles.Length;
        for (int i = 0; i < profilesLength; ++i)
        {
            Profiles[i].ColorFlash(State, ActiveState.Selected);
        }

        optionalShipSelectionHandler?.ButtonSelected(this);

        _signalBus.Fire<Signal_MainMenu_OnShipInitialize>(new Signal_MainMenu_OnShipInitialize(Ship));

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange()
    {
        int profilesLength = Profiles.Length;
        for (int i = 0; i < profilesLength; ++i)
        {
            Profiles[i].StateChange(State);
        }

        switch (State)
        {
            case ActiveState.Disabled:
            case ActiveState.Enabled:
                break;
            case ActiveState.Selected:
                break;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Class Factory.
    /// Implements the <see cref="Zenject.PlaceholderFactory{UnityEngine.Object, ShipProfile, ShipSelectionButton}" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{UnityEngine.Object, ShipProfile, ShipSelectionButton}" />
    public new class Factory : PlaceholderFactory<Object, ShipProfile, ShipSelectionButton> { }
}
#endif