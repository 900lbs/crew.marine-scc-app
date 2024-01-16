// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="StartupState.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Zenject;
using System.Threading.Tasks;

#if SCC_2_5
/// <summary>
/// This is our initial state when the application begins.
/// Implements the <see cref="GameStateEntity" />
/// </summary>
/// <seealso cref="GameStateEntity" />
public class StartupState : GameStateEntity
{
    #region Injection Construction
    /// <summary>
    /// The main menu
    /// </summary>
    readonly MainMenu mainMenu;
    /// <summary>
    /// The network client
    /// </summary>
    readonly INetworkClient networkClient;
    /// <summary>
    /// The signal bus
    /// </summary>
    readonly SignalBus _signalBus;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupState"/> class.
    /// </summary>
    /// <param name="menu">The menu.</param>
    /// <param name="netClient">The net client.</param>
    /// <param name="signal">The signal.</param>
    [Inject]
    public StartupState(MainMenu menu, INetworkClient netClient, SignalBus signal)
    {
        mainMenu = menu;
        networkClient = netClient;
        _signalBus = signal;
    }
    /// <summary>
    /// The current ship
    /// </summary>
    [Inject(Id = "CurrentShip")]
    ShipVariable currentShip;

    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public override void Initialize()
    {
        //Debug.Log("StartupState Initialized");
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public async override void Start()
    {
        await mainMenu.SplashScreenControl.BeginSplashSequence(1.5f);

        mainMenu.SplashScreenControl.FadeCanvas(false);
        mainMenu.MenuControl.FadeCanvas(true);
        //Debug.Log("StartupState Started");

        if (currentShip.Ship.ID == ShipID.None)
        {
            mainMenu.ChangeState(GameState.ShipSelection);
            return;
        }
        else
        {
            _signalBus.Fire<Signal_MainMenu_OnShipInitialize>(new Signal_MainMenu_OnShipInitialize(currentShip.Ship.ID));
            //networkClient.InitializeNetworkSettings(shipData.Ship.ID.ToString());
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Ticks this instance.
    /// </summary>
    public override void Tick()
    {
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Lates the dispose.
    /// </summary>
    public override void LateDispose()
    {

        //Debug.Log("StartupState Disposed");
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    async Task SplashScreenSequence()
    {

    }

    /// <summary>
    /// Class Factory.
    /// Implements the <see cref="Zenject.PlaceholderFactory{StartupState}" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{StartupState}" />
    public class Factory : PlaceholderFactory<StartupState> { }
}
#endif