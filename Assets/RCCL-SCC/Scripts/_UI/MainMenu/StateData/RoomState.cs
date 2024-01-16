// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="RoomState.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
/// <summary>
/// This is our active state when we're viewing a ship session.
/// Implements the <see cref="GameStateEntity" />
/// </summary>
/// <seealso cref="GameStateEntity" />
public class RoomState : GameStateEntity
{
	#region Injection Construction
	/// <summary>
	/// The network client
	/// </summary>
	readonly INetworkClient networkClient;
	/// <summary>
	/// The main menu
	/// </summary>
	readonly MainMenu mainMenu;
	/// <summary>
	/// Initializes a new instance of the <see cref="RoomState"/> class.
	/// </summary>
	/// <param name="netClient">The net client.</param>
	/// <param name="menu">The menu.</param>
	[Inject]
    public RoomState(INetworkClient netClient, MainMenu menu)
    {
        networkClient = netClient;
        mainMenu = menu;
    }

	#endregion
	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public override void Initialize()
    {
        //Debug.Log("RoomState Initialized");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Starts this instance.
	/// </summary>
	public override void Start()
    {
        mainMenu.MenuControl.FadeCanvas(false);
        //Debug.Log("RoomState Started");
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
        mainMenu.MenuControl.FadeCanvas(true);

        //Debug.Log("RoomState Disposed");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Class Factory.
	/// Implements the <see cref="Zenject.PlaceholderFactory{RoomState}" />
	/// </summary>
	/// <seealso cref="Zenject.PlaceholderFactory{RoomState}" />
	public class Factory : PlaceholderFactory<RoomState> { }

}

