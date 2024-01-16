// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="INetworkClient.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#define Zenject
using Zenject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

#if Zenject
/// <summary>
/// Inteface for the public network accessor that all scripts will use and read from,
/// Implements the <see cref="Zenject.IInitializable" />
/// </summary>
/// <seealso cref="Zenject.IInitializable" />
public interface INetworkClient : IInitializable
{
    /// <summary>
    /// Gets or sets the scene set.
    /// </summary>
    /// <value>The scene set.</value>
    Settings.SceneSettings SceneSet { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is master client.
    /// </summary>
    /// <value><c>true</c> if this instance is master client; otherwise, <c>false</c>.</value>
    bool IsMasterClient { get; }

    /// <summary>
    /// Clients the state changed.
    /// </summary>
    /// <param name="state">The state.</param>
    void ClientStateChanged(ClientState state);

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [connected to master server].
    /// </summary>
    void OnConnectedToMasterServer();

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnDisconnected(DisconnectCause cause);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [created room].
    /// </summary>
    Task OnCreatedRoom();

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [joined room].
    /// </summary>
    void OnJoinedRoom();

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [join room failed].
    /// </summary>
    void OnJoinRoomFailed();

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Updates the room property.
    /// </summary>
    /// <param name="table">The table.</param>
    Task UpdateRoomProperty(byte table, params object[] optionalParameters);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Rooms the properties updated.
    /// </summary>
    /// <param name="props">The props.</param>
    void RoomPropertiesUpdated(Hashtable props);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [player joined room].
    /// </summary>
    /// <param name="player">The player.</param>
    void OnPlayerJoinedRoom(Player player);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [player left room].
    /// </summary>
    /// <param name="player">The player.</param>
    void OnPlayerLeftRoom(Player player);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Fires all active players.
    /// </summary>
    Task FireAllActivePlayers();

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Updates the network settings.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>
    void UpdateNetworkSettings(string ipAddress);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes the network settings.
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    bool InitializeNetworkSettings(string roomID);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the room.
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool CreateRoom(string roomID);

    /*----------------------------------------------------------------------------------------------------------------------------*/

    bool CheckIfRoomIsOnline(ShipID ship);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Joins the room.
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool JoinRoom(string roomID);

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Joins the room.
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool JoinRoom(ShipID roomID);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sends the new network event received.
    /// </summary>
    /// <param name="netObject">The net object.</param>
    Task SendNewNetworkEventReceived(INetworkObject netObject);

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sends the new network event.
    /// </summary>
    /// <param name="netObject">The net object.</param>
    Task SendNewNetworkEvent(INetworkObject netObject);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Adds the listener.
    /// </summary>
    /// <param name="value">The value.</param>
    void AddListener(object value);

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Removes the listener.
    /// </summary>
    /// <param name="value">The value.</param>
    void RemoveListener(object value);

    /*------------------------------------------------------------------------------------------------*/

}

#elif (!Zenject)
public interface INetworkClient<T> where T : INetworkClient<T>
{        
    void Initialize();
    void OnCreatedRoom();
    void OnJoinedRoom();
    void UpdateRoomProperty(byte table);
    void RoomPropertiesUpdated(Hashtable props);
    void SendNewNetworkEvent(INetworkEvent networkClassOrStruct);

    void AddListener(object value);
    void RemoveListener(object value);
}
#endif