// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="AssetBundleUtilities.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using Zenject;
/// <summary>
/// Static class used to handle AssetBundles.
/// </summary>
public static class AssetBundleUtilities
{

    /// <summary>
    /// Loads the asset bundle based on ship identifier.
    /// </summary>
    /// <param name="ship">The ship.</param>
    /// <returns>ShipVariable.</returns>
    public static ShipVariable LoadAssetBundleBasedOnShipID(ShipID ship)
    {
        //aSync.StartCoroutine(ClearUnusedAssetBundles(ship));
        AssetBundle myLoadedAssetBundle;

        string reformatted = ship.ToString().ToLower();
        try
        {
            myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, reformatted));
            Debug.Log(ship.ToString().ToLower() + " was successfully loaded.");
        }

        catch
        {
            Debug.LogError(ship.ToString().ToLower() + " was not successfully loaded.");
            return null;
        }

        try
        {
            return myLoadedAssetBundle.LoadAsset<ShipVariable>(ship.ToString());
        }
        catch (System.Exception)
        {
            Debug.LogError("Asset: " + ship.ToString() + "'s ShipVariable was not");
            throw;
        }

    }

    /// <summary>
    /// Checks if asset bundle is loaded.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    static bool CheckIfAssetBundleIsLoaded(string name)
    {
        IEnumerable<AssetBundle> listOfAssetBundles = AssetBundle.GetAllLoadedAssetBundles();

        foreach (var item in listOfAssetBundles)
        {
            if (item.name == name)
            {
                Debug.Log("Found AssetBundle: " + name);
                return true;
            }
        }
        Debug.Log("Did not find AssetBundle: " + name);
        return false;
    }

    /// <summary>
    /// Gets the asset bundle.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>AssetBundle.</returns>
    static AssetBundle GetAssetBundle(string name)
    {
        IEnumerable<AssetBundle> listOfAssetBundles = AssetBundle.GetAllLoadedAssetBundles();

        foreach (var item in listOfAssetBundles)
        {
            if (item.name == name)
            {
                return item;
            }
        }

        return null;
    }
}
