// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 05-03-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="NetworkInstaller.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zenject;

#if SCC_2_5
/// <summary>
/// Class NetworkInstaller.
/// Implements the <see cref="Zenject.MonoInstaller" />
/// </summary>
/// <seealso cref="Zenject.MonoInstaller" />
public class NetworkInstaller : MonoInstaller
{
    public GameObject NetworkButtonPrefab;
	/// <summary>
	/// The scriptable object injection queue
	/// </summary>
	public List<ScriptableObject> ScriptableObjectInjectionQueue;

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Installs the bindings.
	/// </summary>
	public override void InstallBindings()
    {
        InstallNetworkManager();
        InstallNetworkClient();
        InstallUserProfileComponents();
        BindFactories();


        //We need to make sure that NetworkClientManager is initialized first.
        //This is a feature that utilizes Zenject.IInitializable to execute Initialize
        //on each class that you assign via their order number.
        Container.BindExecutionOrder<NetworkClientManager>(0);
        Container.BindExecutionOrder<NetworkClient>(-10);
        
        //Since these scriptable objects are not created via a factory for dynamic injection, 
        //we simply queue each one listed for injection.
        foreach (var item in ScriptableObjectInjectionQueue)
        {
            Container.QueueForInject(item);

        }

    }

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Installs the network manager.
	/// </summary>
	void InstallNetworkManager()
    {
        if (!Container.HasBinding<NetworkClientManager>())
            Container.BindInterfacesAndSelfTo<NetworkClientManager>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
    }

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Here is where we set the singleton reference to whichever version
	/// of our INetworkClient interface that we're using.
	/// </summary>
	void InstallNetworkClient()
    {
        Container.BindInterfacesAndSelfTo<NetworkClient>()
        .AsSingle()
        .Lazy();

        Container.Bind<NetworkRoomPropertyHandler>()
        .AsTransient()
        .Lazy();
    }

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Installs the user profile components.
	/// </summary>
	void InstallUserProfileComponents()
    {
        Container.BindInterfacesAndSelfTo<UserProfileManager>()
        .AsSingle()
        .NonLazy();
    }

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Binds the factories.
	/// </summary>
	void BindFactories()
    {
        Container.BindFactory<AuthorityID, ShipID, UserNameID, UserProfile, UserProfile.Factory>()
        .FromFactory<UserProfileFactory>();

        // Using BindFactoryCustomInterface here because our UserProfileButton factory may require
        // multiple variations.
        Container.BindFactoryCustomInterface<Object, UserProfile, UserProfileButton, UserProfileButton.Factory, IUserProfileButtonFactory>()
        .WithId(UserAction.Login)
        .FromFactory<UserProfileSignInButtonFactory>();

        Container.BindFactory<Object, NetworkAction, NetworkButton, NetworkButton.Factory>()
        .FromFactory<NetworkButtonFactory>();
    
    }
    /*------------------------------------------------------------------------------------------------*/
}
#endif