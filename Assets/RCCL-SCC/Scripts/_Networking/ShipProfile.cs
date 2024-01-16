// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-31-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipProfile.cs" company="900lbs of Creative">
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
/// All data pertaining to each ship.
/// </summary>
public class ShipProfile
{
	/// <summary>
	/// The ip address
	/// </summary>
	public string IpAddress;

	/// <summary>
	/// The ship
	/// </summary>
	public ShipID Ship;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Class Factory.
	/// Implements the <see cref="Zenject.PlaceholderFactory{ShipID, ShipProfile}" />
	/// </summary>
	/// <seealso cref="Zenject.PlaceholderFactory{ShipID, ShipProfile}" />
	public class Factory : PlaceholderFactory<ShipID, ShipProfile> { }
}
#endif