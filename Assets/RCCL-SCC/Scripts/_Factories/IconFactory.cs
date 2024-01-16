// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 05-10-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-21-2019
// ***********************************************************************
// <copyright file="IconFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

#if SCC_2_5
/// <summary>
/// Handles the standardized creation of icons.
/// </summary>
/// <typeparam name="Object"></typeparam>
/// <typeparam name="NewIconSave"></typeparam>
/// <typeparam name="IconBehavior"></typeparam>
public class IconFactory : IFactory<Object, NewIconSave, IconBehavior>
{
    #region Injection Construction
    DeckManager deckManager;
    readonly DiContainer _container;
    readonly SafetyIconManager safetyIconManager;

    [Inject]
    public IconFactory(DeckManager deckMan, DiContainer container, SafetyIconManager iconMan)
    {
        deckManager = deckMan;
        _container = container;
        safetyIconManager = iconMan;

    }

    #endregion

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Instantiates a new icon in world space.
    /// </summary>
    /// <param name="prefab">The prefab we're spawning from resources.</param>
    /// <param name="newSave">Save data, this can either be from the network or constructed manually before creation.</param>
    /// <returns></returns>
    public IconBehavior Create(Object prefab, NewIconSave newSave)
    {
        try
        {
            if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                Debug.Log("<color=yellow> Spinning IconFactory up, building: </color>" + newSave.PrefabName);

            if (prefab == null)
                return null;

            IconBehavior newIcon = _container.InstantiatePrefabForComponent<IconBehavior>(prefab);

            newIcon.IconData = safetyIconManager.GetIconData(newSave.PrefabName);
            newIcon.IconSave = newSave;
            newIcon.StorageType = newIcon.StorageType | NetworkStorageType.Icon;
            /*------------------------------------------------------------------------------------------------*/
            /// Make sure our ID is not 0, if it is then this is obviously being created locally, assign the ID here.
            if (newSave.ID == 0)
            {
                short key = (short)Random.Range(-32767, 32767);
                newSave.ID = key;
                newSave.PlayerID = NetworkClient.GetUserName().ToString();
            }

            newIcon.PlayerID = newSave.PlayerID;
            newIcon.ID = newSave.ID;
            newIcon.PlayerID = newSave.PlayerID;
            newIcon.name = newSave.PrefabName;
            newIcon.IconText = newSave.Text;

            /*------------------------------------------------------------------------------------------------*/
            // Get our parent via deck

            RectTransform iconTransform = newIcon.GetComponent<RectTransform>();

            UserNameID user;
            Enum.TryParse(newIcon.IconSave.PlayerID, out user);

            RectTransform parentTransform = deckManager.GetHolder(newIcon.IconSave.Prop, newIcon.IconSave.DeckID, user);

            iconTransform.SetParent(parentTransform, false);

            iconTransform.anchoredPosition = newSave.CurrentPosition;
            //iconTransform.localPosition = new Vector3(iconTransform.localPosition.x, iconTransform.localPosition.y, 0);
            iconTransform.localScale = newIcon.IconData.GetScaleFromPrefab();
            

            //Debug.Log("<color=cyan>New icon created at vector: " + iconTransform.anchoredPosition + "</color>");

            /*------------------------------------------------------------------------------------------------*/
            // 
            newIcon.Initialize();
            IconBehavior.NetworkObjectOnline?.Invoke(NetworkStorageType.Icon, newSave.ID, newIcon);
            return newIcon;
        }
        catch (ArgumentException arg)
        {
            Debug.LogError("IconFactory failed to create an icon.");
            throw new Exception("IconFactory failed to create an icon,", arg);
        }
    }
}
#endif