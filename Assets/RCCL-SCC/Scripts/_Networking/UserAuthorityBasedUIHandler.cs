using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
public class UserAuthorityBasedUIHandler : MonoBehaviour
{
    SignalBus _signalBus;

    UserProfileManager userProfileManager;

    [Inject]
    public void Construct(SignalBus signal, UserProfileManager userProfileMan)
    {
        _signalBus = signal;
        userProfileManager = userProfileMan;
    }

    [EnumFlag]
    public AuthorityID AcceptedAuthorities;

    void Awake()
    {
        _signalBus.Subscribe<Signal_MainMenu_OnShipInitialized>(OnShipInitialized);
    }

    void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_MainMenu_OnShipInitialized>(OnShipInitialized);
    }

    void OnShipInitialized(Signal_MainMenu_OnShipInitialized signal)
    {
        UserProfile currentUser = userProfileManager.GetUserProfile(NetworkClient.GetUserName());

        currentUser.DebugUserProfile();
        
        gameObject.SetActive(AcceptedAuthorities.HasFlag(currentUser.Authority));
    }
}
