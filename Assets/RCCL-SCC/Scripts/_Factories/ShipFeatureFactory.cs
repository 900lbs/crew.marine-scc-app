// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-22-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-12-2019
// ***********************************************************************
// <copyright file="ShipFeatureFactory.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5
/// <summary>
/// Class ShipFeatureFactory.
/// Implements the <see cref="Zenject.IFactory{UnityEngine.Object, ShipFeatureData, ShipFeature}" />
/// </summary>
/// <seealso cref="Zenject.IFactory{UnityEngine.Object, ShipFeatureData, ShipFeature}" />
public class ShipFeatureFactory : IFactory<Object, ShipFeatureData, ShipFeature>
{
	#region Injection Construction
	/// <summary>
	/// The deck manager
	/// </summary>
	DeckManager deckManager;
	/// <summary>
	/// The container
	/// </summary>
	DiContainer _container;
	/// <summary>
	/// Initializes a new instance of the <see cref="ShipFeatureFactory"/> class.
	/// </summary>
	/// <param name="deckMan">The deck man.</param>
	/// <param name="container">The container.</param>
	public ShipFeatureFactory(DeckManager deckMan, DiContainer container)
    {
        deckManager = deckMan;
        _container = container;
    }

	#endregion

	/*------------------------------------------------------------------------------------------------*/


	/// <summary>
	/// Creates the specified prefab.
	/// </summary>
	/// <param name="prefab">The prefab.</param>
	/// <param name="data">The data.</param>
	/// <returns>ShipFeature.</returns>
	public ShipFeature Create(Object prefab, ShipFeatureData data)
    {
        ShipFeature newFeature = _container.InstantiatePrefabForComponent<ShipFeature>(prefab);

        newFeature.AssignFeature(data);
        newFeature.State = ActiveState.Enabled;

        return newFeature;
    }
}
#endif