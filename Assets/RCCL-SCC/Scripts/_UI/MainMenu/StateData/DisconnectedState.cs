// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="DisconnectedState.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
/// <summary>
/// Handles disconnections based on user authority.
/// Implements the <see cref="GameStateEntity" />
/// </summary>
/// <seealso cref="GameStateEntity" />
public class DisconnectedState : GameStateEntity
{
	/// <summary>
	/// The main menu
	/// </summary>
	readonly MainMenu mainMenu;

	/// <summary>
	/// Initializes a new instance of the <see cref="DisconnectedState"/> class.
	/// </summary>
	/// <param name="menu">The menu.</param>
	public DisconnectedState(MainMenu menu)
    {
        mainMenu = menu;
    }

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public override void Initialize()
    {
        //Debug.Log("DisconnectedState Initialized");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Starts this instance.
	/// </summary>
	public override void Start()
    {
        mainMenu.DisconnectionControl.FadeCanvas(true);
     
        //Debug.Log("DisconnectedState Started");
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
        mainMenu.DisconnectionControl.FadeCanvas(false);
        //Debug.Log("DisconnectedState Disposed");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Class Factory.
	/// Implements the <see cref="Zenject.PlaceholderFactory{DisconnectedState}" />
	/// </summary>
	/// <seealso cref="Zenject.PlaceholderFactory{DisconnectedState}" />
	public class Factory : PlaceholderFactory<DisconnectedState> { }

}

