using System.Threading;
// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="ShipFeature.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5
[RequireComponent(typeof(Button))]
/// <summary>
/// The individual class for all features to be populated.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />

public class ShipFeature : UI_Button, IDynamicShipComponent
{
    #region Injection Construction
    /// <summary>
    /// The UI manager
    /// </summary>
    UIManager uiManager;
    /// <summary>
    /// The deck manager
    /// </summary>
    DeckManager deckManager;
    /// <summary>
    /// The ship manager
    /// </summary>
    ShipManager shipManager;

    XMLWriterDynamic.Factory xmlWriterFactory;

    CancellationTokenSource cts;

    /// <summary>
    /// Constructs the specified UI man.
    /// </summary>
    /// <param name="uiMan">The UI man.</param>
    /// <param name="deckMan">The deck man.</param>
    /// <param name="shipMan">The ship man.</param>
    [Inject]
    public void Construct(UIManager uiMan,
        DeckManager deckMan,
        ShipManager shipMan,
        XMLWriterDynamic.Factory xmlWriterFact,
        CancellationTokenSource c)
    {
        uiManager = uiMan;
        deckManager = deckMan;
        shipManager = shipMan;
        xmlWriterFactory = xmlWriterFact;
        cts = c;
    }

    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/
    [Tooltip("This is used for feature buttons already in the scene, these are ever persistent between ships.")]
    public FeatureTool OptionalFeatureTool;
    /// <summary>
    /// The feature data
    /// </summary>
    public ShipFeatureData FeatureData;

    /// <summary>
    /// The feature name text
    /// </summary>
    public TextMeshProUGUI FeatureNameText;

    /// <summary>
    /// The color change objects
    /// </summary>
    public ColorProfile[] ColorChangeObjects;

    /// <summary>
    /// The feature prefabs
    /// </summary>
    public List<GameObject> FeaturePrefabs;

    public ToggleSwitch OptionalToggleSwitch;

    public XMLWriterDynamic XmlWriter;

    AssetBundle assetBundleReference;

    [ReadOnly]
    public string assetBundle;

    [ReadOnly]
    public string assetName;

