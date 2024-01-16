// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipSelectionButtonFactory.cs" company="900lbs of Creative">
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
/// Class ShipSelectionButtonFactory.
/// Implements the <see cref="Zenject.IFactory{UnityEngine.Object, ShipProfile, ShipSelectionButton}" />
/// </summary>
/// <seealso cref="Zenject.IFactory{UnityEngine.Object, ShipProfile, ShipSelectionButton}" />
public class ShipSelectionButtonFactory : IFactory<Object, ShipProfile, ShipSelectionButton>
{
	#region Injection Construction
	/// <summary>
	/// The container
	/// </summary>
	readonly DiContainer _container;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShipSelectionButtonFactory"/> class.
	/// </summary>
	/// <param name="container">The container.</param>
	[Inject]
    public ShipSelectionButtonFactory(DiContainer container)
    {
        _container = container;
    }

	#endregion

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Creates the specified prefab.
	/// </summary>
	/// <param name="prefab">The prefab.</param>
	/// <param name="profile">The profile.</param>
	/// <returns>ShipSelectionButton.</returns>
	public ShipSelectionButton Create(Object prefab, ShipProfile profile)
    {
        ShipSelectionButton button = _container.InstantiatePrefabForComponent<ShipSelectionButton>(prefab);
        button.ButtonText.text = profile.Ship.ToString();
        button.Profile = profile;

        return button;
    }
}

#endif