// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-28-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="NetworkAnnotationObject.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class NetworkAnnotationObject.
/// Implements the <see cref="INetworkObject" />
/// </summary>
/// <seealso cref="INetworkObject" />
public class NetworkAnnotationObject : INetworkObject
{
    public CancellationTokenSource cts { get; set; }

    /// <summary>
    /// Gets or sets the net event.
    /// </summary>
    /// <value>The net event.</value>
    public NetworkEvent NetEvent { get; set; }

    /// <summary>
    /// Optionally, have this event update or create a room property.
    /// </summary>
    public ShipRoomProperties NetOptionalRoomProp;

    /// <summary>
    /// Type of object
    /// </summary>
    public NetworkStorageType NetStorageType;

    /// <summary>
    /// Gets or sets the player identifier.
    /// </summary>
    /// <value>The player identifier.</value>
    public string PlayerID { get; set; }

    /// <summary>
    /// The net identifier
    /// </summary>
    public short NetID;

    /// <summary>
    /// The actual data being sent, stored as an object for modularity and network purposes.
    /// </summary>
    /// <value>The net data.</value>
    public object NetData { get; set; }

    /// <summary>
    /// The set dirty
    /// </summary>
    public bool SetDirty;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkAnnotationObject"/> class.
    /// </summary>
    public NetworkAnnotationObject() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkAnnotationObject"/> class.
    /// </summary>
    /// <param name="newNetEvent">The new net event.</param>
    /// <param name="newNetOptionalRoomProp">The new net optional room property.</param>
    /// <param name="newNetStorageType">New type of the net storage.</param>
    /// <param name="newPlayerID">The new player identifier.</param>
    /// <param name="newNetID">The new net identifier.</param>
    /// <param name="newNetData">The new net data.</param>
    /// <param name="newSetDirty">if set to <c>true</c> [new set dirty].</param>
    public NetworkAnnotationObject(NetworkEvent newNetEvent, ShipRoomProperties newNetOptionalRoomProp,
    NetworkStorageType newNetStorageType, string newPlayerID, short newNetID, object newNetData,
    bool newSetDirty)
    {
        NetEvent = newNetEvent;
        NetOptionalRoomProp = newNetOptionalRoomProp;
        NetStorageType = newNetStorageType;
        PlayerID = newPlayerID;
        NetID = newNetID;
        NetData = newNetData;
        SetDirty = newSetDirty;

        cts = new CancellationTokenSource();
    }

    /// <summary>
    /// Converts from network object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ConvertFromNetworkObject(object[] value)
    {
        try
        {
            object[] values = (object[])value;

            NetEvent = (NetworkEvent)values[0];
            NetOptionalRoomProp = (ShipRoomProperties)values[1];
            NetStorageType = (NetworkStorageType)values[2];
            PlayerID = (string)values[3];
            NetID = (short)values[4];
            NetData = (object)values[5];
            SetDirty = (bool)values[6];
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
            if (!cts.IsCancellationRequested)
            {
                await new WaitForBackgroundThread();
                object[] messageArray = new object[]
                {
            NetEvent,
            NetOptionalRoomProp,
            NetStorageType,
            PlayerID,
            NetID,
            NetData,
            SetDirty
                };
                return messageArray;
            }
        }
        catch (System.Exception)
        {
			cts.Cancel();
            throw;
        }
        await new WaitForEndOfFrame();
        return null;
    }
}
