// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipSelectionController.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
#if SCC_2_5
/// <summary>
/// Controls the ship selection menu.
/// Implements the <see cref="_MenuController" />
/// </summary>
/// <seealso cref="_MenuController" />
[RequireComponent(typeof(CanvasGroup))]
public class ShipSelectionController : _MenuController
{
	#region Injection Construction
	/// <summary>
	/// The ship data list
	/// </summary>
	[Inject]
    //ShipProfileManager shipProfileManager;
    public ShipVariable[] ShipDataList;

	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Awakes this instance.
	/// </summary>
	public override void Awake()
    {
        base.Awake();
        //TODO: Figure out why this process is causing a stack overflow with factory creation.
        //shipProfileManager.ProcessShipData(ShipDataList);
    }


    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif