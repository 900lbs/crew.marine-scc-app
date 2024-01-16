// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-04-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="NetworkStorageData.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;

using Sirenix.OdinInspector;

#if SCC_2_5
[System.Serializable]
/// <summary>
/// Data storage for annotations.
/// </summary>
public class NetworkStorageData
{
    /// <summary>
    /// The asset type
    /// </summary>
    public NetworkStorageType AssetType;

    /// <summary>
    /// The keys
    /// </summary>
    public List<short> _keys = new List<short>();

    /// <summary>
    /// The values
    /// </summary>
    [SerializeField]
    public List<Object> _values = new List<Object>();

    /// <summary>
    /// The created objects
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Simple display for the number of objects in this dictionary.")]
    protected int createdObjects;
    /// <summary>
    /// Gets or sets the created objects.
    /// </summary>
    /// <value>The created objects.</value>
    public int CreatedObjects
    {
        get
        {
            return createdObjects;
        }

        set
        {
            if (createdObjects != Assets.Count)
            {
                createdObjects = Assets.Count;
            }
        }
    }

    /// <summary>
    /// The assets
    /// </summary>
    Dictionary<short, IShipNetworkObject> assets;
    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <value>The assets.</value>
    [SerializeField]
    public Dictionary<short, IShipNetworkObject> Assets { get { return assets; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkStorageData"/> class.
    /// </summary>
    /// <param name="newType">The new type.</param>
    /// <param name="newAssets">The new assets.</param>
    public NetworkStorageData(NetworkStorageType newType, Dictionary<short, IShipNetworkObject> newAssets)
    {
        AssetType = newType;
        assets = newAssets;
    }

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(short key, IShipNetworkObject value)
    {
        if (!assets.ContainsKey(key))
        {
            assets.Add(key, value);
            _keys.Add(key);
            _values.Add(value.gameObject);
            CreatedObjects = Assets.Count;
        }
    }

    /// <summary>
    /// Removes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void Remove(short key)
    {
        if (assets.ContainsKey(key))
        {
            assets.Remove(key);
            int index = _keys.IndexOf(key);

            _keys.Remove(key);
            _values.RemoveAt(index);
            CreatedObjects = Assets.Count;
        }
    }

    public void Edit(short key, object value)
    {
        if (assets.ContainsKey(key))
        {
            assets[key].Edit(value);
        }
        else
        {
            Debug.LogError(key + " / " + AssetType + " was not found in cache.");
        }
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public async Task Clear()
    {
        List<IShipNetworkObject> temporaryAssets = new List<IShipNetworkObject>();
        temporaryAssets = assets.Values.ToList();
        //await new WaitForBackgroundThread();
        lock(temporaryAssets)
        {
            foreach (var item in temporaryAssets)
            {
                //Debug.Log("Clearing annotation");
                item.Delete(false);
            }
            assets = new Dictionary<short, IShipNetworkObject>();
        }
        await new WaitForUpdate();
        CreatedObjects = Assets.Count;
    }

    public async Task ClearOthers()
    {
        /* List<IShipNetworkObject> previousObjects = new List<IShipNetworkObject>();
        previousObjects = Assets.Values.ToList(); */
        Dictionary<short, IShipNetworkObject> temporaryAssets = new Dictionary<short, IShipNetworkObject>();

        //await new WaitForBackgroundThread();
        try
        {
            lock(assets)
            {
                foreach (var item in assets)
                {
                    if (item.Value.PlayerID == NetworkClient.GetUserName().ToString())
                        temporaryAssets.Add(item.Key, item.Value);
                    else
                        item.Value.Delete(false);

                }
            }
        }

        catch
        {
            await new WaitForUpdate();
            return;
        }

        await new WaitForUpdate();

        assets = temporaryAssets;
    }

    public async Task UpdateNetworkManually(NetworkClient networkClient)
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            List<IShipNetworkObject> previousObjects = new List<IShipNetworkObject>();
            previousObjects = Assets.Values.ToList();
            foreach (var item in previousObjects)
            {
                Debug.Log("<color=#fc6900> Reinstating an annotation on reconnect: " + "</color>", item);
                item.FinalizeAndSend(true, false, NetworkEvent.Create);
                await new WaitForEndOfFrame();
            }
        }
    }
}
#endif