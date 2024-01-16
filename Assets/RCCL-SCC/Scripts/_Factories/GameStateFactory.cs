// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 05-21-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="GameStateFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SCC_2_5

public enum GameState
{
    StartUp, 
    Bridging,
    Disconnected,
    Room,
    ShipSelection,
    UserSelection
}
/// <summary>
/// Used for the creation of GameStates, this factory strictly handles creating the requested type.
/// </summary>
/// <remarks>
///  Do not use unless necessary, refer to MainMenu.cs for creating a new gamestate.
///  See the GameState derived classes themselves for all logic.
/// </remarks>
/// <example>
/// Example: [Inject] GameStateFactory gameStateFactory
/// gameStateFactory.Create(GameState.StartupState);
/// </example>
public class GameStateFactory
{
    #region Injection Construction
    readonly StartupState.Factory startupFactory;
    readonly BridgeState.Factory connectingFactory;
    readonly DisconnectedState.Factory disconnectedFactory;
    readonly RoomState.Factory roomFactory;
    readonly ShipSelectionState.Factory shipSelectionFactory;
    readonly UserSelectionState.Factory userSelectionFactory;

    public GameStateFactory(StartupState.Factory startupFact,
                            BridgeState.Factory connectingFact,
                            DisconnectedState.Factory disconnectedFact,
                            RoomState.Factory roomFact,
                            ShipSelectionState.Factory shipSelectionFact,
                            UserSelectionState.Factory userSelectionFact)
    {
        startupFactory = startupFact;
        connectingFactory = connectingFact;
        disconnectedFactory = disconnectedFact;
        roomFactory = roomFact;
        shipSelectionFactory = shipSelectionFact;
        userSelectionFactory = userSelectionFact;
    }
    #endregion

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the requested game state entity
    /// </summary>
    /// <param name="gameState">State we want to create</param>
    /// <returns>The requested game state entity</returns>
    internal GameStateEntity CreateState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.StartUp:
                return startupFactory.Create();

            case GameState.Bridging:
                return connectingFactory.Create();

            case GameState.Disconnected:
                return disconnectedFactory.Create();

            case GameState.Room:
                return roomFactory.Create();

            case GameState.ShipSelection:
                return shipSelectionFactory.Create();

            case GameState.UserSelection:
                return userSelectionFactory.Create();

            default:
                Debug.LogError("Well, shit.");
                return null;
        }

    }
}
#endif