using System.Threading;
// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-28-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="NetworkShareObject.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
/// <summary>
/// Used for network share actions.
/// Implements the <see cref="INetworkObject" />
/// </summary>
/// <seealso cref="INetworkObject" />
/// <remarks>
/// i.e ShareMyView
/// </remarks>
/// <example> 
/// NetworkShareObject newObject = new NetworkShareObject(
//                        NetworkEvent.Create,
//                        Photon.Pun.PhotonNetwork.NickName, 
//                        networkDataString, 
//                        );
//                
//                    networkClient?.SendNewNetworkEvent(newObject);
//</example>
public class NetworkKillCardRequestObject : INetworkObject
{
    public CancellationTokenSource cts { get; set; }

    public NetworkKillCardRequestObject()
    {
        cts = new CancellationTokenSource();
    }
    /// <summary>
    /// Gets or sets the network event.
    /// </summary>
    /// <value>The network event such as Create, Edit, Delete etc, this tells the network what you want to do with the information.1</value>
    public NetworkEvent NetEvent { get; set; }

    /// <summary>
    /// Gets or sets the player identifier.
    /// </summary>
    /// <value>The player identifier, when sending an event this is your nickname.</value>
    public string PlayerID { get; set; }

    /// <summary>
    /// Gets or sets the network data.
    /// </summary>
    /// <remarks>
    /// The network data object that you're sending over the network, generally we will use a string or string array for
    /// performance reason.
    /// </remarks>
    public object NetData { get; set; }

    public string RequestingUsernameID;



    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes a new completed instance of the <see cref="NetworkShareObject"/> class.
    /// </summary>
    /// <param name="newPlayerID">The new player identifier.</param>
    /// <param name="newNetData">The new net data.</param>
    public NetworkKillCardRequestObject(NetworkEvent newNetEvent, string newPlayerID, object newNetData, string newRequestingUsernameID)
    {
        NetEvent = newNetEvent;
        PlayerID = newPlayerID;
        NetData = newNetData;
        RequestingUsernameID = newRequestingUsernameID;
        cts = new CancellationTokenSource();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Converts from network object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ConvertFromNetworkObject(object[] value)
    {
        try
        {
            object[] values = (object[]) value;

            NetEvent = (NetworkEvent) values[0];
            PlayerID = (string) values[1];
            NetData = (object) values[2];
            RequestingUsernameID = (string) values[3];
            if((string) values[4] != "KillCardRequest")
                return false;
            return true;
        }
        catch (System.Exception)
        {
            return false;
            throw;
        }

    }

    /// <summary>
    /// Converts to network object.
    /// </summary>
    /// <returns>System.Object[].</returns>
    public async Task<object[]> ConvertToNetworkObject()
    {
        try
        {
            await new WaitForBackgroundThread();
            if (!cts.IsCancellationRequested)
            {
                object[] messageArray = new object[]
                {
                    NetEvent,
                    PlayerID,
                    NetData,
                    RequestingUsernameID,
                    "KillCardRequest"
                };

                await new WaitForUpdate();
                return messageArray;
            }
        }

        catch (System.Exception)
        {
            cts.Cancel();
            throw;
        }
        await new WaitForUpdate();
        return null;
    }

/*     public void DebugObject()
    {
        Debug.Log("NetworkEvent: " + (NetworkEvent)NetEvent + " / " + "PlayerID: " + PlayerID.ToString() + "NetData");
    } */

}