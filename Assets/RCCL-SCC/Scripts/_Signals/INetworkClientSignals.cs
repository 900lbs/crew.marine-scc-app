// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="INetworkClientSignals.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using Photon.Realtime;
using Zenject;

/// <summary>
/// Let's subscribers know that the client's connection state has changed.
/// </summary>

public class Signal_NetworkClient_OnClientStateChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_NetworkClient_OnClientStateChanged"/> class.
    /// </summary>
    /// <param name="newState">The new state.</param>
    public Signal_NetworkClient_OnClientStateChanged(ClientState newState)
    {
        NewState = newState;
        //Debug.Log("Client State changed to: " + newState.ToString());
    }

    /// <summary>
    /// Creates new state.
    /// </summary>
    /// <value>The new state.</value>
    public ClientState NewState { get; private set; }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Let's subscribers know that the Active Users list has changed.
/// </summary>
public class Signal_NetworkClient_OnActiveUsersUpdated
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_NetworkClient_OnActiveUsersUpdated"/> class.
    /// </summary>
    /// <param name="users">The users.</param>
    public Signal_NetworkClient_OnActiveUsersUpdated(UserNameID users, object sender)
    {
        Users = users;
        Debug.Log("Users updated: " + Users.ToString() + " by: " + sender);
    }

    /// <summary>
    /// The users
    /// </summary>
    public UserNameID Users;
}

/*----------------------------------------------------------------------------------------------------------------------------*/

public class Signal_NetworkClient_OnDisconnectionOccured
{
    public Signal_NetworkClient_OnDisconnectionOccured(DisconnectCause cause)
    {
        Cause = cause;
        //Debug.Log(Cause.ToString());
    }

    public DisconnectCause Cause { get; private set; }
}

public class Signal_NetworkClient_OnNetworkEventReceived
{
    public Signal_NetworkClient_OnNetworkEventReceived(INetworkObject networkObject)
    {
        NetworkObject = networkObject;
    }

    public INetworkObject NetworkObject { get; private set; }
}

public class Signal_NetworkClient_OnJoinRoomFailed
{
    public Signal_NetworkClient_OnJoinRoomFailed() { }

}

public class Signal_NetworkClient_OnOnlineOfflineToggled
{
    public Signal_NetworkClient_OnOnlineOfflineToggled(bool isOnline)
    {
        IsOnline = isOnline;
    }

    public bool IsOnline { get; private set; }
}


