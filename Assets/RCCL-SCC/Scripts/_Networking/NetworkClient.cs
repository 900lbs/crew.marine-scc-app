// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="NetworkClient.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zenject;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.IO;
using System.Threading;

/// <summary>
/// Our networking public accessor class, has everything any class will/would need for networking functionality.
/// Implements the <see cref="INetworkClient" />
/// Implements the <see cref="Zenject.IInitializable" />
/// </summary>
/// <seealso cref="INetworkClient" />
/// <seealso cref="Zenject.IInitializable" />
[System.Serializable]
public class NetworkClient : INetworkClient, IInitializable
{
    #region Injection Setup

    //Readonly reference to our networkmanager, this is assigned via injection in our class constructor.
    /// <summary>
    /// The network manager
    /// </summary>
    private readonly NetworkClientManager networkManager;

    /// <summary>
    /// The signal
    /// </summary>
    private readonly SignalBus _signalBus;

    private readonly NetworkRoomPropertyHandler roomPropertyHandler;

    private readonly ProjectSettings projectSettings;

    /// <summary>
    /// The save ga edits
    /// </summary>
    //readonly BuildSaveGAEdits saveGAEdits;

    //[Inject]
    //MainMenu mainMenu;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkClient"/> class.
    /// </summary>
    /// <param name="networkMan">The network man.</param>
    /// <param name="signal">The signal.</param>
    [Inject]
    public NetworkClient(NetworkClientManager networkMan,
        NetworkRoomPropertyHandler roomPropertyHand,
        ProjectSettings projectSet,
        SignalBus signal)
    {
        networkManager = networkMan;
        roomPropertyHandler = roomPropertyHand;
        projectSettings = projectSet;
        _signalBus = signal;
    }

    /// <summary>
    /// The ship data
    /// </summary>
    [Inject(Id = "CurrentShip")]
    private ShipVariable shipData;

    #endregion Injection Setup

    /// <summary>
    /// Gets or sets the scene set.
    /// </summary>
    /// <value>The scene set.</value>
    public Settings.SceneSettings SceneSet
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is master client.
    /// </summary>
    /// <value><c>true</c> if this instance is master client; otherwise, <c>false</c>.</value>
    public bool IsMasterClient
    {
        get
        {
            return PhotonNetwork.IsMasterClient;
        }
    }

    public static bool IsOfflineMode
    {
        get
        {
            return PhotonNetwork.OfflineMode;
        }
        set
        {
            PhotonNetwork.OfflineMode = value;
        }
    }

    /// <summary>
    /// The previous network object sent
    /// </summary>
    private NetworkAnnotationObject previousNetworkObjectSent;

    /// <summary>
    /// The active users
    /// </summary>
    public UserNameID ActiveUsers = 0;

    public static UserProfile CurrentUserProfile;

    /// <summary>
    /// Is only ever set and cached if the user is a/the MasterClient, this is mainly used in case of
    /// disconnection (since by the time we find out that we've been disconnected, the user will always
    /// return masterClient = false).
    /// </summary>
    public bool isMasterClientCache;

    /// <summary>
    /// Initialize is part of Zenject.IInitializable, this is basically called on Unity's Start() but
    /// ultimately allows us to control execution order via our Installer.
    /// </summary>
    public void Initialize()
    {
    }

    #region Actions

    /// <summary>
    /// Custom public event to let client know to create a new line renderer and all accompanying
    /// parameters.
    /// </summary>
    private Action<INetworkObject> OnNewNetworkEventReceived;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The on new ga overlay event received
    /// </summary>
    public static Action<Dictionary<string, string>> OnNewGAOverlayEventReceived;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Simple low level network state event, this is called from NetworkClientManager since
    /// this is a static class, attach scene listeners to this guy when you only want updates
    /// on when the state has changed.
    /// </summary>
    public static Action<ClientState> OnNewClientState;

    /// <summary>
    /// The on active users updated
    /// </summary>
    public static Action<UserNameID> OnActiveUsersUpdated;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion Actions

    #region Static Helpers/Accessors

    /// <summary>
    /// Gets the state of the current client.
    /// </summary>
    /// <returns>ClientState.</returns>
    public static ClientState GetCurrentClientState()
    {
        return PhotonNetwork.NetworkClientState;
    }

