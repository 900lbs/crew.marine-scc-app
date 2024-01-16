// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-03-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="NetworkStorageAsset.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zenject;

#if SCC_2_5
/// <summary>
/// Created By: Joshua Bowers - 03/14/19
/// Last Edited By: Joshua Bowers - 06/14/19
/// Purpose: Scriptable object that stores annotation data and is used for updating that data securely.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// Implements the <see cref="UnityEngine.ISerializationCallbackReceiver" />
/// Implements the <see cref="IShipNetworkEvents" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />
/// <seealso cref="UnityEngine.ISerializationCallbackReceiver" />
/// <seealso cref="IShipNetworkEvents" />
[CreateAssetMenu(menuName = "Network Storage/New Storage Data", fileName = "New Storage Data")]
public class NetworkStorageAsset : ScriptableObject, ISerializationCallbackReceiver, IShipNetworkEvents
{
    #region Injection Construction
    /// <summary>
    /// The network client
    /// </summary>
    NetworkClient networkClient;

    /// <summary>
    /// Constructs the specified net client.
    /// </summary>
    /// <param name="netClient">The net client.</param>
    [Inject]
    public void Construct(NetworkClient netClient)
    {
        networkClient = netClient;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The storage data
    /// </summary>
    [SerializeField]
    public NetworkStorageData StorageData = new NetworkStorageData(NetworkStorageType.LineRenderer, new Dictionary<short, IShipNetworkObject>());

    /// <summary>
    /// The network action listener
    /// </summary>
    public IShipNetworkActions NetworkActionListener;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Implement this method to receive a callback before Unity serializes your object.
    /// </summary>
    public void OnBeforeSerialize()
    {
        if (this == null || StorageData == null)
            return;

        StorageData._keys?.Clear();
        StorageData._values?.Clear();

        foreach (var kvp in StorageData.Assets)
        {
            if (kvp.Value != null)
            {
                StorageData._keys?.Add(kvp.Key);
                StorageData._values?.Add(kvp.Value);
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Implement this method to receive a callback after Unity deserializes your object.
    /// </summary>
    public void OnAfterDeserialize()
    {
        StorageData.Clear();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [enable].
    /// </summary>
    void OnEnable()
    {
        IShipNetworkObject.NetworkObjectOnline += OnNetworkObjectOnline;
        IShipNetworkObject.NetworkObjectOffline += OnNetworkObjectOffline;
        NetworkClient.OnNewClientState += ClientStateChanged;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [disable].
    /// </summary>
    void OnDisable()
    {
        IShipNetworkObject.NetworkObjectOnline -= OnNetworkObjectOnline;
        IShipNetworkObject.NetworkObjectOffline -= OnNetworkObjectOffline;
        NetworkClient.OnNewClientState -= ClientStateChanged;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [network action event].
    /// </summary>
    /// <param name="value">The value.</param>
    public void OnNetworkActionEvent(INetworkObject value)
    {
        if (value.GetType() == typeof(NetworkAnnotationObject))
        {
            NetworkAnnotationObject annotationObject = (NetworkAnnotationObject) value;
            if (annotationObject != null)
            {
                if (annotationObject.NetStorageType.HasFlag(StorageData.AssetType))
                {

                    if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                        Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                        Debug.Log("<color=magenta>Disperment found a match, </color>" + name, this);

                    switch (value.NetEvent)
                    {
                        case NetworkEvent.Create:
                            //storageData.Add(value.NetID, value.NetData)
                            if (!Contains(annotationObject.NetID))
                            {
                                if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                                    Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                                    Debug.Log("Creating: " + (string) annotationObject.NetData, this);

                                Create(annotationObject.SetDirty, annotationObject.PlayerID, annotationObject.NetID, annotationObject.NetData);
                            }
                            break;

                            /*------------------------------------------------------------------------------------------------*/

                        case NetworkEvent.Erase:
                            //Debug.Log("Delete event received by: " + name, this);
                            Erase(annotationObject.PlayerID, annotationObject.NetID);
                            break;

                            /*------------------------------------------------------------------------------------------------*/

                        case NetworkEvent.EraseAll:
                            EraseAll(annotationObject);
                            break;

                            /*------------------------------------------------------------------------------------------------*/

                        case NetworkEvent.Edit:

                            if (Contains(annotationObject.NetID))
                            {
                                StorageData.Edit(annotationObject.NetID, annotationObject.NetData);
                            }
                            break;
                    }
                }
                return;
            }
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [network object online].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void OnNetworkObjectOnline(NetworkStorageType type, short key, IShipNetworkObject value)
    {
        if (StorageData.AssetType == type)
        {
            StorageData.Add(key, value);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [network object offline].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void OnNetworkObjectOffline(NetworkStorageType type, short key, IShipNetworkObject value)
    {
        if (StorageData.AssetType == type)
        {
            StorageData.Remove(key);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the specified is dirty.
    /// </summary>
    /// <param name="isDirty">if set to <c>true</c> [is dirty].</param>
    /// <param name="player">The player.</param>
    /// <param name="key">The key.</param>
    /// <param name="data">The data.</param>
    public void Create(bool isDirty, string player, short key, object data)
    {
        //Debug.Log("Storage asset is attempting to create a new " + StorageData.AssetType, this);

        //Debug.Log("Data received and converted: " + (string)data);
        if (!Contains(key))
        {
            NetworkActionListener.NetworkAction(isDirty, StorageData.AssetType, player, key, data);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Edits the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void Edit(string key)
    {

    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Erases the specified annotation.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="key">The key.</param>
    public void Erase(string player, short key)
    {
        Debug.Log("Delete called by: " + player, this);
        if (Contains(key))
        {
            //Debug.Log("Key and Player ID found in StorageData", this);
            StorageData.Assets[key].Delete(false);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Erases all.
    /// </summary>
    /// <param name="value">The value.</param>
    public void EraseAll(NetworkAnnotationObject value)
    {
        List<IShipNetworkObject> cachedAssets = new List<IShipNetworkObject>();
        cachedAssets.AddRange(StorageData.Assets.Values);
        UserNameID usersToErase = (UserNameID) value.NetData;

        foreach (var item in cachedAssets)
        {
            UserNameID userId = 0;
            Enum.TryParse(item.PlayerID, out userId);
            if (usersToErase.HasFlag(userId))
            {
                item.Delete(false);
            }
        }

        /* if (NetworkClient.GetMasterClientID() == value.PlayerID)
        {
            Debug.Log("Erase all called from master client.", this);
            foreach (var item in cachedAssets)
            {
                item.Delete(false);
            }
        }
        else
        {
            Debug.Log("Erase all called from player: " + value.PlayerID, this);
            foreach (var item in cachedAssets)
            {
                if (item.PlayerID == value.PlayerID)
                    item.Delete(false);
            }
        } */
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Adds the listener.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddListener(IShipNetworkActions value)
    {
        networkClient.AddListener(this);
        NetworkActionListener = value;
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Removes the listener.
    /// </summary>
    /// <param name="value">The value.</param>
    public void RemoveListener(IShipNetworkActions value)
    {
        if (NetworkActionListener != null)
        {
            NetworkActionListener = null;
            networkClient.RemoveListener(this);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.</returns>
    public bool Contains(short key)
    {
        return StorageData.Assets.ContainsKey(key);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
    public bool Contains(object value)
    {
        return StorageData.Assets.ContainsValue(value as IShipNetworkObject);
    }

    public async void ForceAnnotationPropertyUpdate()
    {
        //Debug.Log("<color=#fc6900> NetworkStorage has received MasterServer connection confirmation. </color>", this);
        if (StorageData.Assets.Count > 0)
        {
            //Debug.Log("<color=#fc6900> NetworkStorage found annotations to push up: " + StorageData.Assets.Count + " found. </color>", this);
            await StorageData.UpdateNetworkManually(networkClient);
        }
    }

    public async void ClientStateChanged(ClientState state)
    {
        await OnClientStateChanged(state);
    }

    public async Task OnClientStateChanged(ClientState state)
    {
        if (state == ClientState.Joined)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Updating annotation room properties, found: " + StorageData.Assets.Count);
                await StorageData.UpdateNetworkManually(networkClient);
            }
            else
            {
                Debug.Log("Clearing storage of annotations, found: " + StorageData.Assets.Count);
                await StorageData.Clear();
            }

            //await StorageData.ClearOthers();

            //await NetworkRoomPropertyHandler.SpawnAllProperties(networkClient);
        }

        if (state == ClientState.Leaving && !NetworkClient.GetIsPlayerMasterClient() && StorageData.Assets.Count > 0)
        {
            Debug.Log("Player is leaving room, clearing all annotations other than their own.");
            await StorageData.Clear();
        }

    }

    /*------------------------------------------------------------------------------------------------*/
}
#endif