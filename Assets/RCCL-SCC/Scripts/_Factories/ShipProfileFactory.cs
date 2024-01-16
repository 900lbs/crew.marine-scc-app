// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipProfileFactory.cs" company="900lbs of Creative">
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
/// Class ShipProfileFactory.
/// Implements the <see cref="Zenject.IFactory{ShipID, ShipProfile}" />
/// </summary>
/// <seealso cref="Zenject.IFactory{ShipID, ShipProfile}" />
public class ShipProfileFactory : IFactory<ShipID, ShipProfile>
{
	#region Injection Construction
	/// <summary>
	/// The ship profile factory
	/// </summary>
	readonly ShipProfile.Factory shipProfileFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShipProfileFactory"/> class.
	/// </summary>
	/// <param name="shipProfileFact">The ship profile fact.</param>
	[Inject]
    public ShipProfileFactory(ShipProfile.Factory shipProfileFact)
    {
        shipProfileFactory = shipProfileFact;
    }
	#endregion

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Creates the specified ship identifier.
	/// </summary>
	/// <param name="shipID">The ship identifier.</param>
	/// <returns>ShipProfile.</returns>
	public ShipProfile Create(ShipID shipID)
    {
        ShipProfile newShipProfile = shipProfileFactory.Create(shipID);

        newShipProfile.Ship = shipID;
        newShipProfile.IpAddress = GetIPAddress(shipID);

        Debug.Log("Creating new ShipProfile for: " + shipID.ToString());

        return newShipProfile;
    }

	/// <summary>
	/// Assign static public IP's here.
	/// </summary>
	/// <param name="ship">The ship.</param>
	/// <returns>System.String.</returns>
	string GetIPAddress(ShipID ship)
    {
        switch (ship)
        {
            case ShipID.Edge:
            case ShipID.Spectrum:
            case ShipID.Symphony:
                return "4.16.237.2";
            default:
                Debug.LogError("Invalid ShipID found when retreiving IP address for server");
                return null;
        }
    }
}
#endif