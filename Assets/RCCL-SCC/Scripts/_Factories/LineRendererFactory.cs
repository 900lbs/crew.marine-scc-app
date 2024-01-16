// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 04-30-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="LineRendererFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zenject;

using Random = UnityEngine.Random;

#if SCC_2_5
public class LineRendererFactory : PlaceholderFactory<NewLineRendererSave, LineBehavior>
{
    #region Injection Construction
    DeckManager deckManager;

    [Inject]
    public LineRendererFactory(DeckManager deckMan)
    {
        deckManager = deckMan;
    }
    #endregion

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Instantiates a new LineRenderer in world space with all correct information.
    /// </summary>
    /// <param name="newSave">LineBehaviour's required data for instantiation.</param>
    /// <returns></returns>
    public override LineBehavior Create(NewLineRendererSave newSave)
    {
        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
        Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log("Creating new Line Behaviour");

        // Create the new instance from our placeholder factory, this returns a new injected instantiation.
        LineBehavior newBehaviour = base.Create(newSave);

        /*------------------------------------------------------------------------------------------------*/
        /// Make sure our ID is not 0, if it is then this is obviously being created locally, assign the ID here.
        if (newSave.ID == 0)
        {
            short key = (short)Random.Range(-32767, 32767);
            newSave.ID = key;
            newBehaviour.PlayerID = NetworkClient.GetUserName().ToString();
        }

        newBehaviour.lineRendSave = newSave;
        newBehaviour.StorageType = NetworkStorageType.LineRenderer;
        newBehaviour.PlayerID = newSave.PlayerID;
        newBehaviour.lineRendSave.PlayerID = newSave.PlayerID;
        newBehaviour.ID = newSave.ID;
        newBehaviour.name = newSave.PrefabName;

        /*------------------------------------------------------------------------------------------------*/
        // Get our parent via deck

        UserNameID user;
        Enum.TryParse(newSave.PlayerID, out user);

        RectTransform parentTransform = deckManager.GetHolder(newSave.Props, newSave.DeckID, user);
        newBehaviour.transform.SetParent(parentTransform, false);

        if (newBehaviour.gameObject == null)
        {
            Debug.LogError("NewBehaviour does not have a gameobject.", newBehaviour);
            return null;
        }

        if (newBehaviour.gameObject.GetComponent<RectTransform>() == null)
        {
            Debug.LogError("NewBehaviour does not have a rect transform.", newBehaviour);
            return null;
        }

        if (parentTransform == null)
        {
            Debug.LogError("Newbehaviour's parent is null.", newBehaviour);
            return null;
        }
        newBehaviour.gameObject.GetComponent<RectTransform>().position = parentTransform.position;

        /*------------------------------------------------------------------------------------------------*/
        // Convert and set the line renderer's color.
        Color convertedColor;
        string formattedColor = "#" + newSave.Color;
        ColorUtility.TryParseHtmlString(formattedColor, out convertedColor);
        if (newSave.IsHighlighter == "True")
        {
            newBehaviour.gameObject.AddComponent<UIMultiplyEffect>();
            convertedColor = HighLighterColor(convertedColor);
        }
        newBehaviour.lineRenderer.color = convertedColor;

        /*------------------------------------------------------------------------------------------------*/
        // Set the line thickness
        float tryThickness = 0;
        float.TryParse(newSave.Thickness.ToString(), out tryThickness);
        newBehaviour.lineRenderer.LineThickness = tryThickness;

        /*------------------------------------------------------------------------------------------------*/


        newBehaviour.lineRenderer.Points = newSave.Points;

        /*------------------------------------------------------------------------------------------------*/

        LineBehavior.NetworkObjectOnline?.Invoke(NetworkStorageType.LineRenderer, newSave.ID, newBehaviour);

        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log("Wrapping up new line renderer");
        return newBehaviour;
    }

    /*------------------------------------------------------------------------------------------------*/

    Color HighLighterColor(Color initialColor)
    {
        float H, S, V;

        Color.RGBToHSV(initialColor, out H, out S, out V);

        S = S - 0.4f;

        initialColor = Color.HSVToRGB(H, S, V);

        initialColor.a = .75f;

        return initialColor;
    }
}
#endif