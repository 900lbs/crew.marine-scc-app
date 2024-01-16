// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="Button_Network.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5
/// <summary>
/// Class Button_Network.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class NetworkButton : UI_Button
{
    /// <summary>
    /// The current ship
    /// </summary>
    [Inject (Id = "CurrentShip")]
    ShipVariable currentShip;

    [Inject]
    MainMenu mainMenu;

    [Inject]
    ProjectSettings projectSettings;
    [Inject]
    Settings settings;
    [Inject]
    ProjectManager projectManager;
    /// <summary>
    /// The net action
    /// </summary>
    public NetworkAction NetworkAction;

    /// <summary>
    /// The button text
    /// </summary>
    public TextMeshProUGUI ButtonText;

    /// <summary>
    /// The profiles
    /// </summary>
    public ColorProfile[] Profiles;

    /// <summary>
    /// Called when [validate].
    /// </summary>
    private void OnValidate ()
    {
        int profileLength = Profiles.Length;
        for (int i = 0; i < profileLength; ++i)
        {
            Profiles[i].OnValidate ();
        }
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start ()
    {
        if (ButtonText == null)
            ButtonText = GetComponentInChildren<TextMeshProUGUI> ();

        State = ActiveState.Enabled;
    }

    /// <summary>
    /// Called when [enable].
    /// </summary>
    protected override void Awake ()
    {
        base.Awake ();
    }

    /// <summary>
    /// Called when [disable].
    /// </summary>
    protected override void OnDestroy ()
    {
        base.OnDestroy ();
    }

    #region Overrides
    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override async void BTN_OnClick ()
    {
        base.BTN_OnClick ();

        foreach (var item in Profiles)
        {
            item.ColorFlash (ActiveState.Enabled, ActiveState.Selected);
        }

        switch (NetworkAction)
        {
            case NetworkAction.Join:

                if (networkClient.IsMasterClient)
                {
                    if (PhotonNetwork.OfflineMode)
                    {
                        /* if (NetworkClientManager.CachedRoomPropertyList.Count > 0)
                            PhotonNetwork.CurrentRoom.SetCustomProperties(NetworkClientManager.CachedRoomPropertyList); */
                    }

                    PhotonNetwork.OfflineMode = false;
                    CreateRoom ();
                }
                else
                {
                    JoinRoom ();
                }

                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            case NetworkAction.OfflineMode:

                //mainMenu.ChangeState(GameState.Room);
                await CreateRoomOffline ();
                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            case NetworkAction.Rejoin:
                networkClient.InitializeNetworkSettings (currentShip.Ship.ID.ToString ());
                networkClient.JoinRoom (currentShip.Ship.ID);
                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            case NetworkAction.StartOver:
                PhotonNetwork.OfflineMode = false;

                if (projectSettings.MultishipEnabled)
                {
                    mainMenu.ChangeState (GameState.ShipSelection);
                    networkClient.LeaveRoom (false);
                    currentShip.ResetShip ();
                    projectManager.ForceCleanup ();
                    int scene = (settings.IsTabletUser) ? 2 : 1;
                    MultiSceneManager.LoadSceneAdditively (scene, true);
                    return;
                }

                if (PhotonNetwork.CurrentRoom == null)
                {
                    Debug.Log ("LeaveRoom successful, trying to reconnect.");
                    //networkClient.InitializeNetworkSettings(currentShip.Ship.ID.ToString());
                }
                mainMenu.IsStaticMenuVisible = true;
                mainMenu.ChangeState (GameState.UserSelection);
                break;
        }
    }
    /// <summary>
    /// /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange ()
    {
        int profileLength = Profiles.Length;
        for (int i = 0; i < profileLength; ++i)
        {
            Profiles[i].StateChange (State);
        }
    }

    #endregion

    /// <summary>
    /// Creates the room.
    /// </summary>
    void CreateRoom ()
    {
        networkClient.CreateRoom (currentShip.Ship.ID.ToString ());
    }

    async Task CreateRoomOffline ()
    {
        await networkClient.LeaveRoom (true);
        await new WaitForSecondsRealtime (.1f);
        //NetworkClient.Login();
        PhotonNetwork.OfflineMode = true;
        if (NetworkClient.CurrentUserProfile == null)
            mainMenu.ChangeState (GameState.UserSelection);
        else
        {
            networkClient.CreateRoom(currentShip.Ship.ID.ToString());
        }

    }
    /// <summary>
    /// Joins the room.
    /// </summary>
    void JoinRoom ()
    {
        networkClient.InitializeNetworkSettings (currentShip.Ship.ID.ToString ());
    }

    /// <summary>
    /// Logins this instance.
    /// </summary>
    void Login ()
    {
        //ShipNetworkManager.OnPlayerLogin?.Invoke();

        //NetworkClientManager.Login("Player " + UnityEngine.Random.Range(1000, 10000));
    }

    public new class Factory : PlaceholderFactory<Object, NetworkAction, NetworkButton> { }
}
#endif