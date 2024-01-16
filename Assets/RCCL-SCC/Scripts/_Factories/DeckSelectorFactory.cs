// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 05-16-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="DeckSelectorFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

/// <summary>
/// Used for the creation of DeckSelector buttons, add all initialization information here.
/// </summary>
/// <typeparam name="string"></typeparam>
/// <typeparam name="DeckSelector"></typeparam>
#if SCC_2_5
public class DeckSelectorFactory : IFactory<Object, string, DeckSelector>
{
    #region Injection Construction

    readonly Transform deckTransform;
    readonly DiContainer container;

    public DeckSelectorFactory(DiContainer diContainer)
    {
        container = diContainer;
    }
    #endregion

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Simply assigns the correct DeckID and returns the instantiate gameobject.
    /// </summary>
    /// <param name="prefab">The prefab to spawn with.</param>
    /// <param name="deckID">Assigned deck ID</param>
    /// <returns></returns>
    public DeckSelector Create(Object prefab, string deckID)
    {
        DeckSelector newSelector = container.InstantiatePrefabForComponent<DeckSelector>(prefab);

        newSelector.DeckID = deckID;

        return newSelector;
    }
}
#endif