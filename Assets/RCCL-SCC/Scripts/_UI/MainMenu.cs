// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="MainMenu.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Used for behind-the-scenes state of the scene.
/// </summary>
public enum SceneState
{
    /// <summary>
    /// Idle, nothing going on.
    /// </summary>
    Idle,
    /// <summary>
    /// Loading assets
    /// </summary>
    Loading,
    /// <summary>
    /// Connecting to the masterserver or gameserver.
    /// </summary>
    Connecting
}

#if SCC_2_5
/// <summary>
/// The main scene state handler, access the MainMenu in order to change our current <see cref="GameState"/>. 
/// When changing to a different <see cref="GameState"/>, this automatically creates the <see cref="GameStateEntity"/> class associated 
/// and all logic is handled appropriately. For adding UI logic based on GameState’s, 
/// edit the <see cref="GameStateEntity"/> scripts themselves
/// Implements <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MainMenu : MonoBehaviourPunCallbacks
{
    #region Injection Construction
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;
    /// <summary>
    /// The game state factory
    /// </summary>
    GameStateFactory gameStateFactory;
    /// <summary>
    /// The ship manager
    /// </summary>
    ShipManager shipManager;
    /// <summary>
    /// The settings
    /// </summary>
    Settings settings;

    /// <summary>
    /// Constructs the specified signal.
    /// </summary>
    /// <param name="signal">The signal.</param>
    /// <param name="gameStateFact">The game state fact.</param>
    /// <param name="settings">The settings.</param>
    [Inject]
    public void Construct(SignalBus signal, GameStateFactory gameStateFact, Settings settings)
    {
        gameStateFactory = gameStateFact;
        _signalBus = signal;
        this.settings = settings;
    }

    [Inject(Id = "CurrentShip")]
    ShipVariable currentShip;
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The game state entity
    /// </summary>
    GameStateEntity gameStateEntity = null;

    /// <summary>
    /// Gets the state of the current game.
    /// </summary>
    /// <value>The state of the current game.</value>
    [SerializeField]
    public GameState currentGameState
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the state of the previous game.
    /// </summary>
    /// <value>The state of the previous game.</value>
    [SerializeField]
    public GameState previousGameState
    {
        get;
        private set;
    }

    public static GameState GetCurrentGameState;

    public bool IsStaticMenuVisible
    {
        get
        {
            return (MenuControl.isActiveAndEnabled);
        }

        set
        {
            MenuControl.gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// The current scene state
    /// </summary>
    public SceneState CurrentSceneState;

    public Camera MenuCamera;

    /// <summary>
    /// The menu control
    /// </summary>
    public _MenuController MenuControl;
    /// <summary>
    /// The bridging control
    /// </summary>
    public ConnectionController BridgingControl;
    /// <summary>
    /// The disconnection control
    /// </summary>
    public DisconnectionController DisconnectionControl;
    /// <summary>
    /// The ship selection control
    /// </summary>
    public ShipSelectionController ShipSelectionControl;
    /// <summary>
    /// The user profile control
    /// </summary>
    public UserProfileController UserProfileControl;

    [Space(5)]
    public SplashScreenController SplashScreenControl;

    public static Action<GameState> ChangeGameState;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        _signalBus.Subscribe<Signal_NetworkClient_OnClientStateChanged>(OnClientStateChanged);
        ChangeGameState += ChangeState;
        //_signalBus.Subscribe<Signal_NetworkClient_OnJoinRoomFailed>(JoinRoomFailed);

        MenuControl.cg.alpha = 1;
        BridgingControl.cg.alpha = 0;
        DisconnectionControl.cg.alpha = 0;
        ShipSelectionControl.cg.alpha = 0;
        UserProfileControl.cg.alpha = 0;

        ChangeState(GameState.StartUp);

        GetCurrentGameState = currentGameState;
        IsStaticMenuVisible = true;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [destroy].
    /// </summary>
    void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnClientStateChanged>(OnClientStateChanged);
        ChangeGameState -= ChangeState;
        //_signalBus.TryUnsubscribe<Signal_NetworkClient_OnJoinRoomFailed>(JoinRoomFailed);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Changes the game state
    /// </summary>
    /// <param name="gameState">The state to transition to</param>
    internal void ChangeState(GameState gameState)
    {
        if (gameStateEntity != null)
        {
            gameStateEntity.LateDispose();
            gameStateEntity = null;
        }

        previousGameState = currentGameState;
        currentGameState = gameState;
        GetCurrentGameState = currentGameState;

        if (previousGameState == GameState.Room)
            IsStaticMenuVisible = false;
        else
            IsStaticMenuVisible = true;

        Debug.Log("Previous game state: " + previousGameState.ToString());
        Debug.Log("Current game state: " + currentGameState.ToString());

        gameStateEntity = gameStateFactory.CreateState(gameState);
        gameStateEntity.Start();

        _signalBus.Fire<Signal_MainMenu_OnGameStateChanged>(new Signal_MainMenu_OnGameStateChanged(gameState));
        //ActivateCanvas(currentGameState);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [client state changed].
    /// </summary>
    /// <param name="signal">The signal.</param>
    void OnClientStateChanged(Signal_NetworkClient_OnClientStateChanged signal)
    {
        switch (signal.NewState)
        {
            case ClientState.Joined:
                CurrentSceneState = SceneState.Loading;
                ChangeState(GameState.Room);
                break;

            case ClientState.ConnectingToGameserver:
            case ClientState.ConnectingToMasterserver:
            case ClientState.ConnectingToNameServer:
                CurrentSceneState = SceneState.Connecting;
                ChangeState(GameState.Bridging);
                break;

            case ClientState.ConnectedToMasterserver:
                if (currentGameState != GameState.Disconnected && currentGameState != GameState.UserSelection)
                {
                    if (currentShip.Ship.ID != ShipID.None)
                        ChangeState(GameState.UserSelection);
                    else
                    {
                        ChangeState(GameState.ShipSelection);
                    }
                }
                break;
            case ClientState.ConnectedToGameserver:
                if (currentGameState != GameState.Room)
                    ChangeState(GameState.Room);
                break;

            case ClientState.Disconnected:
            case ClientState.DisconnectingFromGameserver:
            case ClientState.DisconnectingFromMasterserver:
                if (!NetworkClient.IsOfflineMode)
                    ChangeState(GameState.Disconnected);
                break;
            default:
                break;
        }
    }

    void JoinRoomFailed(Signal_NetworkClient_OnJoinRoomFailed signal)
    {
        UserProfileControl.PromptJoinFailed(true);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region PUN Callbacks

    public override void OnJoinedRoom() { }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    #endregion
}
#endif