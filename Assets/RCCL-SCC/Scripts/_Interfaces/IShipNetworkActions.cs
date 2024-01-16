// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-04-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="IShipNetworkActions.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Threading.Tasks;
using UnityEngine;

using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

#if SCC_2_5
/// <summary>
/// Interface IShipNetworkActions
/// </summary>
public interface IShipNetworkActions
{
    /// <summary>
    /// Networks the action.
    /// </summary>
    /// <param name="additionalData">The additional data.</param>
    void NetworkAction(params object[] additionalData);
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Interface IShipNetworkEvents
/// </summary>
public interface IShipNetworkEvents
{
    /// <summary>
    /// Called when [network action event].
    /// </summary>
    /// <param name="value">The value.</param>
    void OnNetworkActionEvent(INetworkObject value);
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Class IShipNetworkObject.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
[System.Serializable]
public class IShipNetworkObject : SerializedMonoBehaviour
{
    //Necessary to keep track of in storage.
    /// <summary>
    /// The network object online
    /// </summary>
    public static Action<NetworkStorageType, short, IShipNetworkObject> NetworkObjectOnline;
    /// <summary>
    /// The network object offline
    /// </summary>
    public static Action<NetworkStorageType, short, IShipNetworkObject> NetworkObjectOffline;

    /// <summary>
    /// The storage type
    /// </summary>
    public NetworkStorageType StorageType;

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The player identifier
    /// </summary>
    [SerializeField]
    string playerID;

    /// <summary>
    /// Gets or sets the player identifier.
    /// </summary>
    /// <value>The player identifier.</value>
    public string PlayerID
    {
        get
        {
            return playerID;
        }

        set
        {
            playerID = value;
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The identifier
    /// </summary>
    [SerializeField]
    short id;

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public short ID
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The state
    /// </summary>
    [SerializeField]
    [ReadOnly]
    ActiveState state;
    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>The state.</value>
    public ActiveState State
    {
        get
        {
            return state;
        }
        set
        {
            if (state != value)
            {
                state = value;
                OnStateChange();
            }
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Add the base after all other Start functionality, that way everything is set before deciding to notify
    /// the data container.
    /// </summary>
    public virtual void Start()
    {
        NetworkObjectOnline?.Invoke(StorageType, ID, this);
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Use this function to update the object in whatever way it needs.
    /// </summary>
    public virtual void Edit(object value)
    {

    }

    /// <summary>
    /// Deletes the specified send network.
    /// </summary>
    /// <param name="sendNetwork">if set to <c>true</c> [send network].</param>
    public virtual async Task Delete(bool sendNetwork)
    {
        NetworkObjectOffline?.Invoke(StorageType, ID, this);
        try
        {
            if (gameObject != null)
                Destroy(gameObject);
            await new WaitForEndOfFrame();
        }
        catch (System.Exception)
        {

            await new WaitForEndOfFrame();
            return;
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [state change].
    /// </summary>
    public virtual void OnStateChange()
    {

    }

    /*------------------------------------------------------------------------------------------------*/

    public virtual void FinalizeAndSend(bool isNetwork, bool isDirty, NetworkEvent netEvent) { }

    public void GetEnumerator() { }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Class ShipNetworkActionHelpers.
/// </summary>
[System.Serializable]
public static class ShipNetworkActionHelpers
{
    /// <summary>
    /// Gets the type of the ship network.
    /// </summary>
    /// <param name="storageType">Type of the storage.</param>
    /// <returns>Type.</returns>
    public static Type GetShipNetworkType(this NetworkStorageType storageType)
    {
        switch (storageType)
        {
            case NetworkStorageType.LineRenderer:
                Type lineType = Type.GetType("UILineRenderer");
                return lineType;

                /*------------------------------------------------------------------------------------------------*/

            case NetworkStorageType.Icon:
                Type iconType = Type.GetType("IconBehaviour");
                return iconType;

                /*------------------------------------------------------------------------------------------------*/

            case NetworkStorageType.Timer:
                Debug.LogError("No type set for Timer's yet");
                return null;

                /*------------------------------------------------------------------------------------------------*/

            default:
                return null;
        }
    }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

#endif