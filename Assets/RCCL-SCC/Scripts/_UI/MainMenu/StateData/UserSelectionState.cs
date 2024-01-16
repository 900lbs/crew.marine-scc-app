// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="UserSelectionState.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

#if SCC_2_5
/// <summary>
/// This is our state when we are selecting our user ID.
/// Implements the <see cref="GameStateEntity" />
/// </summary>
/// <seealso cref="GameStateEntity" />
public class UserSelectionState : GameStateEntity
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
	/// The ship data
	/// </summary>
	[Inject(Id = "CurrentShip")]
	readonly ShipVariable currentShip;

	/// <summary>
	/// Initializes a new instance of the <see cref="UserSelectionState"/> class.
	/// </summary>
	/// <param name="mainMen">The main men.</param>
	/// <param name="netClient">The net client.</param>
	[Inject]
	public UserSelectionState(MainMenu mainMen, INetworkClient netClient)
	{
		mainMenu = mainMen;
		networkClient = netClient;
	}

	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public override void Initialize()
	{
		//Debug.Log("UserSelectionState Initialized");
	}

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Starts this instance.
	/// </summary>
	public override void Start()
	{
		//networkClient.InitializeNetworkSettings(currentShip.Ship.ID.ToString());
		mainMenu.UserProfileControl.FadeCanvas(true);
		mainMenu.MenuControl.FadeCanvas(true);

		//Debug.Log("UserSelectionState Started");
	}

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Ticks this instance.
	/// </summary>
	public override void Tick() { }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Lates the dispose.
	/// </summary>
	public override void LateDispose()
	{
		mainMenu.UserProfileControl.FadeCanvas(false);
		//Debug.Log("UserSelectionState Disposed");
	}

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Class Factory.
	/// Implements the <see cref="Zenject.PlaceholderFactory{UserSelectionState}" />
	/// </summary>
	/// <seealso cref="Zenject.PlaceholderFactory{UserSelectionState}" />
	public class Factory : PlaceholderFactory<UserSelectionState> { }

}

#endif