using System.Linq;
using System.Threading;

// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="NetworkClientManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DG.Tweening;
using ExitGames.Client.Photon;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Hashtable = ExitGames.Client.Photon.Hashtable;

#if SCC_2_5

/// <summary>
/// Class NetworkClientManager.
/// Implements the <see cref="Photon.Pun.MonoBehaviourPunCallbacks" />
/// Implements the <see cref="Photon.Realtime.IOnEventCallback" />
/// Implements the <see cref="Zenject.IInitializable" />
/// </summary>
/// <seealso cref="Photon.Pun.MonoBehaviourPunCallbacks" />
/// <seealso cref="Photon.Realtime.IOnEventCallback" />
/// <seealso cref="Zenject.IInitializable" />
public class NetworkClientManager : MonoBehaviourPunCallbacks, IOnEventCallback, IInitializable
{
    #region Injection Construction

    /// <summary>
    /// The network client
    /// </summary>
    private NetworkClient networkClient;

    /// <summary>
    /// Constructs the specified net client.
    /// </summary>
    /// <param name="netClient">The net client.</param>
    [Inject]
    public void Construct(NetworkClient netClient)
    {
        networkClient = netClient;
    }

    #endregion Injection Construction

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The debug display type
    /// </summary>
    public DebugType DebugDisplayType;

    /// <summary>
    /// The player list entries
    /// </summary>
    [ReadOnly]
    [SerializeField]
    public Player[] PlayerListEntries;

    /// <summary>
    /// The network actions listeners
    /// </summary>
    [SerializeField]
    public List<IShipNetworkEvents> NetworkActionsListeners;

    /// <summary>
    /// The annotation listeners
    /// </summary>
    [SerializeField]
    public List<IShipNetworkAnnotation> AnnotationListeners;

    /// <summary>
    /// The cached room list
    /// </summary>
    private Dictionary<string, RoomInfo> cachedRoomList;

    /// <summary>
    /// The local room event cache
    /// </summary>
    [SerializeField]
    private List<EventData> LocalRoomEventCache;

    //public static Hashtable CachedRoomPropertyList;

    /// <summary>
    /// The cached client state
    /// </summary>
    private ClientState cachedClientState;

    /// <summary>
    /// The debug listener count
    /// </summary>
    public int DebugListenerCount;

    /// <summary>
    /// The debug room property list
    /// </summary>
    public List<string> DebugRoomPropertyList;

    /// <summary>
    /// The local debugging enabled
    /// </summary>
    public bool LocalDebuggingEnabled;

    /// <summary>
    /// The debug style
    /// </summary>
    private GUIStyle debugStyle;

