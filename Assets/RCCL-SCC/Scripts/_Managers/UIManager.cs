// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="UIManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Zenject;

#if SCC_2_5
/// <summary>
/// Class UIManager.
/// Implements the <see cref="Zenject.IInitializable" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="Zenject.IInitializable" />
/// <seealso cref="System.IDisposable" />
public class UIManager : IInitializable, IDisposable
{
	#region Injection Construction
	/// <summary>
	/// The UI
	/// </summary>
	public ShipUIVariable UI;
	/// <summary>
	/// Gets the deck spawn.
	/// </summary>
	/// <value>The deck spawn.</value>
	public RectTransform DeckSpawn { get; private set; }
	/// <summary>
	/// Gets the deck selector holder.
	/// </summary>
	/// <value>The deck selector holder.</value>
	public RectTransform DeckSelectorHolder { get; private set; }
	/// <summary>
	/// Gets the left feature column.
	/// </summary>
	/// <value>The left feature column.</value>
	public RectTransform LeftFeatureColumn { get; private set; }
	/// <summary>
	/// Gets the middle feature column.
	/// </summary>
	/// <value>The middle feature column.</value>
	public RectTransform MiddleFeatureColumn { get; private set; }
	/// <summary>
	/// Gets the right feature column.
	/// </summary>
	/// <value>The right feature column.</value>
	public RectTransform RightFeatureColumn { get; private set; }

	/// <summary>
	/// Creates new iconprefab.
	/// </summary>
	/// <value>The new icon prefab.</value>
	public IconBehavior NewIconPrefab { get; private set; }

	/// <summary>
	/// The signal bus
	/// </summary>
	readonly SignalBus _signalBus;

	/// <summary>
	/// Initializes a new instance of the <see cref="UIManager"/> class.
	/// </summary>
	/// <param name="newUI">The new UI.</param>
	/// <param name="objects">The objects.</param>
	/// <param name="signal">The signal.</param>
	[Inject]
    public UIManager(ShipUIVariable newUI, UIObjects objects, SignalBus signal)
    {
        UI = newUI;
        DeckSpawn = objects.DeckSpawn;
        DeckSelectorHolder = objects.DeckSelectorHolder;
        LeftFeatureColumn = objects.LeftFeatureColumn;
        MiddleFeatureColumn = objects.MiddleFeatureColumn;
        RightFeatureColumn = objects.RightFeatureColumn;
        NewIconPrefab = objects.SafetyIconPrefab;
        _signalBus = signal;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	#endregion

	/// <summary>
	/// The on initialize
	/// </summary>
	public static Action OnInitialize; //Need to convert to signal
									   /// <summary>
									   /// The on destroy ship assets
									   /// </summary>
	public static Action OnDestroyShipAssets; //Need to convert to signal or rework completely.

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public void Initialize()
    {
        _signalBus.Subscribe<Signal_NetworkClient_OnClientStateChanged>(NetworkClient_OnClientStateChanged);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Disposes this instance.
	/// </summary>
	public void Dispose()
    {
        _signalBus.Unsubscribe<Signal_NetworkClient_OnClientStateChanged>(NetworkClient_OnClientStateChanged);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// May change in the future, simply listens for the correct state to load everything.
	/// </summary>
	/// <param name="signal">The signal.</param>
	public static void NetworkClient_OnClientStateChanged(Signal_NetworkClient_OnClientStateChanged signal)
    {
        if (signal.NewState == ClientState.ConnectedToMasterserver)
            OnInitialize?.Invoke();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Setups the new ship.
	/// </summary>
	public static void SetupNewShip()
    {
        OnDestroyShipAssets?.Invoke();
        OnInitialize?.Invoke();
    }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Class UIObjects.
/// </summary>
[System.Serializable]
public class UIObjects
{
	/// <summary>
	/// The deck spawn
	/// </summary>
	[Space(5f)]
    [Header("Spawn Locations")]
    public RectTransform DeckSpawn;
	/// <summary>
	/// The deck selector holder
	/// </summary>
	public RectTransform DeckSelectorHolder;
	/// <summary>
	/// The left feature column
	/// </summary>
	public RectTransform LeftFeatureColumn;
	/// <summary>
	/// The middle feature column
	/// </summary>
	public RectTransform MiddleFeatureColumn;
	/// <summary>
	/// The right feature column
	/// </summary>
	public RectTransform RightFeatureColumn;

	/// <summary>
	/// The safety icon prefab
	/// </summary>
	[Space(5)]
    [Header("UI Prefabs")]
    public IconBehavior SafetyIconPrefab;

}
#endif