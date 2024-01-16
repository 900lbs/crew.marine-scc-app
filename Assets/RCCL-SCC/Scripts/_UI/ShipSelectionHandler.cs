// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-31-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-19-2019
// ***********************************************************************
// <copyright file="ShipSelectionHandler.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;

#if SCC_2_5
/// <summary>
/// Class ShipSelectionHandler.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ShipSelectionHandler : MonoBehaviour
{
    #region Injection Construction
    ShipProfileManager shipProfileManager;
    ShipSelectionButtonFactory shipSelectionButtonFactory;

    SignalBus _signalBus;

    [Inject]
    public void Construct(ShipProfileManager shipProfileMan/*,  ShipSelectionButtonFactory shipSelectionButtonFact */, SignalBus signal)
    {
        shipProfileManager = shipProfileMan;
        /* shipSelectionButtonFactory = shipSelectionButtonFact;   */ 
        _signalBus = signal;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The ship selection button prefab
    /// </summary>
    public GameObject ShipSelectionButtonPrefab;

    /// <summary>
    /// The maximum spawn per parent
    /// </summary>
    public int MaximumSpawnPerParent;

    /// <summary>
    /// The spawn parent
    /// </summary>
    [Tooltip("Leave empty if you want the buttons to spawn under this object.")]
    public Transform[] SpawnParent;

    public ShipSelectionButton[] ShipSelectButtons;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void Start()
    {
        ShipSelectButtons = GetComponentsInChildren<ShipSelectionButton>();
        
        //_signalBus.Subscribe<Signal_MainMenu_OnGameStateChanged>()
    }

    public void ButtonSelected(ShipSelectionButton shipButton)
    {
        int shipCount = ShipSelectButtons.Length;
        for (int i = 0; i < shipCount; i++)
        {
            if(ShipSelectButtons[i] != shipButton)
            {
                ShipSelectButtons[i].State = ActiveState.Enabled;
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /* public async Task WaitForLoad()
    {
        while (shipProfileManager.GetAllShipProfiles() == null)
        {
            Debug.Log("Waiting for Ship Profiles to be loaded.", this);
            await new WaitForEndOfFrame();
        }

        await SpawnUserButtons();
    } */

    /*----------------------------------------------------------------------------------------------------------------------------*/

   /*  async Task SpawnUserButtons()
    {
        foreach (var item in shipProfileManager.GetAllShipProfiles())
        {
            ShipSelectionButton button = shipSelectionButtonFactory.Create(ShipSelectionButtonPrefab, item);

            if (SpawnParent != null)
            {
                for (int i = 0; i < SpawnParent.Length; i++)
                {
                    if (SpawnParent[i].childCount < MaximumSpawnPerParent)
                    {
                        button.transform.parent = SpawnParent[i];
                    }
                }
            }
            else
            {
                button.transform.parent = transform;
            }
        }

        await new WaitForEndOfFrame();

    }
 */

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif