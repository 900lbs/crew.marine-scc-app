// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="MenuInstaller.cs" company="900lbs of Creative">
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
/// Installs all facets of the main menu (i.e ship/user selection, connection UI etc)
/// Implements the <see cref="Zenject.MonoInstaller{MenuInstaller}" />
/// </summary>
/// <seealso cref="Zenject.MonoInstaller{MenuInstaller}" />
public class MenuInstaller : MonoInstaller<MenuInstaller>
{
    /// <summary>
    /// The main menu prefab
    /// </summary>
    public GameObject MainMenuPrefab;

    /// <summary>
    /// The ship data
    /// </summary>
    //public ShipVariable ShipData;
    /// <summary>
    /// Installs the bindings.
    /// </summary>
    public override void InstallBindings()
    {
        InstallMainMenu();
        InstallShipSettingsBindings();
        InstallFactories();
        InstallGameStates();
        InstallMisc();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the main menu.
    /// </summary>
    void InstallMainMenu()
    {
        Container.Bind<MainMenu>()
        .FromComponentInNewPrefab(MainMenuPrefab)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the factories.
    /// </summary>
    void InstallFactories()
    {
        Container.BindFactory<StartupState, StartupState.Factory>().WhenInjectedInto<GameStateFactory>();
        Container.BindFactory<BridgeState, BridgeState.Factory>().WhenInjectedInto<GameStateFactory>();
        Container.BindFactory<DisconnectedState, DisconnectedState.Factory>().WhenInjectedInto<GameStateFactory>();
        Container.BindFactory<RoomState, RoomState.Factory>().WhenInjectedInto<GameStateFactory>();
        Container.BindFactory<ShipSelectionState, ShipSelectionState.Factory>().WhenInjectedInto<GameStateFactory>();
        Container.BindFactory<UserSelectionState, UserSelectionState.Factory>().WhenInjectedInto<GameStateFactory>();

        Container.BindFactory<Object, PromptMenu, PromptMenu.Factory>()
        .FromFactory<PromptMenuFactory>();

        Container.BindFactory<Object, UI_Button, UI_Button.Factory>()
        .FromFactory<UI_ButtonFactory>();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the game states.
    /// </summary>
    void InstallGameStates()
    {
        Container.Bind<GameStateFactory>().AsSingle();

        Container.BindInterfacesAndSelfTo<StartupState>().AsSingle();
        Container.BindInterfacesAndSelfTo<BridgeState>().AsSingle();
        Container.BindInterfacesAndSelfTo<DisconnectedState>().AsSingle();
        Container.BindInterfacesAndSelfTo<RoomState>().AsSingle();
        Container.BindInterfacesAndSelfTo<ShipSelectionState>().AsSingle();
        Container.BindInterfacesAndSelfTo<UserSelectionState>().AsSingle();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the ship settings bindings.
    /// </summary>
    void InstallShipSettingsBindings()
    {
        Container.Bind<ShipProfileManager>()
        .AsSingle()
        .NonLazy();

        //Container.BindInstance(ShipData).WithId("CurrentShip").AsSingle();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the misc.
    /// </summary>
    void InstallMisc()
    {
        Container.Bind<ASyncProcessor>()
        .FromNewComponentOnNewGameObject()
        .AsSingle();

        Container.BindInterfacesAndSelfTo<Prompt>().AsTransient();
    }

    /*------------------------------------------------------------------------------------------------*/
}
#endif