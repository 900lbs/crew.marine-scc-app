using System.Threading;

// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="ShipVariable.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;

#region Data Classes

/// <summary>
/// Determines an objects place in the features column, this gives the user control to assign specific features to certain columns and populate
/// empty buttons for the rest.
/// </summary>

[System.Serializable]
public class ShipData
{
    #region Properties

    /// <summary>
    /// The identifier
    /// </summary>
    [Header("Scene Assets")]
    [SerializeField]
    private ShipID id;

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public ShipID ID
    { get { return id; } set { id = value; } }

    /// <summary>
    /// The decks
    /// </summary>
    [SerializeField]
    private Sprite[] decks;

    /// <summary>
    /// Gets the decks.
    /// </summary>
    /// <value>The decks.</value>
    public Sprite[] Decks
    { get { return decks; } }

    /// <summary>
    /// The decks
    /// </summary>
    [SerializeField]
    private Sprite[] cabins;

    /// <summary>
    /// Optional cabin textures.
    /// </summary>
    /// <value>The decks.</value>
    public Sprite[] Cabins
    { get { return cabins; } }

    [SerializeField]
    private LegendFeatureData[] legends;

    public LegendFeatureData[] Legends
    { get { return legends; } }

    /// <summary>
    /// The features
    /// </summary>
    [ReadOnly]
    public string[] Features;

    /// <summary>
    /// The eniram ship identifier
    /// </summary>
    [Space(5f)]
    [Header("Ship Data")]
    [SerializeField] private string shipAbbreviation;

    public string ShipAbbreviation
    { get { return shipAbbreviation; } }

    [SerializeField] private string eniramShipID;

    /// <summary>
    /// Gets the eniram ship identifier.
    /// </summary>
    /// <value>The eniram ship identifier.</value>
    public string EniramShipID
    { get { return eniramShipID; } }

    [SerializeField] private string eMusteringURL;

    /// <summary>
    /// Gets the eniram ship identifier.
    /// </summary>
    /// <value>The eniram ship identifier.</value>
    public string EMusteringURL
    { get { return eMusteringURL; } }

    #endregion Properties

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipData"/> class.
    /// </summary>
    /// <param name="newShipName">New name of the ship.</param>
    /// <param name="newShipMiniMap">The new ship mini map.</param>
    /// <param name="newDecks">The new decks.</param>
    /// <param name="newFeatures">The new features.</param>
    /// <param name="newEniramShipID">The new eniram ship identifier.</param>
    public ShipData(ShipID newShipName,
    Sprite[] newDecks,
    string newDeckNames,
    Sprite[] newCabins,
    LegendFeatureData[] newLegends,
    string[] newFeatures,
    string newEniramShipID,
    string newEmusteringURL)
    {
        id = newShipName;
        decks = newDecks;
        cabins = newCabins;
        legends = newLegends;
        Features = newFeatures;
        eniramShipID = newEniramShipID;
        eMusteringURL = newEmusteringURL;
    }

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="targetData">The target data.</param>
    /// <returns>ShipData.</returns>
    public ShipData CopyTo(ShipData targetData)
    {
        targetData.id = id;
        targetData.decks = decks;
        targetData.cabins = cabins;
        targetData.legends = legends;
        targetData.Features = Features;
        targetData.eniramShipID = eniramShipID;
        targetData.eMusteringURL = eMusteringURL;

        return targetData;
    }

    public void ResetShip()
    {
        id = 0;
        decks = null;
        cabins = null;
        legends = null;
        Features = null;
        eniramShipID = null;
        eMusteringURL = null;
    }
}

#endregion Data Classes

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Class ShipVariable.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />
[System.Serializable]
[CreateAssetMenu(menuName = "Ships/New Ship", fileName = "New Ship")]
public class ShipVariable : ScriptableObject
{
    [Inject]
    private CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// The ship
    /// </summary>
    [SerializeField]
    public ShipData Ship = new ShipData(ShipID.None, null, null, null, null, null, "", "");

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void ResetShip()
    {
        Ship.ResetShip();
    }

    public async void DebugLoad()
    {
        await LoadAssets(Ship.Decks, "persistent");
    }

    public async Task LoadAssets(Sprite[] sprites, string bundleName)
    {
        await GetAssetBundle(sprites, bundleName);
    }

    public async Task GetAssetBundle(Sprite[] sprites, string bundleName)
    {
        AssetBundle bundleLoadRequest;
        List<Sprite> tempSpriteList = new List<Sprite>();

        bundleLoadRequest = await AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(Application.streamingAssetsPath, (Ship.ID.ToString().ToLower() + "/" + bundleName)));

        string[] assetNames = bundleLoadRequest.GetAllAssetNames();
        for (int i = 0; i < assetNames.Length; i++)
        {
            var assetSprites = bundleLoadRequest.LoadAssetWithSubAssets<Sprite>(assetNames[i]);
            tempSpriteList.AddRange(assetSprites);
            Debug.Log("Added sprite: " + tempSpriteList.Count, this);
        }
    }
}