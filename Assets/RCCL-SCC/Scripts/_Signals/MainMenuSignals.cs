// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-19-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="MainMenuSignals.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

/// <summary>
/// Class Signal_MainMenu_OnShipInitialize.
/// </summary>
public class Signal_MainMenu_OnShipInitialize
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_MainMenu_OnShipInitialize"/> class.
    /// </summary>
    /// <param name="shipID">The ship identifier.</param>
    public Signal_MainMenu_OnShipInitialize(ShipID shipID)
    {
        Ship = shipID;
    }

    /// <summary>
    /// The ship
    /// </summary>
    public ShipID Ship;
}

/// <summary>
/// Updates all prompt menus as to which one has been changed, also includes optional parameters
/// [0] = Message that is displayed.
/// </summary>
public class Signal_MainMenu_OnPromptMenuChanged
{
    public Signal_MainMenu_OnPromptMenuChanged(PromptMenuType type, params object[] optionalParams)
    {
        Debug.Log("Prompt menu changed: " + type.ToString() + "Optional Parameters: " + optionalParams.Length);
        PromptType = type;
        OptionalParams = optionalParams;
    }

    public PromptMenuType PromptType { get; private set; }
    public object[] OptionalParams { get; private set; }
}

public class Signal_MainMenu_OnGameStateChanged
{
    public Signal_MainMenu_OnGameStateChanged(GameState state)
    {
        State = state;
    }

    public GameState State { get; private set; }
}

public class Signal_MainMenu_OnShipInitialized
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_MainMenu_OnShipInitialize"/> class.
    /// </summary>
    /// <param name="shipID">The ship identifier.</param>
    public Signal_MainMenu_OnShipInitialized() { }

}

