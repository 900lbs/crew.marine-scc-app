// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipInstaller.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5
/// <summary>
/// Class ShipInstaller.
/// Implements the <see cref="Zenject.MonoInstaller{ShipInstaller}" />
/// </summary>
/// <seealso cref="Zenject.MonoInstaller{ShipInstaller}" />
public class ShipInstaller : MonoInstaller<ShipInstaller>
{
    /// <summary>
    /// The ship manager object
    /// </summary>
    [Space(5f)]
    [Header("Managers")]
    public GameObject ShipManagerObject;

    public LegendsHandler LegendsHandler;
    /// <summary>
    /// The zoom in deck object
    /// </summary>
    public ZoomInToDeck ZoomInDeckObject;
    /// <summary>
    /// The zoom out object
    /// </summary>
    public ZoomOut ZoomOutObject;

    /// <summary>
    /// The injection queue list
    /// </summary>
    [Space(5f)]
    [Header("Misc.")]
    public List<Object> InjectionQueueList;

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the bindings.
    /// </summary>
    public override void InstallBindings()
    {
        InstallFactories();
        InstallShipManager();
        InstallDeckManager();
        InstallLegends();
        InstallZoomComponents();
        InjectionQueue();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the factories.
    /// </summary>
    void InstallFactories()
    {
        Container.BindFactory<Object, ShipFeatureData, ShipFeature, ShipFeature.Factory>()
        .FromFactory<ShipFeatureFactory>();

        Container.BindFactory<ShipID, ShipProfile, ShipProfile.Factory>()
         .FromFactory<ShipProfileFactory>();

        Container.BindFactory<Object, ShipProfile, ShipSelectionButton, ShipSelectionButton.Factory>()
        .FromFactory<ShipSelectionButtonFactory>();

        Container.BindFactory<Object, string, Sprite, Task<Deck>, Deck.Factory>()
        .FromFactory<DeckFactory>();

        Container.BindFactory<Object, string, DeckSelector, DeckSelector.Factory>()
        .FromFactory<DeckSelectorFactory>();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the ship manager.
    /// </summary>
    public void InstallShipManager()
    {
        Container.Bind<ShipManager>()
        .FromComponentOn(ShipManagerObject)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the deck manager.
    /// </summary>
    public void InstallDeckManager()
    {
        Container.BindInterfacesAndSelfTo<DeckManager>()
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the deck components.
    /// </summary>
    public void InstallLegends()
    {
        Container.Bind<LegendsHandler>()
        .FromComponentOn(LegendsHandler.gameObject)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the zoom components.
    /// </summary>
    void InstallZoomComponents()
    {
        Container.BindInterfacesAndSelfTo<ZoomInToDeck>()
        .FromComponentOn(ZoomInDeckObject.gameObject)
        .AsSingle()
        .NonLazy();

        Container.BindInterfacesAndSelfTo<ZoomOut>()
        .FromComponentOn(ZoomOutObject.gameObject)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Injections the queue.
    /// </summary>
    void InjectionQueue()
    {
        //Since these scriptable objects are not created via a factory for dynamic injection, 
        //we simply queue each one listed for injection.
        foreach (var item in InjectionQueueList)
        {
            Container.QueueForInject(item);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

}
#endif