    [SerializeField] bool selectedOnStart;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    void OnValidate()
    {
        for (int i = 0; i < ColorChangeObjects.Length; i++)
        {
            ColorChangeObjects[i].SetColorsFromSettings();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    protected override void Awake()
    {
        base.Awake();

        if (OptionalFeatureTool != FeatureTool.None)
        {
            _signalBus.Subscribe<Signal_ShipManager_OnFeatureToolCreated>(FeatureToolCreated);
            _signalBus.Subscribe<Signal_MainMenu_OnShipInitialized>(ShipInitialized);
        }

        if (OptionalToggleSwitch)
            button.onClick.AddListener(OptionalToggleSwitch.Toggle);
    }
    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnEnable()
    {
        _signalBus.Subscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    void OnDisable()
    {
        _signalBus.TryUnsubscribe<Signal_ProjectManager_OnShipReset>(OnResetShip);
    }

    /// <summary>
    /// Called when [disable].
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (OptionalToggleSwitch)
            button.onClick.RemoveListener(OptionalToggleSwitch.Toggle);

        if (OptionalFeatureTool != FeatureTool.None)
        {
            _signalBus.TryUnsubscribe<Signal_ShipManager_OnFeatureToolCreated>(FeatureToolCreated);
            _signalBus.TryUnsubscribe<Signal_MainMenu_OnShipInitialized>(ShipInitialized);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void FeatureToolCreated(Signal_ShipManager_OnFeatureToolCreated signal)
    {
        if (signal.Tool == OptionalFeatureTool)
        {
            AssignFeature(signal.Data);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void ShipInitialized(Signal_MainMenu_OnShipInitialized signal)
    {
        transform.parent.gameObject.SetActive(FeatureData.Overlays.Length > 0);

        if (selectedOnStart)
        {
            State = ActiveState.Selected;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected async override void OnStateChange()
    {
        for (int i = 0; i < ColorChangeObjects.Length; i++)
        {
            switch (State)
            {
                case ActiveState.Disabled:
                    return;

                case ActiveState.Enabled:
                    ColorChangeObjects[i].StateChange(ActiveState.Enabled);
                    FeatureManager.AddOrRemoveFromActiveFeaturesList(this, false);
                    await XmlWriter.AttemptCustomSave(FeatureData.FeatureName + ":Off");
                    await XmlWriter.Save();
                    break;

                case ActiveState.Selected:
                    ColorChangeObjects[i].StateChange(ActiveState.Selected);
                    FeatureManager.AddOrRemoveFromActiveFeaturesList(this, true);
                    await XmlWriter.AttemptCustomSave(FeatureData.FeatureName + ":On");
                    await XmlWriter.Save();
                    break;
            }
        }
        ToggleFeature();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        State = (State == ActiveState.Selected) ? ActiveState.Enabled : ActiveState.Selected;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Assigns all of the content to the feature.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AssignFeature(ShipFeatureData value)
    {
        if (value.OptionalFeatureTool == FeatureTool.None || value.Overlays.Length > 0)
        {
            FeatureData = value;

            FeaturePrefabs = new List<GameObject>();

            FeatureNameText.text = FeatureData.FeatureName;

            if (FeatureData.OverlayDict == null)
                FeatureData.AssignAllOverlaysToDictionary();

            FeatureManager.AddFeature(this);

            XmlWriter = xmlWriterFactory.Create(gameObject, XMLType.Overlays);
        }
        else
        {
            transform.parent.gameObject.SetActive(FeatureData.Overlays != null);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Toggles the feature.
    /// </summary>
    void ToggleFeature()
    {
        if (!cts.IsCancellationRequested)
        {
            switch (State)
            {

                /*------------------------------------------------------------------------------------------------*/

                case ActiveState.Disabled:
                    break;

                    /*------------------------------------------------------------------------------------------------*/

                case ActiveState.Enabled:
                    if (FeaturePrefabs != null)
                    {
                        DestroyOverlayPrefabs();
                    }

                    FeaturePrefabs = null;

                    /*                 button.image.color = uiManager.UI.Settings.DeselectedButtonCol;
                                    FeatureNameText.color = uiManager.UI.Settings.DeSelectedTextCol; */
                    break;

                    /*------------------------------------------------------------------------------------------------*/

                case ActiveState.Selected:
                    if (OptionalFeatureTool == FeatureTool.None) //TODO - Band-aid fix, adjust FeatureTool textures to assetbundle loading later.
                    {
                        assetBundleReference = AssetBundle.LoadFromFile(assetBundle);
                        ShipFeatureVariable requestedFeature = assetBundleReference.LoadAsset<ShipFeatureVariable>(assetName) as ShipFeatureVariable;
                        FeatureData = requestedFeature.data;
                    }

                    FeatureData.AssignAllOverlaysToDictionary();

                    int overlayCount = FeatureData.OverlayDict.Count;
                    for (int i = 0; i < overlayCount; ++i)
                    {
                        KeyValuePair<string, Sprite> s = FeatureData.OverlayDict.ElementAt(i);

                        if (deckManager.AllDecks.ContainsKey(s.Key)) //As long as our deck dictionary has a corresponding deck key
                        {
                            CreateOverlayPrefab(deckManager.AllDecks[s.Key].transform, s.Value); //Create the new overlay 
                        }
                    }
                    break;
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the overlay prefab.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="image">The image.</param>
    async void CreateOverlayPrefab(Transform value, Sprite image)
    {
        if (!cts.IsCancellationRequested)
        {
            if (FeaturePrefabs == null)
                FeaturePrefabs = new List<GameObject>();

            RectTransform newOverlay = Instantiate(shipManager.overlayPrefab, value).GetComponent<RectTransform>();
            newOverlay.SetSiblingIndex(1);
            newOverlay.GetComponent<Image>().sprite = image;
            newOverlay.name = FeatureData.FeatureName;

            FeaturePrefabs.Add(newOverlay.gameObject);

            await new WaitForEndOfFrame();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Destroys the overlay prefabs.
    /// </summary>
    void DestroyOverlayPrefabs()
    {
        for (int i = 0; i < FeaturePrefabs.Count; i++)
        {
            Destroy(FeaturePrefabs[i]);
            FeaturePrefabs.TrimExcess();
        }

        assetBundleReference?.Unload(true);
        //Resources.UnloadUnusedAssets();
    }

    public void OnResetShip()
    {
        if (assetBundleReference != null)
            assetBundleReference.Unload(true);

        //Destroy(this.gameObject);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Class Factory.
    /// Implements the <see cref="Zenject.PlaceholderFactory{UnityEngine.Object, ShipFeatureData, ShipFeature}" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{UnityEngine.Object, ShipFeatureData, ShipFeature}" />
    public class Factory : PlaceholderFactory<Object, ShipFeatureData, ShipFeature> { }

}
#endif