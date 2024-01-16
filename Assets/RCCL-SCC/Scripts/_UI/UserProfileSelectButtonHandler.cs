using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;
public class UserProfileSelectButtonHandler : UserProfileButtonHandler
{
    [Inject]
    AnnotationManager annotationManager;
    [Inject]
    SignalBus _signalBus;

    List<UserProfileButton> ProfileButtons;

    [EnumFlag]
    public UserNameID UsersSelected;

    public override void Start()
    {
        base.Start();

        AllUsersDeselected();

        _signalBus.Subscribe<Signal_AnnoMan_OnRoomPropertyChanged>(RoomPropertyChanged);
        _signalBus.Subscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_AnnoMan_OnRoomPropertyChanged>(RoomPropertyChanged);
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    void RoomPropertyChanged(Signal_AnnoMan_OnRoomPropertyChanged signal)
    {
        ToggleButtons(!signal.roomProperty.HasFlag(ShipRoomProperties.GAOverlay));
    }

    void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        if(signal.PromptType == PromptMenuType.Erase)
            {
                UsersSelected = 0;
                for (int i = 0; i < ProfileButtons.Count; i++)
                {
                    ProfileButtons[i].State = ActiveState.Enabled;
                }
            }
    }

    public override async Task WaitForLoad()
    {
        ProfileButtons = new List<UserProfileButton>();

        while (userProfileManager.GetAllUserProfiles() == null && Application.isPlaying)
        {
            await new WaitForEndOfFrame();
        }

        await SpawnUserButtons(userProfileManager.GetAllUserProfiles());
        await new WaitForEndOfFrame();
    }

    protected override async Task SpawnUserButtons(List<UserProfile> profiles)
    {

        availableUserNameIDs = (NetworkClient.GetIsPlayerMasterClient()) ? NetworkClient.GetAllPossibleUserNameIDs() : NetworkClient.GetUserName();

        int userProfileCount = profiles.Count;

        for (int i = 0; i < userProfileCount; ++i)
        {
            //Debug.Log("Found available user: " + item.UserNameID, this);
            UserProfileButton button = DetermineCorrectFactoryToUse().Create(UserProfileButtonPrefab, profiles[i]);

            button.ButtonText.text = (IsAbbreviated) ? UserProfileFactory.GetAbbreviatedName(profiles[i].UserNameID) : UserProfileFactory.GetFullName(profiles[i].UserNameID);

            if (SpawnParent.Length > 0)
            {
                for (int u = 0; u < SpawnParent.Length; u++)
                {
                    if (SpawnParent[u].childCount < MaximumSpawnPerParent)
                    {
                        button.transform.SetParent(SpawnParent[u]);
                        button.transform.localScale = Vector3.one;
                    }
                }
            }
            else
            {
                button.transform.SetParent(transform);
                button.transform.localScale = Vector3.one;
            }
            ProfileButtons.Add(button);
            button.ButtonHandler = this;
            button.UpdateSelectedColorBasedOnUser();
        }
        await new WaitForUpdate();
    }

    public void AllUsersSelected()
    {
        UsersSelected = 0;
        UsersSelected = NetworkClient.GetAllPossibleUserNameIDs();

        int totalProfiles = ProfileButtons.Count;
        for (int i = 0; i < totalProfiles; ++i)
        {
            ProfileButtons[i].State = ActiveState.Selected;
        }
    }
    public void AllUsersDeselected()
    {
        UsersSelected = NetworkClient.GetAllPossibleUserNameIDs();

        int totalProfiles = ProfileButtons.Count;
        for (int i = 0; i < totalProfiles; ++i)
        {
            ProfileButtons[i].State = ActiveState.Enabled;
        }

        UsersSelected = 0;
    }

    public override void UserSelected(UserNameID user)
    {
        Debug.Log(user.ToString() + " was " + (UsersSelected.HasFlag(user) ? "Deselected" : "Selected"), this);
        if (UsersSelected.HasFlag(user))
        {
            UsersSelected &= ~user;
        }
        else
        {
            UsersSelected = (UsersSelected | user);
        }
    }

    public void ToggleButtons(bool value)
    {
        UsersSelected = 0;

        int buttonCount = ProfileButtons.Count;
        for (int i = 0; i < buttonCount; ++i)
        {
            //UsersSelected = ProfileButtons[i].User.UserNameID | UsersSelected;
            ProfileButtons[i].gameObject.SetActive(value);
        }
    }
}
