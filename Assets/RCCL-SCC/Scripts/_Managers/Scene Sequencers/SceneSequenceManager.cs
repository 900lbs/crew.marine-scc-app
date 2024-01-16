// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-24-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-27-2019
// ***********************************************************************
// <copyright file="SceneSequenceManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Manager class that handles the scene initialization sequence.
/// Implements the <see cref="Zenject.IInitializable" />
/// Implements the <see cref="Zenject.ILateDisposable" />
/// </summary>
/// <seealso cref="Zenject.IInitializable" />
/// <seealso cref="Zenject.ILateDisposable" />
public class SceneSequenceManager : ISceneInitializer, ILateDisposable
{
    /// <summary>
    /// The current ship
    /// </summary>
    [Inject(Id = "CurrentShip")]
    public ShipVariable CurrentShip;


    /// <summary>
    /// The ship manager
    /// </summary>
    readonly ShipManager shipManager;

    /// <summary>
    /// The network client
    /// </summary>
    readonly NetworkClient networkClient;
    /// <summary>
    /// The main menu
    /// </summary>
    public MainMenu mainMenu { get; set; }

    readonly LegendsHandler legendsHandler;
    /// <summary>
    /// The eniram data feed
    /// </summary>
    //readonly EniramDataFeed eniramDataFeed;
    /// <summary>
    /// The signal bus
    /// </summary>
    public SignalBus _signalBus { get; set; }

    public CancellationTokenSource cts { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneSequenceManager"/> class.
    /// </summary>
    /// <param name="shipMan">The ship manager.</param>
    /// <param name="gaEdits">The ga edits manager.</param>
    /// <param name="netClient">The network client.</param>
    /// <param name="menu">The mainmenu.</param>
    /// <param name="eniramData">The eniram data.</param>
    /// <param name="signal">The signal bus.</param>
    [Inject]
    public SceneSequenceManager(ShipManager shipMan,
    NetworkClient netClient,
    MainMenu menu,
    LegendsHandler legendsHandle,
    //EniramDataFeed eniramData,
    SignalBus signal,
    CancellationTokenSource c)
    {
        shipManager = shipMan;
        networkClient = netClient;
        mainMenu = menu;
        legendsHandler = legendsHandle;
        //eniramDataFeed = eniramData;
        _signalBus = signal;
        cts = c;
        _signalBus.Subscribe<Signal_MainMenu_OnShipInitialize>(InitializeScene);
    }

    /// <summary>
    /// Lates the dispose.
    /// </summary>
    public void LateDispose()
    {
        _signalBus.Unsubscribe<Signal_MainMenu_OnShipInitialize>(InitializeScene);
    }

    /// <summary>
    /// Initializes the scene.
    /// </summary>
    /// <param name="signal">The signal.</param>
    public async void InitializeScene(Signal_MainMenu_OnShipInitialize signal)
    {
        if (!cts.IsCancellationRequested)
        {
            /*----------------------------------------------------------------------------------------------------------------------------*/
            //Set Main Menu to loading sequence.
            mainMenu.CurrentSceneState = SceneState.Loading;
            mainMenu.ChangeState(GameState.Bridging);

            /*----------------------------------------------------------------------------------------------------------------------------*/

            // All scene initialization logic goes here in sequence.
            await shipManager.Initialize(signal.Ship);


            /*----------------------------------------------------------------------------------------------------------------------------*/

            await legendsHandler.SpawnLegends();

            /*----------------------------------------------------------------------------------------------------------------------------*/

            //eniramDataFeed.Initialize();

            //Initialize network settings
            // This jumps the sequence to the NetworkClient, it will attempt to connect to the room and if it's successful, fire off the signal letting
            // everyone else know that it's time to finish initializing.
            networkClient.InitializeNetworkSettings(CurrentShip.Ship.ID.ToString());

            /*----------------------------------------------------------------------------------------------------------------------------*/

            //Set MainMenu to userselection for signing in.
            mainMenu.ChangeState(GameState.UserSelection);
            mainMenu.CurrentSceneState = SceneState.Idle;
        }
    }
}
