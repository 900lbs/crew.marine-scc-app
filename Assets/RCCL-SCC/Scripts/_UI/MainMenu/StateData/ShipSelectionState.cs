// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="ShipSelectionState.cs" company="900lbs of Creative">
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
/// This is our state when selecting which ship we want to connect to.
/// Implements the <see cref="GameStateEntity" />
/// </summary>
/// <seealso cref="GameStateEntity" />
public class ShipSelectionState : GameStateEntity
{
	#region Injection Construction
	/// <summary>
	/// The main menu
	/// </summary>
	readonly MainMenu mainMenu;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShipSelectionState"/> class.
	/// </summary>
	/// <param name="menu">The menu.</param>
	public ShipSelectionState(MainMenu menu)
    {
        mainMenu = menu;
    }
	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public override void Initialize()
    {
        //Debug.Log("ShipSelectionState Initialized");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Starts this instance.
	/// </summary>
	public override void Start()
    {
        mainMenu.ShipSelectionControl.FadeCanvas(true);
        mainMenu.MenuControl.FadeCanvas(true);
     
        //Debug.Log("ShipSelectionState Started");
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
        mainMenu.ShipSelectionControl.FadeCanvas(false);
        
        //Debug.Log("ShipSelectionState Disposed");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Class Factory.
	/// Implements the <see cref="Zenject.PlaceholderFactory{ShipSelectionState}" />
	/// </summary>
	/// <seealso cref="Zenject.PlaceholderFactory{ShipSelectionState}" />
	public class Factory : PlaceholderFactory<ShipSelectionState> { }

}

#endif