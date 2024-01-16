using System.Linq;
using System.IO;

// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-25-2019
// ***********************************************************************
// <copyright file="ShipManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5

/// <summary>
/// Handles creation of all ship assets.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ShipManager : MonoBehaviour, IDynamicShipComponent
{
    #region Injection Construction

    /// <summary>
    /// The UI manager
    /// </summary>
    private UIManager uiManager;

    /// <summary>
    /// The deck manager
    /// </summary>
    private DeckManager deckManager;

    /// <summary>
    /// The deck selector factory
    /// </summary>
    private DeckSelector.Factory deckSelectorFactory;

    /// <summary>
    /// The ship feature factory
    /// </summary>
    private ShipFeature.Factory shipFeatureFactory;

    private Deck.Factory deckFactory;

    /// <summary>
    /// The signal bus
    /// </summary>
    private SignalBus _signalBus;

    /// <summary>
    /// The network client
    /// </summary>
    private INetworkClient networkClient;

    private CancellationTokenSource cancellationTokenSource;

    private Settings settingsInstaller;

    /// <summary>
    /// Constructs the specified UI man.
    /// </summary>
    /// <param name="uiMan">The UI man.</param>
    /// <param name="deckMan">The deck man.</param>
    /// <param name="deckSelectFact">The deck select fact.</param>
    /// <param name="shipFeatureFact">The ship feature fact.</param>
    /// <param name="signal">The signal.</param>
    /// <param name="netClient">The net client.</param>
    [Inject]
    private void Construct(UIManager uiMan, DeckManager deckMan, DeckSelector.Factory deckSelectFact,
    Deck.Factory deckFact, ShipFeature.Factory shipFeatureFact, SignalBus signal, INetworkClient netClient,
    CancellationTokenSource cancellationTokenSourc, Settings settingsInstall)
    {
        uiManager = uiMan;
        deckManager = deckMan;
        deckSelectorFactory = deckSelectFact;
        shipFeatureFactory = shipFeatureFact;
        deckFactory = deckFact;
        _signalBus = signal;
        networkClient = netClient;
        cancellationTokenSource = cancellationTokenSourc;
        settingsInstaller = settingsInstall;
    }

    #endregion Injection Construction

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The current ship
    /// </summary>
    [Inject(Id = "CurrentShip")]
    public ShipVariable CurrentShip;

    public ShipVariable LoadedShip;

    /// <summary>
    /// The deck prefab
    /// </summary>
    [Header("Prefabs")]
    public GameObject deckPrefab;

    /// <summary>
    /// The feature prefab
    /// </summary>
    public GameObject featurePrefab;

    /// <summary>
    /// The overlay prefab
    /// </summary>
    public GameObject overlayPrefab;

    /// <summary>
    /// The deck selector prefab
    /// </summary>
    public GameObject DeckSelectorPrefab;

    public Dictionary<string, string> FeatureReferenceDictionary;

    [SerializeField]
    public AssetBundle bundleLoadPersistentRequest;

    [SerializeField]
    public AssetBundle bundleLoadFeatureRequest;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Unity Functions

    private void Start()
    {
        _signalBus.Subscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    private void OnDestroy()
    {
        if (bundleLoadPersistentRequest != null)
            bundleLoadPersistentRequest.Unload(true);
        _signalBus.TryUnsubscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    #endregion Unity Functions

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes the specified ship.
    /// </summary>
    /// <param name="ship">The ship.</param>
    /// <returns>Task.</returns>
    public async Task Initialize(ShipID ship)
    {
        FeatureReferenceDictionary = new Dictionary<string, string>();
        try
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await LoadSceneAssets(ship);
        }
        catch (OperationCanceledException canceled)
        {
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Loads the scene assets.
    /// </summary>
    /// <param name="ship">The ship.</param>
    /// <returns>Task.</returns>
    private async Task LoadSceneAssets(ShipID ship)
    {
        try
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                if (bundleLoadPersistentRequest != null)
                {
                    Debug.Log("Bundle found, unloading: " + bundleLoadPersistentRequest.name, this);
                    bundleLoadPersistentRequest.Unload(true);
                }
                string path = ship.ToString().ToLower() + "/persistent";
                bundleLoadPersistentRequest = await AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(Application.streamingAssetsPath, path));
                LoadedShip = await bundleLoadPersistentRequest.LoadAssetAsync<ShipVariable>(ship.ToString()) as ShipVariable;
                LoadedShip.Ship.CopyTo(CurrentShip.Ship);
                await ShipInit();
            }
        }
        catch (OperationCanceledException canceled)
        {
            throw;
        }
        catch (System.Exception e)
        {
            Debug.Log("Error: " + e.Message + "/ " + e.Source, this);
            //throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Ships the initialize.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task ShipInit()
    {
        try
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                int deckCount = CurrentShip.Ship.Decks.Length;
                int featureCount = CurrentShip.Ship.Features.Length;

                for (int i = deckCount - 1; i >= 0; --i)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    await CreateDeck(CurrentShip.Ship.Decks[i]);
                }

                for (int i = 0; i < featureCount; ++i)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    await CreateFeature(transform, CurrentShip.Ship.Features[i]);
                }
            }
        }
        catch (OperationCanceledException canceled)
        {
            Debug.LogError("Ship initialization error : " + canceled.Message + " / Stacktrace: " + canceled.StackTrace);
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Create a new feature from the prefab and turn it loose on assigning everything else.
    /// </summary>
    /// <param name="parent">Where the feature should spawn.</param>
    /// <param name="data">The data.</param>
    /// <returns>Task.</returns>
    private async Task CreateFeature(Transform parent, string name)
    {
        Debug.Log("Creating feature: " + name);
        try
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            string formattedBundle = name.Substring(name.LastIndexOf(CONSTANTS.bundleDelimeter) + 1);

            Debug.Log("<color=green>Formatted feature: " + formattedBundle + "</color>");
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, name);
            Debug.Log("<color=green>Feature path: " + path + "</color>");
            bundleLoadFeatureRequest = await AssetBundle.LoadFromFileAsync(path);
            ShipFeatureVariable feature = await bundleLoadFeatureRequest.LoadAssetAsync<ShipFeatureVariable>(formattedBundle) as ShipFeatureVariable;

            if (feature == null)
            {
                Debug.LogError("Feature '" + name + "' not found.");
                return;
            }

            if (!cancellationTokenSource.IsCancellationRequested)
            {
                if (feature.data.OptionalFeatureTool != FeatureTool.None)
                {
                    _signalBus.Fire<Signal_ShipManager_OnFeatureToolCreated>(new Signal_ShipManager_OnFeatureToolCreated(feature.data.OptionalFeatureTool, feature.data));
                }
                else if (feature.data.Overlays.Length > 0)
                {
                    ShipFeature newFeature = shipFeatureFactory.Create(featurePrefab, feature.data);
                    newFeature.name = feature.data.FeatureName;
                    newFeature.AssignFeature(feature.data);
                    newFeature.assetBundle = path;
                    newFeature.assetName = formattedBundle;

                    string referenceName = feature.data.FeatureName; //Cache the feature name before we unload it
                    FeatureReferenceDictionary.Add(referenceName, name); //Create a reference dictionary for Features to reference from when loading their bundle.

                    bundleLoadFeatureRequest.Unload(true);
                    await new WaitForUpdate();

                    newFeature.transform.SetParent(FindColumnLocation(FindNextColumn()));
                    newFeature.transform.localScale = Vector3.one;
                }
            }
        }
        catch (OperationCanceledException canceled)
        {
            throw;
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("Feature failed to complete, '{0}' / '{1}'", e.InnerException, e.StackTrace);
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Create a new deck from the prefab and turn it loose on assigning everything else.
    /// </summary>
    /// <param name="deck">The deck image, is used to generate the naming of other pieces as well.</param>
    /// <returns>Task.</returns>
    private async Task CreateDeck(Sprite deck)
    {
        try
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (!cancellationTokenSource.IsCancellationRequested)
            {
                string deckName = DeckUtility.GetDeckIndex(deck.name);

                Deck newDeck = await deckFactory.Create(deckPrefab, deckName, deck);
                newDeck.transform.SetParent(uiManager.DeckSpawn);

                await newDeck.InitializeDeck(deckName, deck);

                if (CurrentShip.Ship.Cabins != null)
                {
                    int cabinCount = CurrentShip.Ship.Cabins.Length;
                    for (int i = 0; i < cabinCount; i++)
                    {
                        if (DeckUtility.GetDeckIndex(CurrentShip.Ship.Cabins[i].name) == newDeck.DeckID)
                        {
                            GameObject cabinsObject = new GameObject("Deck_Cabins", typeof(Image));
                            cabinsObject.transform.SetParent(newDeck.transform);

                            Image newCabins = cabinsObject.GetComponent<Image>();
                            newCabins.transform.SetAsLastSibling();
                            newCabins.rectTransform.localPosition = Vector3.zero;
                            newCabins.raycastTarget = false;

                            RectTransform deckRect = newDeck.DeckImage.transform.GetComponent<RectTransform>();

                            newCabins.rectTransform.anchorMax = deckRect.anchorMax;
                            newCabins.rectTransform.anchorMin = deckRect.anchorMin;
                            newCabins.rectTransform.pivot = deckRect.pivot;
                            newCabins.rectTransform.sizeDelta = deckRect.sizeDelta;
                            newCabins.rectTransform.localPosition = deckRect.localPosition;
                            newCabins.rectTransform.localScale = deckRect.localScale;

                            newCabins.sprite = CurrentShip.Ship.Cabins[i];
                            continue;
                        }
                    }
                }

                await CreateNewDeckSelector(deckName);

                deckManager.AddDeck(deckName, newDeck);
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the new deck selector.
    /// </summary>
    /// <param name="deckID">The deck identifier.</param>
    /// <returns>Task.</returns>
    private async Task CreateNewDeckSelector(string deckID)
    {
        try
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (!cancellationTokenSource.IsCancellationRequested)
            {
                DeckSelector selector = deckSelectorFactory.Create(DeckSelectorPrefab, deckID);

                await new WaitForUpdate();

                selector.transform.SetParent(uiManager.DeckSelectorHolder);
                selector.transform.localScale = Vector3.one;
                selector.name = "DeckSelector_" + deckID;

                await selector.InitializeSelector(deckID);
            }
        }
        catch (OperationCanceledException canceled)
        {
            throw;
        }
        catch
        {
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Finds the column location.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Transform.</returns>
    private Transform FindColumnLocation(ColumnID value)
    {
        switch (value)
        {
            case ColumnID.Left:

                return uiManager.LeftFeatureColumn;

            case ColumnID.Middle:

                return uiManager.MiddleFeatureColumn;

            case ColumnID.Right:

                return uiManager.RightFeatureColumn;

            default:
                return null;
        }
    }

    /// <summary>
    /// Finds the next available column, in order to distribute features evenly.
    /// </summary>
    /// <returns>The chosen Column based on organization.</returns>
    private ColumnID FindNextColumn()
    {
        int leftCount = uiManager.LeftFeatureColumn.childCount;
        int middleCount = uiManager.MiddleFeatureColumn.childCount;
        int rightCount = uiManager.RightFeatureColumn.childCount;

        if (!settingsInstaller.IsTabletUser)
        {
            if (leftCount == middleCount && leftCount == rightCount)
                return ColumnID.Left;
            if (middleCount < leftCount && middleCount == rightCount)
                return ColumnID.Middle;
            else
                return ColumnID.Right;
        }
        else
        {
            if (leftCount == middleCount && leftCount < 3)
                return ColumnID.Left;
            if (middleCount < leftCount && middleCount < 3)
                return ColumnID.Middle;
            else
                return ColumnID.Right;
        }
    }

    public void OnResetShip()
    {
        DestroyImmediate(LoadedShip, true);
    }
}

#endif