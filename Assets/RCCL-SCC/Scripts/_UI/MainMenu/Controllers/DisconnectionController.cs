// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="DisconnectionController.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// Class DisconnectionController.
/// Implements the <see cref="_MenuController" />
/// </summary>
/// <seealso cref="_MenuController" />
public class DisconnectionController : _MenuController
{
    #region Injection Construction

    NetworkClient networkClient;
    SignalBus _signalBus;

    [Inject]
    public void Construct(NetworkClient netClient, SignalBus signal)
    {
        networkClient = netClient;
        _signalBus = signal;
    }

    #endregion

    RectTransform SmallWindowButtonParent;
    public RectTransform FullScreenButtonParent;
    TextMeshProUGUI DisconnectionText;

    public TextMeshProUGUI SmallWindowDisconnectionText;
    public TextMeshProUGUI FullScreenDisconnectionText;

    public GameObject SmallWindowMessage;
    public GameObject FullScreenMessage;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public override void Awake()
    {
        base.Awake();

        _signalBus.Subscribe<Signal_NetworkClient_OnDisconnectionOccured>(OnDisconnectionOccured);
        _signalBus.Subscribe<Signal_NetworkClient_OnJoinRoomFailed>(OnJoinRoomFailed);

        SmallWindowButtonParent = ButtonsParent;
        DisconnectionText = SmallWindowDisconnectionText;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_NetworkClient_OnDisconnectionOccured>(OnDisconnectionOccured);
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnJoinRoomFailed>(OnJoinRoomFailed);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void OnJoinRoomFailed(Signal_NetworkClient_OnJoinRoomFailed signal)
    {
        if (!NetworkClient.IsOfflineMode)
        {
            Debug.Log("Join room failed, showing disconnection panel.");
            string s = "Connection <color=#00FFED>Unsuccessful</color>." + Environment.NewLine + "UNABLE TO CONNECT TO HOST.";
            NetworkAction[] joinRoomOptions = new []
            {
                NetworkAction.StartOver,
                NetworkAction.OfflineMode
            };
            SmallWindowMessage.SetActive(true);
            FullScreenMessage.SetActive(false);
            ButtonsParent = SmallWindowButtonParent;
            DisconnectionText = SmallWindowDisconnectionText;
            PopulateButtons(s, joinRoomOptions);
            mainMenu.ChangeState(GameState.Disconnected);
            mainMenu.IsStaticMenuVisible = true;
        }
    }

    void OnDisconnectionOccured(Signal_NetworkClient_OnDisconnectionOccured signal)
    {
        if (!Photon.Pun.PhotonNetwork.OfflineMode)
        {
            Debug.Log("Disconnection occured.");

            if (mainMenu.currentGameState == GameState.Room)
            {
                SmallWindowMessage.SetActive(false);
                FullScreenMessage.SetActive(true);
                //mainMenu.IsStaticMenuVisible = false;
                ButtonsParent = FullScreenButtonParent;
                DisconnectionText = FullScreenDisconnectionText;
            }
            else
            {
                SmallWindowMessage.SetActive(true);
                FullScreenMessage.SetActive(false);
                //mainMenu.IsStaticMenuVisible = true;
                ButtonsParent = SmallWindowButtonParent;
                DisconnectionText = SmallWindowDisconnectionText;
            }

            NetworkAction[] masterClientActions;

            if (networkClient.isMasterClientCache)
            {
                masterClientActions = new []
                {
                    NetworkAction.StartOver, NetworkAction.OfflineMode
                };
            }
            else
            {
                masterClientActions = new []
                {
                    NetworkAction.StartOver,
                    NetworkAction.OfflineMode
                };
            }
            Debug.Log(UpdateDisconnectionText(signal.Cause));

            // Populate the specific buttons and text for this type of disconnect.
            PopulateButtons(UpdateDisconnectionText(signal.Cause), masterClientActions);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    string UpdateDisconnectionText(DisconnectCause cause)
    {

        switch (cause)
        {
            case DisconnectCause.DisconnectByClientLogic:
                if (NetworkClient.GetIsPlayerMasterClient())
                {
                    return "CONNECTION HAS BEEN TERMINATED";
                }

                return "HOST HAS TERMINATED THE CONNECTION";
            case DisconnectCause.ExceptionOnConnect:

                string message = "CONNECTION <color=#00FFED>UNSUCCESSFUL</color>.                       PLEASE CHECK YOUR NETWORK CONNECTION.";
                Debug.Log(message);
                return message;
            case DisconnectCause.ClientTimeout:
                return "NETWORK CONNECTION LOST, CLIENT HAS TIMED OUT";

            default:
                return "Connection Interrupted";
        }
    }

    /// <summary>
    /// Populates the specified buttons.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="actions">The actions.</param>
    void PopulateButtons(string message, NetworkAction[] actions)
    {
        Debug.Log("Populating disconnection buttons, total = " + actions.Length);
        DisconnectionText.text = message;
        SpawnButtons(actions);
        //mainMenu.ChangeState(GameState.Disconnected);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}