    public static UserNameID GetAllPossibleUserNameIDs()
    {
        UserNameID allUsers = 0;

        int userNameCount = Enum.GetValues(typeof(UserNameID)).Length;

        foreach (var item in Enum.GetValues(typeof(UserNameID)))
        {
            allUsers = allUsers | (UserNameID)item;
        }

        return allUsers;
    }

    /// <summary>
    /// Logins the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static void Login(UserProfile value)
    {
        PhotonNetwork.NickName = value.UserNameID.ToString();
        CurrentUserProfile = value;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the current ship.
    /// </summary>
    /// <returns>ShipID.</returns>
    public static ShipID GetCurrentShip()
    {
        ShipID currentShip;
        Enum.TryParse(PhotonNetwork.CurrentRoom.Name, out currentShip);

        return currentShip;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    /// <returns>System.String.</returns>
    public static UserNameID GetUserName()
    {
        UserNameID user;
        Enum.TryParse(PhotonNetwork.LocalPlayer.NickName, out user);
        return user;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the is player master client.
    /// </summary>
    /// <returns><c>true</c> if player is the masterclient, <c>false</c> otherwise.</returns>
    public static bool GetIsPlayerMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the master client identifier.
    /// </summary>
    /// <returns>System.String.</returns>
    public static string GetMasterClientID()
    {
        return PhotonNetwork.MasterClient.NickName;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Fires all active players.
    /// </summary>
    public async Task FireAllActivePlayers()
    {
        await new WaitForBackgroundThread();
        if (PhotonNetwork.CurrentRoom != null)
        {
            ActiveUsers = 0;
            List<Player> tempUsers = new List<Player>();

            Dictionary<int, Player> checkPlayers = PhotonNetwork.CurrentRoom.Players;
            tempUsers.AddRange(checkPlayers.Values);

            foreach (var activePlayer in tempUsers)
            {
                UserNameID user = 0;
                Enum.TryParse(activePlayer.NickName, out user);
                ActiveUsers = ActiveUsers | user;
            }

            await new WaitForUpdate();
            _signalBus.Fire<Signal_NetworkClient_OnActiveUsersUpdated>(new Signal_NetworkClient_OnActiveUsersUpdated(ActiveUsers, this));
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion Static Helpers/Accessors

    #region Network Client Manager PUN callbacks

    /// <summary>
    /// Should only be called by the NetworkClientManager, subscribe to the Signal_NetworkClient_OnClientStateChanged
    /// signal to get state changes.
    /// </summary>
    /// <param name="state">The state.</param>
    public void ClientStateChanged(ClientState state)
    {
        OnNewClientState?.Invoke(state);
        _signalBus.Fire(new Signal_NetworkClient_OnClientStateChanged(state));
    }

    /// <summary>
    /// Called when [connected to master server].
    /// </summary>
    public async void OnConnectedToMasterServer()
    {
        if (PhotonNetwork.OfflineMode)
            _signalBus.Fire<Signal_NetworkClient_OnOnlineOfflineToggled>(new Signal_NetworkClient_OnOnlineOfflineToggled(false));
        else
        {
            _signalBus.Fire<Signal_NetworkClient_OnOnlineOfflineToggled>(new Signal_NetworkClient_OnOnlineOfflineToggled(true));
        }

        await new WaitForEndOfFrame();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void OnDisconnected(DisconnectCause cause)
    {
        _signalBus.Fire<Signal_NetworkClient_OnDisconnectionOccured>(new Signal_NetworkClient_OnDisconnectionOccured(cause));
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when the table creates a room, this function is responsible for setting up all of our saved data
    /// such as annotations, timers etc.
    /// </summary>
    public async Task OnCreatedRoom()
    {
        //await saveGAEdits.LoadPrefabs();
        await UpdateRoomProperty((byte)ShipRoomProperties.GAOverlay);
        await UpdateRoomProperty((byte)ShipRoomProperties.Annotations);
        //await UpdateRoomProperty((byte) ShipRoomProperties.KillCards);
        //await NetworkRoomPropertyHandler.SpawnAllProperties(this);
        isMasterClientCache = true;
        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log("Created room with " + PhotonNetwork.CurrentRoom.CustomProperties.Count + " properties.");
        await new WaitForEndOfFrame();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [joined room].
    /// </summary>
    public async void OnJoinedRoom()
    {
        //Check if we're SCC, takeover masterclient if needed.
        if (CurrentUserProfile.UserNameID == UserNameID.SafetyCommandCenter && !IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetMasterClient(PhotonNetwork.LocalPlayer);
            Debug.Log("MasterClient was set to this player.");
        }

        if (!IsMasterClient)
        {
            Debug.Log("Joined Room " + PhotonNetwork.CurrentRoom.Name);
            RoomPropertiesUpdated(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        await FireAllActivePlayers();

        _signalBus.Fire<Signal_MainMenu_OnShipInitialized>(new Signal_MainMenu_OnShipInitialized());
        _signalBus.Fire<Signal_NetworkClient_OnOnlineOfflineToggled>(new Signal_NetworkClient_OnOnlineOfflineToggled(!PhotonNetwork.OfflineMode));

        await new WaitForEndOfFrame();
        await NetworkRoomPropertyHandler.SpawnAllProperties(this);

        Debug.Log("Joined room.");
    }

    /// <summary>
    /// Called when [join room failed].
    /// </summary>
    public void OnJoinRoomFailed()
    {
        _signalBus.Fire<Signal_NetworkClient_OnJoinRoomFailed>(new Signal_NetworkClient_OnJoinRoomFailed());
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Updates the room property.
    /// </summary>
    /// <param name="key">The key.</param>
    public async Task UpdateRoomProperty(byte key, params object[] optionalParameters)
    {
        switch ((ShipRoomProperties)key)
        {
            #region GAOverlay

            case ShipRoomProperties.GAOverlay:
                ClearRoomProperty((ShipRoomProperties)key);
                object newKey = key;

                string file = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/GAOverlay.csv");

                if (file.Length == 0)
                {
                    Debug.Log("No GA Edits found.");
                    return;
                }
                Dictionary<string, string> propDict = new Dictionary<string, string>();

                List<string> rowLines = file.Split('\n').ToList();
                Dictionary<string, List<string>> suffixList = new Dictionary<string, List<string>>();

                for (int i = 0; i < rowLines.Count; i++)
                {
                    if (rowLines[i] != "")
                    {
                        string convertedData = rowLines[i];
                        string[] convertedLine = convertedData.Split(","[0]);
                        List<string> getCurrentList = new List<string>();

                        //Debug.Log("<color=red> Attempting to check if Suffix is already created.</color>");
                        if ((!suffixList.TryGetValue(convertedLine[4], out getCurrentList)))
                        {
                            getCurrentList = new List<string>();
                            //Debug.Log("<color=red> Suffix was not found, adding suffix with data: " + convertedData + "</color>");
                            getCurrentList.Add(convertedData);
                            suffixList.Add(convertedLine[4], getCurrentList);
                        }
                        else
                        {
                            //Debug.Log("<color=red> Found a pre-existing suffix, adding: " + convertedData + "</color>");
                            suffixList[convertedLine[4]].Add(convertedData);
                        }

                        //        propDict.Add(i.ToString(), convertedData);
                    }
                }

                foreach (var item in suffixList)
                {
                    short index = 0;

                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        propDict.Add(i.ToString(), item.Value[i]);
                        index++;
                    }

                    networkManager.CreateOrUpdateRoomProperty(NetworkEvent.Create, ShipRoomProperties.GAOverlay, item.Key, index, null, propDict);
                    propDict.Clear();
                }

                break;

            #endregion GAOverlay

            #region Annotations

            case ShipRoomProperties.Annotations:
                Dictionary<string, string> annoPropDict = new Dictionary<string, string>();
                //if (NetworkClientManager.GetRoomProperty(ShipRoomProperties.Annotations) == null)
                networkManager.CreateOrUpdateRoomProperty(NetworkEvent.Create, ShipRoomProperties.Annotations, "", (short)key);
                break;

            #endregion Annotations

            #region KillCards

            case ShipRoomProperties.KillCards:
                List<KillCardClass> killCardClasses = new List<KillCardClass>();
                Dictionary<string, string> properties = new Dictionary<string, string>();
                if (optionalParameters != null)
                {
                    // As long as there are killcards..
                    if (optionalParameters.Length > 0)
                    {
                        try
                        {
                            await new WaitForBackgroundThread();
                            killCardClasses = (List<KillCardClass>)optionalParameters[0]; // Convert
                            if (killCardClasses.Count > 0) //Probably redundant
                            {
                                int count = killCardClasses.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    string killCardTitle = killCardClasses[i].title; // Our key
                                    string killCardVersionID = killCardClasses[i].VersionID; // Our value

                                    if (!properties.ContainsKey(killCardTitle))
                                    {
                                        properties.Add(killCardTitle, killCardVersionID);// Add it if it's not already.
                                    }
                                    else
                                    {
                                        await new WaitForUpdate();
                                        Debug.LogError("KillCard was already found in the RoomProperties list: " + killCardTitle);
                                        await new WaitForBackgroundThread();
                                    }
                                }
                            }

                            networkManager.CreateOrUpdateRoomProperty(NetworkEvent.Create, ShipRoomProperties.KillCards, "", (short)key, null, properties);

                            await new WaitForUpdate();
                            break;
                        }
                        catch (System.Exception e)
                        {
                            await new WaitForUpdate();
                            Debug.LogError(e.Source + " / " + e.InnerException + " / " + e.StackTrace);
                            throw;
                        }
                    }
                }
                break;

            #endregion KillCards

            default:

                break;
        }
    }

    public void ClearRoomProperty(ShipRoomProperties prop)
    {
        string convertedProp = prop.ToString();
        List<Hashtable> newProperties = new List<Hashtable>();
        Hashtable cachedProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        foreach (var item in cachedProperties)
        {
            string convertedPropKey = (string)item.Key;

            if (convertedPropKey.StartsWith(convertedProp))
            {
                Hashtable resetTable = new Hashtable();
                Dictionary<string, string> resetDict = new Dictionary<string, string>();
                resetTable.Add(item.Key, resetDict);
                newProperties.Add(resetTable);
            }
        }

        for (int i = 0; i < newProperties.Count; i++)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties[i]);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called on everyone but only updates if the player is a tablet, or "client", since the table is the only
    /// device that can update the saved edits, it doesn't need to worry about this function. This is only necessary
    /// in realtime if it's a property similar to GAOveralys where we send one massive update each time it's changed
    /// instead of individual events.
    /// </summary>
    /// <param name="propertiesThatChanged">A table of properties that were updated.</param>
    public void RoomPropertiesUpdated(Hashtable propertiesThatChanged)
    {
        if (!PhotonNetwork.IsMasterClient || networkManager.LocalDebuggingEnabled == true)
        {
            string prop = ShipRoomProperties.GAOverlay.ToString();

            foreach (var item in propertiesThatChanged)
            {
                string convertedKey = (string)item.Key;

                if (convertedKey.StartsWith(prop))
                {
                    Debug.Log("GA property found: " + convertedKey);
                    var dataDict = item.Value as Dictionary<string, string>;
                    OnNewGAOverlayEventReceived?.Invoke(dataDict);
                }
            }

            /*             if (propertiesThatChanged.ContainsKey(prop))
                        {
                            var dataDict = propertiesThatChanged[prop] as Dictionary<string, string>;
                            OnNewGAOverlayEventReceived?.Invoke(dataDict);
                        } */
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [player joined room].
    /// </summary>
    /// <param name="player">The player.</param>
    public async void OnPlayerJoinedRoom(Player player)
    {
        Debug.Log(player.NickName + " joined the room.");

        await FireAllActivePlayers();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [player left room].
    /// </summary>
    /// <param name="player">The player.</param>
    public async void OnPlayerLeftRoom(Player player)
    {
        await FireAllActivePlayers();
        //_signalBus.Fire<Signal_NetworkClient_OnActiveUsersUpdated>(new Signal_NetworkClient_OnActiveUsersUpdated(ActiveUsers, this));
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion Network Client Manager PUN callbacks

    /// <summary>
    /// Send a creation request to the network in order to let all other clients know that a new annotation
    /// should be created.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>

    public void UpdateNetworkSettings(string ipAddress)
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = ipAddress;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes the network settings.
    /// </summary>
    /// <param n`ame="roomID">The room identifier.</param>
    public bool InitializeNetworkSettings(string roomID)
    {
        return networkManager.InitializeNetworkManager();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the room.
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool CreateRoom(string roomID)
    {
        RoomOptions options = new RoomOptions();
        options.PlayerTtl = 60;
        options.EmptyRoomTtl = 0;
        options.PublishUserId = true;
        options.IsOpen = true;
        options.IsVisible = true;
        options.MaxPlayers = 16;

        string room = (projectSettings.DevEnabled) ? roomID + "DEV" : roomID;

        //options.CustomRoomProperties = new Hashtable();
        /*string gaOverlays = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/GAOverlay.csv");
        Dictionary<string, string> newGADict = new Dictionary<string, string>();
        newGADict.Add(ShipRoomProperties.GAOverlay.ToString(), gaOverlays); */
        //options.CustomRoomProperties.Add(ShipRoomProperties.GAOverlay.ToString(), "");

        PhotonNetwork.AutomaticallySyncScene = true;

        return PhotonNetwork.JoinOrCreateRoom(room, options, null);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public bool CheckIfRoomIsOnline(ShipID ship)
    {
        return PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, ship.ToString());
    }

    /// <summary>
    /// Joins the room, this returns a bool in order to align with PUN's architecture, mean't to throw an error in the case of
    /// a joined room failed (in which, JoinRoomFailed() is called).
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool JoinRoom(string roomID)
    {
        string room = (projectSettings.DevEnabled) ? roomID + "DEV" : roomID;
        Debug.Log("Joining room " + room);

        return networkManager.JoinRoom(room);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public bool JoinRoom(ShipID roomID)
    {
        Debug.Log("Joining room.");
        return networkManager.JoinRoom(roomID.ToString());
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public async Task<bool> LeaveRoom(bool leaveMaster)
    {
        if (leaveMaster)
        {
            PhotonNetwork.Disconnect();
            return true;
        }
        else
        {
            ActiveUsers = 0;
            _signalBus.Fire<Signal_NetworkClient_OnActiveUsersUpdated>(new Signal_NetworkClient_OnActiveUsersUpdated(ActiveUsers, this));
            return PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// Alerts all listeners that a new network event has been received and the object as it's base class.
    /// </summary>
    /// <param name="netObject">The received event object at it's base class, listeners must check for type themselves, this keeps everything
    /// nice and generic.</param>
    public async Task SendNewNetworkEventReceived(INetworkObject netObject)
    {
        OnNewNetworkEventReceived?.Invoke(netObject); //Pre-Zenject, need to convert at some point to the new signal flow.

        if (projectSettings.DebugEnabled)
            Debug.Log("New network object event: " + netObject.GetType() + " received from player: " + netObject.PlayerID);

        _signalBus.Fire<Signal_NetworkClient_OnNetworkEventReceived>(new Signal_NetworkClient_OnNetworkEventReceived(netObject));

        await new WaitForEndOfFrame();
    }

    /// <summary>
    /// Sends a new network event, as long as the data being sent is inherited from INetworkObject and we check the class here,
    /// everything else is taken care of.
    /// </summary>
    /// <param name="netObject">The net object.</param>
    public async Task SendNewNetworkEvent(INetworkObject netObject)
    {
        if (netObject.GetType() == typeof(NetworkAnnotationObject))
        {
            NetworkAnnotationObject convertedObject = (NetworkAnnotationObject)netObject;

            await SendNetworkAnnotationObject(convertedObject);
            await new WaitForUpdate();
            return;
        }

        if (netObject.GetType() == typeof(NetworkShareObject))
        {
            NetworkShareObject convertedShareObject = (NetworkShareObject)netObject;

            await SendNetworkShareObject(convertedShareObject);
            await new WaitForUpdate();
            return;
        }

        if (netObject.GetType() == typeof(NetworkKillCardObject))
        {
            NetworkKillCardObject convertedTextureObject = (NetworkKillCardObject)netObject;

            await SendNetworkKillCardObject(convertedTextureObject);
            await new WaitForUpdate();
            return;
        }

        if (netObject.GetType() == typeof(NetworkKillCardRequestObject))
        {
            NetworkKillCardRequestObject convertedRequestObject = (NetworkKillCardRequestObject)netObject;
            if (!IsMasterClient)
                convertedRequestObject.RequestingUsernameID = GetUserName().ToString();
            await SendNetworkKillCardRequestObject(convertedRequestObject);
            await new WaitForUpdate();
            return;
        }
        else if (netObject.GetType() == typeof(NetworkCheckmarkObject))
        {
            NetworkCheckmarkObject convertedCheckmarkObject = (NetworkCheckmarkObject)netObject;

            await SendNetworkCheckmarkObject(convertedCheckmarkObject);
            await new WaitForUpdate();
            return;
        }

        await new WaitForUpdate();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Add a listener to whichever events that it's selected interface(s) dictate.
    /// </summary>
    /// <param name="value">The object itself.</param>
    public void AddListener(object value)
    {
        IShipNetworkEvents actions = value as IShipNetworkEvents;

        if (actions != null && !networkManager.NetworkActionsListeners.Contains(actions))
        {
            OnNewNetworkEventReceived += actions.OnNetworkActionEvent;
            networkManager.NetworkActionsListeners.Add(actions);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Remove a listener from whichever events that it's selected interface(s) dictate.
    /// </summary>
    /// <param name="value">The object itself</param>
    public void RemoveListener(object value)
    {
        IShipNetworkEvents actions = value as IShipNetworkEvents;

        if (actions != null && networkManager.NetworkActionsListeners.Contains(actions))
        {
            OnNewNetworkEventReceived -= actions.OnNetworkActionEvent;
            networkManager.NetworkActionsListeners.Remove(actions);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sends the network annotation object.
    /// </summary>
    /// <param name="value">The value.</param>
    private async Task SendNetworkAnnotationObject(NetworkAnnotationObject value)
    {
        if (value == previousNetworkObjectSent)
        {
            //Debug.Log("<color=red> Duplicate event found, returning now.</color>");
            return;
        }

        previousNetworkObjectSent = value;

        /*----------------------------------------------------------------------------------------------------------------------------*/

        byte id = (byte)value.NetEvent;

        var message = value.NetData;

        object[] messageArray = await value.ConvertToNetworkObject();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var eventOptions = new RaiseEventOptions();
        eventOptions.CachingOption = EventCaching.DoNotCache;
        eventOptions.Receivers = (networkManager.LocalDebuggingEnabled) ? ReceiverGroup.All : ReceiverGroup.Others;

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;

        /*----------------------------------------------------------------------------------------------------------------------------*/
        if (PhotonNetwork.RaiseEvent(id, messageArray, eventOptions, sendOptions))
        {
            //Debug.Log("Sending new event: " + value.NetEvent + " / " + value.NetOptionalRoomProp + " / " + value.NetData);
        }
        else
        {
            Debug.LogError("Could not send event: " + value.NetEvent + " / " + value.NetOptionalRoomProp + " / " + value.NetData);
        }

        /*----------------------------------------------------------------------------------------------------------------------------*/
        //Attempt to update the room property if there is one.
        try
        {
            if (value.NetOptionalRoomProp != ShipRoomProperties.None)
                networkManager.CreateOrUpdateRoomProperty(value.NetEvent, value.NetOptionalRoomProp, "", value.NetID, value.NetData.ToString());
        }
        catch (System.Exception e)
        {
            string errorEventCompiled = "";
            foreach (var item in messageArray)
            {
                errorEventCompiled = errorEventCompiled + item + " / ";
            }

            Debug.LogError("ERROR sending new annotation:" + errorEventCompiled + " --> " + e.ToString());
            throw;
        }

        await new WaitForUpdate();
    }

    /// <summary>
    /// Sends the network share object.
    /// </summary>
    /// <param name="value">The value.</param>
    private async Task SendNetworkShareObject(NetworkShareObject value)
    {
        byte id = (byte)value.NetEvent;

        var message = value.NetData;

        object[] messageArray = await value.ConvertToNetworkObject();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var eventOptions = new RaiseEventOptions();
        eventOptions.CachingOption = EventCaching.DoNotCache;
        eventOptions.Receivers = (networkManager.LocalDebuggingEnabled) ? ReceiverGroup.All : ReceiverGroup.Others;

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;

        try
        {
            PhotonNetwork.RaiseEvent(id, messageArray, eventOptions, sendOptions);
        }
        catch (System.Exception)
        {
            return;
        }
        await new WaitForUpdate();
    }

    private async Task SendNetworkKillCardObject(NetworkKillCardObject value)
    {
        byte id = (byte)value.NetEvent;

        var message = value.NetData;

        object[] messageArray = await value.ConvertToNetworkObject();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var eventOptions = new RaiseEventOptions();
        eventOptions.CachingOption = EventCaching.DoNotCache;
        eventOptions.Receivers = (networkManager.LocalDebuggingEnabled) ? ReceiverGroup.All : ReceiverGroup.Others;

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;

        PhotonNetwork.RaiseEvent(id, messageArray, eventOptions, sendOptions);

        await new WaitForUpdate();
        await SendImageChunks(value, messageArray);
    }

    private async Task SendNetworkKillCardRequestObject(NetworkKillCardRequestObject value)
    {
        byte id = (byte)value.NetEvent;

        var message = value.NetData;

        object[] messageArray = await value.ConvertToNetworkObject();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var eventOptions = new RaiseEventOptions();
        eventOptions.CachingOption = EventCaching.DoNotCache;
        eventOptions.Receivers = (IsMasterClient) ? ReceiverGroup.Others : ReceiverGroup.MasterClient;

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;

        PhotonNetwork.RaiseEvent(id, messageArray, eventOptions, sendOptions);

        await new WaitForUpdate();
    }

    private async Task SendNetworkCheckmarkObject(NetworkCheckmarkObject value)
    {
        byte id = (byte)value.NetEvent;

        var message = value.NetData;

        object[] messageArray = await value.ConvertToNetworkObject();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var eventOptions = new RaiseEventOptions();
        eventOptions.CachingOption = EventCaching.DoNotCache;
        eventOptions.Receivers = (networkManager.LocalDebuggingEnabled) ? ReceiverGroup.All : ReceiverGroup.Others;

        /*----------------------------------------------------------------------------------------------------------------------------*/

        var sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;

        PhotonNetwork.RaiseEvent(id, messageArray, eventOptions, sendOptions);

        await new WaitForUpdate();
    }

    private async Task SendImageChunks(NetworkKillCardObject value, object[] killCardData)
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        Debug.Log("Attempting to send image chunks");
        object[] objects = (object[])value.NetData;
        string[] refPhotos = (string[])objects[5];
        string debug = "";
        foreach (var photo in refPhotos)
        {
            debug += "- " + photo + " - ";
        }

        Debug.Log("Found photo(s): " + debug);

        if (!cts.IsCancellationRequested)
        {
            try
            {
                foreach (var item in refPhotos)
                {
                    Debug.Log("Sending chunks for image: " + item);
                    byte[] image = new byte[0];
                    byte[] refPhoto;

                    string fullPath = KillCardPhotoManager.GetPhotoPath(item);

                    image = File.ReadAllBytes(fullPath);
                    Debug.Log("File size: " + image.Length);
                    refPhoto = image;

                    int nbSocket = (image.Length / KillCardNetworkingImageUtility.IMAGE_LENGTH);

                    Debug.Log("NBSocket = " + nbSocket);

                    if (nbSocket == 0)
                    {
                        Hashtable imageTable = new Hashtable();

                        imageTable.Add(item, image);

                        NetworkImageObject imageChunkObject = new NetworkImageObject();
                        imageChunkObject.NetEvent = value.NetEvent;
                        imageChunkObject.PlayerID = value.PlayerID;
                        imageChunkObject.NetData = imageTable;

                        object[] killCardNetData = (object[])killCardData[2];
                        imageChunkObject.KillCardTitle = killCardNetData[0].ToString();

                        object[] messageArray = await imageChunkObject.ConvertToNetworkObject();

                        string secondDebugString = "Byte array: ";
                        foreach (byte theByte in image)
                        {
                            secondDebugString += "/ " + theByte;
                        }
                        Debug.Log("Byte array length: " + image.Length + secondDebugString);

                        /*----------------------------------------------------------------------------------------------------------------------------*/

                        var eventOptions = new RaiseEventOptions();
                        eventOptions.CachingOption = EventCaching.DoNotCache;
                        eventOptions.Receivers = (networkManager.LocalDebuggingEnabled) ? ReceiverGroup.All : ReceiverGroup.Others;

                        /*----------------------------------------------------------------------------------------------------------------------------*/

                        var sendOptions = new SendOptions();
                        sendOptions.DeliveryMode = DeliveryMode.Reliable;

                        if (PhotonNetwork.RaiseEvent((byte)NetworkEvent.Stop, messageArray, eventOptions, sendOptions))
                        {
                            Debug.Log("Event successfully finished.");
                            return;
                        }
                    }

                    byte[] completeChunk = refPhoto;
                    for (int i = 0; i <= nbSocket + 1; i++)
                    {
                        int lastsBytes = image.Length - (KillCardNetworkingImageUtility.IMAGE_LENGTH * i);

                        List<byte> newImageBytes;
                        Hashtable imageTable = new Hashtable();
                        newImageBytes = new List<byte>();

                        //Debug.Log("LastBytes = " + lastsBytes);
                        //Debug.Log("Sending chunk '" + i + "' for image: " + item);
                        //Debug.Log("Full chunk size: " + completeChunk.Length);

                        await new WaitForBackgroundThread();

                        int maxLeft = (lastsBytes > KillCardNetworkingImageUtility.IMAGE_LENGTH) ?
                            KillCardNetworkingImageUtility.IMAGE_LENGTH :
                            lastsBytes;

                        for (int j = 0; j < maxLeft; j++)
                        {
                            int index = j + (i * maxLeft);
                            newImageBytes.Add(completeChunk[index]);
                        }

                        await new WaitForEndOfFrame();
                        newImageBytes.TrimExcess();
                        //Debug.Log("Added " + newImageBytes.Count + " image bytes to send.");

                        /* string debugString = "Byte array: ";
                        foreach (byte theByte in newImageBytes)
                        {
                            debugString += "/ " + theByte;
                        }

                        Debug.Log("Byte array length: " + newImageBytes.Count + debugString); */

                        imageTable.Add(item, newImageBytes.ToArray());

                        //Debug.Log("<color=green>ImageTable added new array: " + item + " / Count:" + newImageBytes.Count + "</color>");

                        NetworkImageObject imageChunkObject = new NetworkImageObject();
                        imageChunkObject.PlayerID = value.PlayerID;
                        imageChunkObject.NetData = imageTable;

                        object[] killCardContents = (object[])killCardData[2];
                        imageChunkObject.KillCardTitle = killCardContents[0].ToString();

                        /*----------------------------------------------------------------------------------------------------------------------------*/

                        var eventOptions = new RaiseEventOptions();
                        eventOptions.CachingOption = EventCaching.DoNotCache;
                        eventOptions.Receivers = (networkManager.LocalDebuggingEnabled) ? ReceiverGroup.All : ReceiverGroup.Others;

                        /*----------------------------------------------------------------------------------------------------------------------------*/

                        var sendOptions = new SendOptions();
                        sendOptions.DeliveryMode = DeliveryMode.Reliable;

                        if (i == nbSocket)
                        {
                            imageChunkObject.NetEvent = NetworkEvent.Stop;
                            object[] finalMessageArray = await imageChunkObject.ConvertToNetworkObject();
                            if (PhotonNetwork.RaiseEvent((byte)NetworkEvent.Stop, finalMessageArray, eventOptions, sendOptions))
                            {
                                Hashtable tempTable = (Hashtable)imageChunkObject.NetData;
                                byte[] tempArray = (byte[])tempTable[item];
                                Debug.Log("Total bytes sent successfully: " + tempArray.Length);
                                Debug.Log("Event successfully finished.");
                                continue;
                            }
                        }

                        imageChunkObject.NetEvent = NetworkEvent.Create;
                        object[] messageArray = await imageChunkObject.ConvertToNetworkObject();
                        if (PhotonNetwork.RaiseEvent((byte)NetworkEvent.Create, messageArray, eventOptions, sendOptions))
                        {
                            Hashtable tempTable = (Hashtable)imageChunkObject.NetData;
                            byte[] tempArray = (byte[])tempTable[item];
                            Debug.Log("Total bytes sent successfully: " + tempArray.Length);
                            Debug.Log("Event successfully sent.");
                        }

                        await new WaitForSecondsRealtime(0.25f);
                    }

                    //string fullPath = await KillCardPhotoManager.GetPhotoPath(item);
                }
            }
            catch (System.Exception e)
            {
                cts.Cancel();
                Debug.LogError("Could not send network event: " + e.Source + " / Stacktrace: " + e.StackTrace);
                throw;
            }
        }
    }
}