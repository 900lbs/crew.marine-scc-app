// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipProfileManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

#if SCC_2_5
/// <summary>
/// Handles the processing and containment of ship profiles.
/// </summary>
public class ShipProfileManager
{
	#region Injection Construction
	/// <summary>
	/// The network client
	/// </summary>
	INetworkClient networkClient;
	/// <summary>
	/// The signal bus
	/// </summary>
	SignalBus _signalBus;
	/// <summary>
	/// The ship profile factory
	/// </summary>
	ShipProfile.Factory shipProfileFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShipProfileManager"/> class.
	/// </summary>
	/// <param name="netClient">The net client.</param>
	/// <param name="signal">The signal.</param>
	[Inject]
    public ShipProfileManager(INetworkClient netClient, SignalBus signal/* , ShipProfile.Factory shipProfileFact */)
    {
        networkClient = netClient;
        _signalBus = signal;
        //shipProfileFactory = shipProfileFact;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	#endregion

	/// <summary>
	/// List of all loaded Ship Profiles.
	/// </summary>
	List<ShipProfile> ShipProfiles;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Creates all ship profiles sent to it via ShipVariables.
	/// </summary>
	/// <param name="data">The data.</param>
	public void ProcessShipData(ShipVariable[] data)
    {
        Debug.Log(data.Length + " ships found, processing now.");
        ShipProfiles = new List<ShipProfile>();

		int shipCount = data.Length;

        for (int i = 0; i < shipCount; ++i)
        {
            ShipProfile newShip = shipProfileFactory.Create(data[i].Ship.ID);
            ShipProfiles.Add(newShip);
        }

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets all ship profiles.
	/// </summary>
	/// <returns>List&lt;ShipProfile&gt;.</returns>
	public List<ShipProfile> GetAllShipProfiles()
    {
        return ShipProfiles;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif