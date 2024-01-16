using System;
// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="ShipFeatureData.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SCC_2_5
/// <summary>
/// Class ShipFeatureData.
/// </summary>
[System.Serializable]
public class ShipFeatureData
{
    /// <summary>
    /// The feature name
    /// </summary>
    [SerializeField]
    string featureName;
    /// <summary>
    /// Gets the name of the feature.
    /// </summary>
    /// <value>The name of the feature.</value>
    public string FeatureName { get { return featureName; } }

    public ShipID Ship;

    public FeatureTool OptionalFeatureTool;

    /// <summary>
    /// The column
    /// </summary>
    [SerializeField]

    ColumnID column;
    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>The column.</value>
    public ColumnID Column { get { return column; } }

    /// <summary>
    /// The overlays
    /// </summary>
    [SerializeField]
    Sprite[] overlays;
    /// <summary>
    /// Gets the overlays.
    /// </summary>
    /// <value>The overlays.</value>
    public Sprite[] Overlays { get { return overlays; } }

    /// <summary>
    /// The overlay dictionary
    /// </summary>
    Dictionary<string, Sprite> overlayDict;

    /// <summary>
    /// Gets the overlay dictionary.
    /// </summary>
    /// <value>The overlay dictionary.</value>
    public Dictionary<string, Sprite> OverlayDict { get { return overlayDict; } }

    [ReadOnly]
    public string AssetBundleName;

    public ShipFeatureData(ShipID newShip, string newFeatureName, FeatureTool newTool, ColumnID newColumn,
    Sprite[] newOverlays)
    {
        Ship = newShip;
        featureName = newFeatureName;
        OptionalFeatureTool = newTool;
        column = newColumn;
        overlays = newOverlays;
    }

    public void AssignTextures(Sprite[] textures)
    {
        overlays = textures;
    }

    public void CopyTo(ShipFeatureData data)
    {
        this.Ship = data.Ship;
        this.featureName = data.featureName;
        this.OptionalFeatureTool = data.OptionalFeatureTool;
        this.column = data.column;
        this.overlays = data.overlays;
    }


    /*------------------------------------------------------------------------------------------------*/


    /// <summary>
    /// Safe method to add a new sprite to the dictionary.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="sprite">The sprite.</param>
    public void AddSpriteToDictionary(string name, Sprite sprite)
    {
        if (!overlayDict.ContainsKey(name))
        {
            overlayDict.Add(name, sprite);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Safe method to query the dictionary and return the corresponding sprite.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Sprite.</returns>
    public Sprite RetrieveSpriteFromDictionary(string key)
    {
        if (overlayDict.ContainsKey(key))
        {
            return overlayDict[key];
        }
        else
        {
            Debug.LogError(key + " was not found in " + featureName + "'s Dictionary.");
            return null;
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Use to assign each sprite found into the feature's dictionary.
    /// </summary>
    public void AssignAllOverlaysToDictionary()
    {
        overlayDict = new Dictionary<string, Sprite>();

        foreach (Sprite overlaySprite in overlays)
        {
            string index = DeckUtility.GetDeckIndex(overlaySprite.name);

            if (!overlayDict.ContainsKey(index))
            {
                overlayDict.Add(index, overlaySprite);
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif