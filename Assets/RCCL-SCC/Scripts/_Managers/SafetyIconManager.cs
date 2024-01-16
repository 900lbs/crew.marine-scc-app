using System.Linq;
// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-09-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="SafetyIconManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Zenject;

#if SCC_2_5
/// <summary>
/// Handles shiproompropertyholder Activate/Deactivate states, changes and (optionally) dynamic spawning.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="Zenject.ILateDisposable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="Zenject.ILateDisposable" />
public class SafetyIconManager : MonoBehaviour, ILateDisposable
{
    #region Injection construction
    /// <summary>
    /// The annotation manager
    /// </summary>
    AnnotationManager annotationManager;
    /// <summary>
    /// The icon button factory
    /// </summary>
    SelectIcon.Factory iconButtonFactory;
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;

    /// <summary>
    /// Constructs the specified anno man.
    /// </summary>
    /// <param name="annoMan">The anno man.</param>
    /// <param name="iconButtonFact">The icon button fact.</param>
    /// <param name="signal">The signal.</param>
    [Inject]
    public void Construct(AnnotationManager annoMan, SelectIcon.Factory iconButtonFact, SignalBus signal)
    {
        annotationManager = annoMan;
        iconButtonFactory = iconButtonFact;
        _signalBus = signal;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion

    /// <summary>
    /// The safety icon prefab
    /// </summary>
    public GameObject SafetyIconPrefab;

    /// <summary>
    /// The icon objects
    /// </summary>
    public SortedDictionary<string, SafetyIconData> IconObjects;

    /// <summary>
    /// The icon object data list
    /// </summary>
    [ReadOnly]
    public List<SafetyIconData> IconObjectDataList;

    /// <summary>
    /// The icon holder template
    /// </summary>
    public ShipRoomPropertyHolder IconHolderTemplate;

    /// <summary>
    /// The property holders
    /// </summary>
    Dictionary<ShipRoomProperties, ShipRoomPropertyHolder> PropertyHolders;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        _signalBus.Subscribe<Signal_AnnoMan_OnRoomPropertyChanged>(AnnoMan_RoomPropertyChanged);
        _signalBus.Subscribe<Signal_AnnoMan_OnAnnotationToolsChanged>(AnnoMan_AnnotationToolsChanged);
        _signalBus.Subscribe<Signal_NetworkClient_OnClientStateChanged>(NetworkClient_OnClientStateChanged);

        PropertyHolders = new Dictionary<ShipRoomProperties, ShipRoomPropertyHolder>();

        ShipRoomPropertyHolder[] holders = GetComponentsInChildren<ShipRoomPropertyHolder>();
        int holderCount = holders.Length;

        for (int i = 0; i < holderCount; ++i)
        {
            PropertyHolders.Add(holders[i].AssignedProperty, holders[i]);
        }

        IconObjects = new SortedDictionary<string, SafetyIconData>();
        IconObjectDataList = new List<SafetyIconData>();
        SafetyIconData[] resourceIcons = Resources.LoadAll<SafetyIconData>("ScriptableObjects/SafetyIcons");
        int resourceIconsCount = resourceIcons.Length;

        for (int i = 0; i < resourceIconsCount; ++i)
        {
            IconObjects.Add(resourceIcons[i].IconName, resourceIcons[i]);
            IconObjectDataList.Add(resourceIcons[i]);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Lates the dispose.
    /// </summary>
    public void LateDispose()
    {
        _signalBus.Unsubscribe<Signal_NetworkClient_OnClientStateChanged>(NetworkClient_OnClientStateChanged);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnRoomPropertyChanged>(AnnoMan_RoomPropertyChanged);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnAnnotationToolsChanged>(AnnoMan_AnnotationToolsChanged);

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Networks the client on client state changed.
    /// </summary>
    /// <param name="state">The state.</param>
    void NetworkClient_OnClientStateChanged(Signal_NetworkClient_OnClientStateChanged state)
    {
        if (state.NewState == ClientState.Joined)
        {
            //Uncomment for auto button spawning.
            //CreateSafetyIconButton();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Listens for tools state update, checks which flags are active and activates/deactivates accordingly.
    /// </summary>
    /// <param name="tools">The tools.</param>
    void AnnoMan_AnnotationToolsChanged(Signal_AnnoMan_OnAnnotationToolsChanged tools)
    {
        if (tools.ActiveTools.HasFlag(ActiveAnnotationTools.Art))
        {
            CheckActiveProperty(annotationManager.GetCurrentPropertyState());
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the safety icon button.
    /// </summary>
    void CreateSafetyIconButton()
    {
        foreach (var item in IconObjects)
        {

            if (item.Value == null)
                return;

            if ((!item.Value.AssignedShips.HasFlag(NetworkClient.GetCurrentShip())))
                return;

            SelectIcon newIcon = iconButtonFactory.Create(SafetyIconPrefab, item.Value);
            AddToHolder(newIcon.IconData.AssignedProperties, newIcon.gameObject);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Annoes the man room property changed.
    /// </summary>
    /// <param name="signal">The signal.</param>
    void AnnoMan_RoomPropertyChanged(Signal_AnnoMan_OnRoomPropertyChanged signal)
    {
        CheckActiveProperty(signal.roomProperty);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Checks the active property.
    /// </summary>
    /// <param name="prop">The property.</param>
    void CheckActiveProperty(ShipRoomProperties prop)
    {
        foreach (var item in PropertyHolders)
        {
            item.Value.gameObject.SetActive(item.Value.AssignedProperty.HasFlag(prop));
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the icon data.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>SafetyIconData.</returns>
    public SafetyIconData GetIconData(string query)
    {
        try
        {
            return IconObjects[query];
        }
        catch
        {
            Debug.LogError("Icon data query '" + query + "' not found.");
            return null;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Adds the icon object.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddIconObject(SafetyIconData value)
    {
        if (!IconObjects.ContainsKey(value.IconName))
        {
            IconObjects.Add(value.IconName, value);
        }
        else
        {
            Debug.LogError((IconObjects.ContainsKey(value.IconName) ?
            "Icon Manager already has " + value.IconName + " added." :
            "IconManager could not accept " + value.IconName + " for some reason, fix your shit."));
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Removes the icon object.
    /// </summary>
    /// <param name="value">The value.</param>
    public void RemoveIconObject(SafetyIconData value)
    {
        if (IconObjects.ContainsKey(value.IconName))
        {
            IconObjects.Remove(value.IconName);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Adds to holder.
    /// </summary>
    /// <param name="props">The props.</param>
    /// <param name="value">The value.</param>
    void AddToHolder(ShipRoomProperties props, GameObject value)
    {
        foreach (var item in PropertyHolders)
        {
            if (item.Key.HasFlag(props))
            {
                item.Value.AddNewObject(value);
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif