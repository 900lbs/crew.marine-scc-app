// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="UserProfileButton.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Zenject;

#if SCC_2_5

/// <summary>
/// Tells the system which user has been selected and allows it to update accordingly.
/// It's nasty as hell and needs to be reworked at this point, too many duct tape fixes.
/// Implements the <see cref="UI_Button" />
/// Implements the <see cref="Zenject.IInitializable" />
/// Implements the <see cref="Zenject.ILateDisposable" />
/// </summary>
/// <seealso cref="UI_Button" />
/// <seealso cref="Zenject.IInitializable" />
/// <seealso cref="Zenject.ILateDisposable" />
public class UserProfileButton : UI_Button
{
    #region Injection Construction

    /// <summary>
    /// The current ship
    /// </summary>
    [Inject(Id = "CurrentShip")]
    private ShipVariable currentShip;

    [InjectOptional]
    private AnnotationManager annotationManager; // Assgined in the factory, ghetto coupled fix, break apart later.

    [Inject]
    private ProjectManager projectManager;

    /// <summary>
    /// The user profile manager
    /// </summary>
    private UserProfileManager userProfileManager;

    private MainMenu mainMenu;

    /// <summary>
    /// The settings
    /// </summary>
    private Settings settings;

    private ProjectSettings projectSettings;

    /// <summary>
    /// Constructs the specified net client.
    /// </summary>
    /// <param name="netClient">The net client.</param>
    /// <param name="userProfileMan">The user profile man.</param>
    /// <param name="signal">The signal.</param>
    /// <param name="setting">The setting.</param>
    [Inject]
    public void Construct(UserProfileManager userProfileMan, MainMenu menu, SignalBus signal, Settings setting, ProjectSettings project)
    {
        userProfileManager = userProfileMan;
        mainMenu = menu;
        _signalBus = signal;
        this.settings = setting;
        projectSettings = project;
    }

    #endregion Injection Construction

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public bool IsStaticUserAction;

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    /// <value>The user.</value>
    public UserProfile User;

    /// <summary>
    /// The user button action
    /// </summary>
    public UserAction UserButtonAction;

    /// <summary>
    /// The button text
    /// </summary>
    public TextMeshProUGUI ButtonText;

    /// <summary>
    /// The profiles
    /// </summary>
    public ColorProfile[] Profiles;

    public UserProfileButtonHandler ButtonHandler { get; set; }

    [Tooltip("Ghetto af - Add each color profile by index that needs to have a dynamic change based on user.")]
    public int[] OptionalDynamicProfileBasedOnUsername;

    public bool IsDynamicSelectedColorBasedOnUser;

    [Tooltip("This is for ColorProfile that need to toggle specifically when we are disabled.")]
    public ColorProfile[] DisabledTogglingProfiles;

    [SerializeField]
    [ReadOnly]
    private bool isDisabledToggled = false;

    private bool isInitialized = false;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    private void OnValidate()
    {
        int profilesLength = Profiles.Length;
        for (int i = 0; i < profilesLength; ++i)
        {
            Profiles[i].OnValidate();
        }

        int disabledProfilesLength = DisabledTogglingProfiles.Length;
        for (int i = 0; i < disabledProfilesLength; ++i)
        {
            DisabledTogglingProfiles[i].OnValidate();
        }
    }