    /// <summary>
    /// The debug enabled
    /// </summary>
    private bool debugEnabled = false;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Initialize()
    {
        //DebugUtils.Log(DebugType.Networking, Color.green, "Initializing Network Manager.", this);

        AnnotationListeners = new List<IShipNetworkAnnotation>();
        NetworkActionsListeners = new List<IShipNetworkEvents>();
        DebugRoomPropertyList = new List<string>();
        LocalRoomEventCache = new List<EventData>();
        //CachedRoomPropertyList = new Hashtable();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Unity Functions

    /// <summary>
    /// Called when [enable].
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();

        PhotonNetwork.MaxResendsBeforeDisconnect = 500;
        PhotonNetwork.SendRate = 100;
        PhotonNetwork.SerializationRate = 100;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [disable].
    /// </summary>
    public override void OnDisable()
    {
        base.OnDisable();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Updates this instance.
    /// </summary>
    private void Update()
    {
        ///Network debugging

        if (Input.GetKey(KeyCode.LeftShift) & Input.GetKey(KeyCode.LeftAlt) & Input.GetKeyDown(KeyCode.D))
        {
            foreach (var item in GetRoomProperty(ShipRoomProperties.Annotations))
            {
                Debug.Log("Annotations room property found: " + item.Key + " / " + item.Value, this);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) & Input.GetKey(KeyCode.LeftAlt) & Input.GetKeyDown(KeyCode.F))
        {
            PhotonNetwork.Disconnect();
        }

        ///Simple local player list cache
        if (PlayerListEntries != PhotonNetwork.PlayerList)
        {
            PlayerListEntries = PhotonNetwork.PlayerList;
        }

        if (AnnotationListeners != null && DebugListenerCount != AnnotationListeners.Count)
        {
            DebugListenerCount = AnnotationListeners.Count;
        }

        if (cachedClientState != PhotonNetwork.NetworkClientState)
        {
            cachedClientState = PhotonNetwork.NetworkClientState;
            networkClient.ClientStateChanged(cachedClientState);
        }

        if (PhotonNetwork.CurrentRoom != null)
        {
            if (DebugRoomPropertyList.Count != PhotonNetwork.CurrentRoom.CustomProperties.Count)
            {
                DebugRoomPropertyList.Clear();
                foreach (var kvp in PhotonNetwork.CurrentRoom.CustomProperties)
                {
                    DebugRoomPropertyList.Add(kvp.Key.ToString());
                }
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            debugEnabled = !debugEnabled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [GUI].
    /// </summary>
    private void OnGUI()
    {
        /*if (debugEnabled)
        {
            if (debugStyle == null)
            {
                debugStyle = new GUIStyle();
                debugStyle.normal.textColor = Color.red;
                debugStyle.fontSize = 28;
            }
            //GUILayout.Label("Master Server Region: " + PhotonNetwork.CloudRegion, debugStyle);
            //GUILayout.Label("Master Server: " + PhotonNetwork.ServerAddress?.ToString(), debugStyle);

            GUILayout.Label("Is Online: " + !PhotonNetwork.OfflineMode, debugStyle);
            GUILayout.Label("Is Master client: " + PhotonNetwork.IsMasterClient.ToString(), debugStyle);
            string room = (PhotonNetwork.CurrentRoom == null) ? "none" : PhotonNetwork.CurrentRoom.Name;
            GUILayout.Label("Current room: " + room, debugStyle);
            GUILayout.Label("Current Player Count: " + PlayerListEntries.Length, debugStyle);

            foreach (Player item in PlayerListEntries)
            {
                GUILayout.Label("Player: " + item.NickName, debugStyle);
            }

            if (PhotonNetwork.CurrentRoom != null)
            {
                foreach (DictionaryEntry hash in PhotonNetwork.CurrentRoom.CustomProperties)
                {
                    try
                    {
                        var key = (string) hash.Key;
                        Dictionary<string, string> value = new Dictionary<string, string>();
                        if (hash.Value.GetType() == typeof(Dictionary<string, string>))
                        {
                            value = (Dictionary<string, string>) hash.Value;
                            GUILayout.Label("Current Property: " + key as string + ":" + key.GetType() + "  /  Current Value Length: " + value.Count, debugStyle);
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError(hash.Value.GetType() + " was found as a room property instead of Dictionary<string,string> : " + hash.Value);
                        throw;
                    }
                }
            }

            GUILayout.Label("Current Orientation: " + DeviceOrientationManager.CurrentOrientation.ToString(), debugStyle);

            /*  if (CachedRoomPropertyList != null)
             {
                 foreach (DictionaryEntry hash in CachedRoomPropertyList)
                 {
                     try
                     {
                         var key = (string)hash.Key;
                         Dictionary<string, string> value = new Dictionary<string, string>();
                         if (hash.Value.GetType() == typeof(Dictionary<string, string>))
                         {
                             value = (Dictionary<string, string>)hash.Value;
                             GUILayout.Label("Current Property: " + key as string + ":" + key.GetType() + "  /  Current Value Length: " + value.Count, debugStyle);
                         }
                     }
                     catch (System.Exception)
                     {
                         Debug.LogError(hash.Value.GetType() + " was found as a room property instead of Dictionary<string,string> : " + hash.Value);
                         throw;
                     }
                 }
             } */
    }

    #endregion Unity Functions

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region PUN Callbacks

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    /// </summary>
    /// <remarks>The list of available rooms won't become available unless you join a lobby via LoadBalancingClient.OpJoinLobby.
    /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.</remarks>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master.", this);
        //DebugUtils.Log(DebugType.Networking, Color.green, "Connected to Master.", this);
        networkClient.OnConnectedToMasterServer();
    }

    /// <summary>
    /// Called after switching to a new MasterClient when the current one leaves.
    /// </summary>
    /// <param name="newMasterClient">The new master client.</param>
    /// <remarks>This is not called when this client enters a room.
    /// The former MasterClient is still in the player list when this method get called.</remarks>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //PhotonNetwork.Disconnect();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when a remote player entered the room. This Player is already added to the playerlist.
    /// </summary>
    /// <param name="newPlayer">The new player.</param>
    /// <remarks>If your game starts with a certain number of players, this callback can be useful to check the
    /// Room.playerCount and find out if you can start.</remarks>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        networkClient.OnPlayerJoinedRoom(newPlayer);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
    /// </summary>
    /// <param name="otherPlayer">The other player.</param>
    /// <remarks>If another player leaves the room or if the server detects a lost connection, this callback will
    /// be used to notify your game logic.
    /// Depending on the room's setup, players may become inactive, which means they may return and retake
    /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
    /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
    /// the callback is called.</remarks>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        networkClient.OnPlayerLeftRoom(otherPlayer);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
    /// </summary>
    /// <remarks>This callback is only called on the client which created a room (see OpCreateRoom).
    /// As any client might close (or drop connection) anytime, there is a chance that the
    /// creator of a room does not execute OnCreatedRoom.
    /// If you need specific room properties or a "start signal", implement OnMasterClientSwitched()
    /// and make each new MasterClient check the room's state.</remarks>
    public override void OnCreatedRoom()
    {
        networkClient.OnCreatedRoom();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when the server couldn't create a room (OpCreateRoom failed).
    /// </summary>
    /// <param name="returnCode">Operation ReturnCode from the server.</param>
    /// <param name="message">Debug message for the error.</param>
    /// <remarks>The most common cause to fail creating a room, is when a title relies on fixed room-names and the room already exists.</remarks>
    public override void OnCreateRoomFailed(short returnCode, string message)
    { }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when the LoadBalancingClient entered a room, no matter if this client created it or simply joined.
    /// </summary>
    /// <remarks>When this is called, you can access the existing players in Room.Players, their custom properties and Room.CustomProperties.
    /// In this callback, you could create player objects. For example in Unity, instantiate a prefab for the player.
    /// If you want a match to be started "actively", enable the user to signal "ready" (using OpRaiseEvent or a Custom Property).</remarks>
    public override void OnJoinedRoom()
    {
        networkClient.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        networkClient.OnDisconnected(cause);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when a previous OpJoinRoom call failed on the server.
    /// </summary>
    /// <param name="returnCode">Operation ReturnCode from the server.</param>
    /// <param name="message">Debug message for the error.</param>
    /// <remarks>The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).</remarks>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        networkClient.OnJoinRoomFailed();
        Debug.Log(message, this);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
    /// </summary>
    /// <param name="propertiesThatChanged">The properties that changed.</param>
    /// <remarks>Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br />
    /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.</remarks>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        networkClient.RoomPropertiesUpdated(propertiesThatChanged);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called for any update of the room-listing while in a lobby (InLobby) on the Master Server.
    /// </summary>
    /// <param name="roomList">The room list.</param>
    /// <remarks>Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
    /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.</remarks>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Roomlist updated.", this);

        UpdateCachedRoomList(roomList);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The standard PUN Event listener, inherited from IOnEventCallback.
    /// </summary>
    /// <param name="photonEvent">The photon event.</param>
    /// <remarks>To receive events, implement IOnEventCallback in any class and register it via AddCallbackTarget
    /// (either in LoadBalancingClient or PhotonNetwork).
    /// With the EventData.Sender you can look up the Player who sent the event.
    /// It is best practice to assign an eventCode for each different type of content and action, so the Code
    /// will be essential to read the incoming events.</remarks>
    public async void OnEvent(EventData photonEvent)
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        try
        {
            if (!cts.IsCancellationRequested)
            {
                if (photonEvent.Code > 199)
                    return;

                //Debug.Log("Event type: " + photonEvent.CustomData.GetType(), this);
                ///Here we run through our type checks, this is cheaper than reflection and also required
                /// since the data sent and received has to be an object. We manually go through each class that
                /// inherits from INetworkObject and check which one it is by utilizing ConvertFromNetwork()
                /// inherited from the interface.
                NetworkAnnotationObject receivedObject = new NetworkAnnotationObject();
                if (receivedObject.ConvertFromNetworkObject((object[])photonEvent.CustomData))
                {
                    Debug.Log("Received Annotation object", this);
                    await networkClient.SendNewNetworkEventReceived((INetworkObject)receivedObject);
                    return;
                }

                NetworkKillCardObject receivedKillCardObject = new NetworkKillCardObject();
                if (receivedKillCardObject.ConvertFromNetworkObject((object[])photonEvent.CustomData))
                {
                    Debug.Log("Received NetworkKillCardObject.", this);
                    await networkClient.SendNewNetworkEventReceived(receivedKillCardObject);
                    return;
                }

                NetworkImageObject receivedImageObject = new NetworkImageObject();
                if (receivedImageObject.ConvertFromNetworkObject((object[])photonEvent.CustomData))
                {
                    Debug.Log("Received NetworkImageObject.", this);
                    await networkClient.SendNewNetworkEventReceived(receivedImageObject);
                    return;
                }

                NetworkCheckmarkObject receivedCheckmarkObject = new NetworkCheckmarkObject();
                if (receivedCheckmarkObject.ConvertFromNetworkObject((object[])photonEvent.CustomData))
                {
                    Debug.Log("Received Checkmark Object.", this);
                    await networkClient.SendNewNetworkEventReceived(receivedCheckmarkObject);
                    return;
                }

                NetworkShareObject receivedSharedObject = new NetworkShareObject();
                if (receivedSharedObject.ConvertFromNetworkObject((object[])photonEvent.CustomData) && await CheckIsEventAlreadyCached(photonEvent) == false)
                {
                    Debug.Log("Received new share object.", this);
                    await networkClient.SendNewNetworkEventReceived((INetworkObject)receivedSharedObject);
                    return;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Event error on received: " + e.StackTrace, this);
            cts.Cancel();
            return;
        }
    }

    /// <summary>
    /// Checks the is event already cached.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private async Task<bool> CheckIsEventAlreadyCached(EventData value)
    {
        if (LocalRoomEventCache == null)
            LocalRoomEventCache = new List<EventData>();

        if (LocalRoomEventCache.Contains(value))
        {
            Debug.Log("Found the same event in our cache.", this);
            return true;
        }
        else
        {
            Debug.Log("Did not find this event in the cache: " + value.CustomData, this);
            LocalRoomEventCache.Add(value);
            await new WaitForEndOfFrame();
            return false;
        }
    }

    #endregion PUN Callbacks

    /*------------------------------------------------------------------------------------------------*/

    #region Static Functions

    /// <summary>
    /// Joins the room.
    /// </summary>
    /// <param name="roomID">The room identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool JoinRoom(string roomID)
    {
        //Debug.Log("Requesting NetworkManager to join: " + roomID, this);
        return PhotonNetwork.JoinRoom(roomID);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the room properties.
    /// </summary>
    /// <returns>Hashtable.</returns>
    public static Hashtable GetRoomProperties()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates or update a room property.
    /// </summary>
    /// <param name="netEvent">The net event.</param>
    /// <param name="propKey">The property key.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="multipleValues">The multiple values.</param>
    public void CreateOrUpdateRoomProperty(NetworkEvent netEvent = NetworkEvent.Create,
        ShipRoomProperties propKey = ShipRoomProperties.None,
        string optionalPropKeySuffix = "",
        short key = 0,
        string value = "",
        Dictionary<string, string> multipleValues = null)
    {
        string convertedPropKey = propKey.ToString();

        Hashtable propertyTable = new Hashtable();

        if (!string.IsNullOrEmpty(optionalPropKeySuffix))
            convertedPropKey += optionalPropKeySuffix;

        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log((PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(convertedPropKey) ? "Updating" : "Creating") +
            " room property " + convertedPropKey);

        //If our Hashtable already contains this room property
        try
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(convertedPropKey))
            {
                //Debug.Log("Found property key: " + convertedPropKey + " of type: " + PhotonNetwork.CurrentRoom.CustomProperties[convertedPropKey].GetType(), this);
                Dictionary<string, string> propertyDict = new Dictionary<string, string>();

                //Create a local version of those properties as Dictionary<string,string>

                if (PhotonNetwork.CurrentRoom.CustomProperties[convertedPropKey].GetType() == typeof(string))
                {
                    //Debug.Log("<color=red>" + PhotonNetwork.CurrentRoom.CustomProperties[convertedPropKey] + " found instead of a dictionary.</color>", this);
                    return;
                }
                propertyDict = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties[convertedPropKey];

                if (multipleValues != null)
                {
                    propertyDict = multipleValues;
                    //Debug.Log("Multiple Property Values found, mirroring Property.");
                }
                else
                {
                    //Else if we already have this key in our dictionary, update it. This is used for
                    //properties that only have one keyValuePair to worry about.

                    switch (netEvent)
                    {
                        case NetworkEvent.Create:
                            if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                                Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                                Debug.Log("Creating: " + value);

                            if (propertyDict.ContainsKey(key.ToString()))
                            {
                                return;
                            }
                            propertyDict.Add(key.ToString(), value);

                            break;

                        case NetworkEvent.Edit:
                            if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                                Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                                Debug.Log("Editing" + value, this);
                            if (propertyDict.ContainsKey(key.ToString()))
                                propertyDict[key.ToString()] = value;
                            else
                                Debug.LogError(string.Format("Edit attempted but failed for: '{0}' / '{1}'.", key, value));
                            break;

                        case NetworkEvent.Erase:
                            propertyDict.Remove(key.ToString());

                            break;

                        case NetworkEvent.EraseAll:
                            if (NetworkClient.GetIsPlayerMasterClient())
                            {
                                Debug.Log("Erase all called by master client", this);
                                propertyDict = null;
                            }
                            else
                            {
                                List<string> tempPropertyDictList = new List<string>();
                                tempPropertyDictList.AddRange(propertyDict.Keys);

                                foreach (var item in tempPropertyDictList)
                                {
                                    string[] convertedAnno = AnnotationUtilities.GetAnnotationArray(propertyDict[item]).Result;
                                    if (convertedAnno.Length > 1)
                                    {
                                        if (convertedAnno[2] == NetworkClient.GetUserName().ToString())
                                        {
                                            propertyDict.Remove(item);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                propertyTable.Add(convertedPropKey, propertyDict);

                //Debug.Log("<color=green>" + propertyTable[convertedPropKey].ToString() + " has been updated to Total: " + propertyDict.Count + "</Color>");

                //Update our room properties with the local table we built up.
                PhotonNetwork.CurrentRoom.SetCustomProperties(propertyTable);
                //CachedRoomPropertyList = propertyTable;
            }
            //Else if we did not find a prexisting Room Property by this ID
            else
            {
                //Our Hashtable's value for this specific key
                Dictionary<string, string> propDict = new Dictionary<string, string>();

                //Add our current new entry to this properties new database dictionary
                if (multipleValues == null)
                    propDict.Add(key.ToString(), value);
                else
                    propDict = multipleValues;

                //Add the dictionary to the new Hashtable, this is the correct format to send over the network.
                propertyTable.Add(convertedPropKey, propDict);

                //Set the room properties
                PhotonNetwork.CurrentRoom.SetCustomProperties(propertyTable);
                //CachedRoomPropertyList = propertyTable;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Could not finish or create a property, reason: " + e.Message + " Trace: " + e.StackTrace, this);
            return;
        }
    }

    /// <summary>
    /// Resets the room property.
    /// </summary>
    /// <param name="propKey">The property key.</param>
    public static void ResetRoomProperty(ShipRoomProperties propKey)
    {
        string convertedPropKey = propKey.ToString();

        Hashtable propertyTable = new Hashtable();

        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log((PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(convertedPropKey) ? "Updating" : "Creating") + " room property " + convertedPropKey);

        //If our Hashtable already contains this room property
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(convertedPropKey))
        {
            Dictionary<string, string> propertyDict = new Dictionary<string, string>();

            propertyDict.Add(convertedPropKey, "");
            propertyTable.Add(convertedPropKey, propertyDict);

            PhotonNetwork.CurrentRoom.SetCustomProperties(propertyTable);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the room property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
    public static Dictionary<string, string> GetRoomProperty(ShipRoomProperties key)
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key.ToString()))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.Count <= 0)
            {
                Debug.LogWarning("No room properties found.");
            }
            else
            {
                if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                    Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                    Debug.Log("Room contains " + PhotonNetwork.CurrentRoom.CustomProperties.Count + " properties, but could not find " + key.ToString());
            }
        }

        Dictionary<string, string> getProperty = PhotonNetwork.CurrentRoom.CustomProperties[key.ToString()] as Dictionary<string, string>;

        return getProperty;
    }

    #endregion Static Functions

    /*------------------------------------------------------------------------------------------------*/

    #region Public Functions

    /// <summary>
    /// Initializes the network manager.
    /// </summary>
    public bool InitializeNetworkManager()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        PlayerListEntries = new Player[0];
        PhotonNetwork.PhotonServerSettings = DataIO.GetPhotonSettings().ServerSettings;
        return PhotonNetwork.ConnectUsingSettings();
        //ServerSettings cachedSettings = PhotonNetwork.PhotonServerSettings;

        /*         if (cachedSettings.AppSettings.UseNameServer)
                {
                    return PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    return PhotonNetwork.ConnectToMaster(cachedSettings.AppSettings.Server, cachedSettings.AppSettings.Port, cachedSettings.AppSettings.AppIdRealtime);
                } */
    }

    #endregion Public Functions

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Private Functions

    /// <summary>
    /// Updates the cached room list.
    /// </summary>
    /// <param name="roomList">The room list.</param>
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion Private Functions
}

#endif