    private void UpdateAnnotationManager()
    {
        if (UserButtonAction == UserAction.ToggleAnnotations)
        {
            annotationManager?.SetCurrentActiveUserAnnotations(User.UserNameID);
            ToggleDisabledTogglingComponents(true);
            isInitialized = true;
        }
        else if (UserButtonAction == UserAction.ReturnToShipSelection)
        {
            UpdateSelectedColorBasedOnUser();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Start()
    {
        if ((UserButtonAction == UserAction.ReturnToShipSelection || UserButtonAction == UserAction.ReturnToUserSelection) && !IsStaticUserAction)
            UserButtonAction = (projectSettings.MultishipEnabled) ? UserAction.ReturnToShipSelection : UserAction.ReturnToUserSelection;

        State = ((UserButtonAction == UserAction.ReturnToShipSelection || UserButtonAction == UserAction.ReturnToShipSelection) && !IsStaticUserAction) ? ActiveState.Selected : ActiveState.Enabled;
        name = User.UserNameID.ToString();
        ButtonHandler = GetComponentInParent<UserProfileButtonHandler>();

        OnStateChange();

        switch (UserButtonAction)
        {
            case UserAction.Login:
                _signalBus.Subscribe<Signal_MainMenu_OnGameStateChanged>(GameStateChanged);
                break;

            case UserAction.ToggleAnnotations:
                _signalBus.Subscribe<Signal_NetworkClient_OnActiveUsersUpdated>(ActiveUsersUpdated);
                _signalBus.Subscribe<Signal_AnnoMan_OnActiveUserAnnotationsUpdated>(ActiveUserAnnotationsUpdated);
                _signalBus.Subscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnotationStateChanged);
                _signalBus.Subscribe<Signal_MainMenu_OnGameStateChanged>(GameStateChanged);
                break;
        }
        _signalBus.Subscribe<Signal_MainMenu_OnShipInitialized>(ShipInitialized);
        _signalBus.Subscribe<Signal_NetworkClient_OnClientStateChanged>(ClientStateChanged);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Lates th/// e dispose.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        _signalBus.TryUnsubscribe<Signal_AnnoMan_OnActiveUserAnnotationsUpdated>(ActiveUserAnnotationsUpdated);
        _signalBus.TryUnsubscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnotationStateChanged);
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnActiveUsersUpdated>(ActiveUsersUpdated);
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnGameStateChanged>(GameStateChanged);
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnShipInitialized>(ShipInitialized);
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnClientStateChanged>(ClientStateChanged);
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnGameStateChanged>(GameStateChanged);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override async void BTN_OnClick()
    {
        base.BTN_OnClick();

        switch (UserButtonAction)
        {
            case UserAction.Login:
                await DetermineLogin();
                break;

            case UserAction.ToggleAnnotations:
                UpdateAnnotationManager();
                break;

            case UserAction.Select:
                ButtonHandler.UserSelected(User.UserNameID);

                State = (State == ActiveState.Enabled) ? ActiveState.Selected : ActiveState.Enabled;
                break;

            case UserAction.ReturnToUserSelection:
                NetworkClient.IsOfflineMode = false;
                await networkClient.LeaveRoom(false);
                mainMenu.ChangeState(GameState.UserSelection);
                break;

            case UserAction.ReturnToShipSelection:
                mainMenu.ChangeState(GameState.ShipSelection);
                await networkClient.LeaveRoom(false);
                currentShip.ResetShip();
                projectManager.ForceCleanup();
                //int scene = (settings.IsTabletUser) ? 2 : 1;
                MultiSceneManager.LoadSceneAdditively(1, true);
                break;
        }
    }

    private void ShipInitialized()
    {
    }

    private async Task DetermineLogin()
    {
        NetworkClient.Login(User);
        State = ActiveState.Selected;

        string room = currentShip.Ship.ID.ToString();

        //if (!Photon.Pun.PhotonNetwork.IsConnected)
        byte timeout = 0;

        //NetworkClient.IsOfflineMode = false;
        if (!PhotonNetwork.OfflineMode)
        {
            networkClient.InitializeNetworkSettings(room);
            while (!PhotonNetwork.IsConnectedAndReady || timeout < 2)

            {
                Debug.Log("Waiting to connect.");
                timeout++;
                await new WaitForEndOfFrame();
            }
        }

        if (User.UserNameID == UserNameID.SafetyCommandCenter || PhotonNetwork.OfflineMode == true)
        {
            Debug.Log("SCC or offline mode detected, creating room.");
            networkClient.CreateRoom(room);
            return;
        }
        networkClient.JoinRoom(room);

        //mainMenu.ChangeState (GameState.Room);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private void ClientStateChanged(Signal_NetworkClient_OnClientStateChanged signal)
    {
        if (!IsDynamicSelectedColorBasedOnUser)
            return;

        if (signal.NewState == Photon.Realtime.ClientState.Joined && (UserButtonAction == UserAction.ReturnToUserSelection || UserButtonAction == UserAction.ReturnToShipSelection))
        {
            UserProfile user = userProfileManager.GetUserProfile(NetworkClient.GetUserName());
            User = user;
            ButtonText.text = UserProfileFactory.GetAbbreviatedName(user.UserNameID);
            transform.localScale = Vector3.one;
            UpdateSelectedColorBasedOnUser();
            State = ActiveState.Selected;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [active users updated].
    /// </summary>
    /// <param name="signal">The signal.</param>
    public void ActiveUsersUpdated(Signal_NetworkClient_OnActiveUsersUpdated signal)
    {
        DetermineNetworkState(signal.Users);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [active users updated].
    /// </summary>
    /// <param name="signal">The signal.</param>
    public void ActiveUserAnnotationsUpdated(Signal_AnnoMan_OnActiveUserAnnotationsUpdated signal)
    {
        DetermineAnnotationState(signal.Users);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private void AnnotationStateChanged(Signal_AnnoMan_OnAnnotationStateChanged signal)
    {
        if (!signal.annotationState.HasFlag(AnnotationState.Move) && User.UserNameID == NetworkClient.GetUserName() && UserButtonAction == UserAction.ToggleAnnotations)
        {
            if (State != ActiveState.Selected)
                UpdateAnnotationManager();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    internal void GameStateChanged(Signal_MainMenu_OnGameStateChanged signal)
    {
        if (signal.State == GameState.Room && !isInitialized)
            UpdateAnnotationManager();
        if (UserButtonAction == UserAction.Login)
            State = ActiveState.Enabled;
    }

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange()
    {
        UpdateSelectedColorBasedOnUser();
        int profilesLength = Profiles.Length;
        //Debug.Log(name + " UserButton set to state: " + State.ToString());
        for (int i = 0; i < profilesLength; ++i)
        {
            Profiles[i].StateChange(State);
        }
    }

    private void DetermineNetworkState(UserNameID user)
    {
        UpdateSelectedColorBasedOnUser();

        ActiveState cacheState = (State == ActiveState.Disabled) ? ActiveState.Selected : State;
        State = (user.HasFlag(User.UserNameID)) ? cacheState : ActiveState.Disabled;

        if (IsDynamicSelectedColorBasedOnUser)
            ToggleDisabledTogglingComponents(false);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private void DetermineAnnotationState(UserNameID user)
    {
        if (State != ActiveState.Disabled)
        {
            State = (user.HasFlag(User.UserNameID)) ? ActiveState.Selected : ActiveState.Enabled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private void ToggleDisabledTogglingComponents(bool isToggling)
    {
        if (State == ActiveState.Disabled)
        {
            for (int i = 0; i < DisabledTogglingProfiles.Length; i++)
            {
                isDisabledToggled = (isToggling) ? !isDisabledToggled : isDisabledToggled;
                DisabledTogglingProfiles[i].StateChange((isDisabledToggled) ? ActiveState.Enabled : ActiveState.Selected);
            }
        }
    }

    public void UpdateSelectedColorBasedOnUser()
    {
        if (IsDynamicSelectedColorBasedOnUser)
        {
            for (int i = 0; i < OptionalDynamicProfileBasedOnUsername.Length; i++)
            {
                Profiles[OptionalDynamicProfileBasedOnUsername[i]].SelectedColor = AnnotationManager.GetLineColorBasedOnUser(User.UserNameID);

                if (UserButtonAction == UserAction.ReturnToUserSelection || UserButtonAction == UserAction.ReturnToShipSelection)
                    Profiles[OptionalDynamicProfileBasedOnUsername[i]].StateChange(ActiveState.Selected);
            }
        }
    }

    /// <summary>
    /// Class Factory.
    /// Implements the <see cref="Zenject.PlaceholderFactory{UnityEngine.Object, UserProfile, UserProfileButton}" />
    /// Implements the <see cref="IUserProfileButtonFactory" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{UnityEngine.Object, UserProfile, UserProfileButton}" />
    /// <seealso cref="IUserProfileButtonFactory" />
    public new class Factory : PlaceholderFactory<Object, UserProfile, UserProfileButton>, IUserProfileButtonFactory
    { }
}

#endif