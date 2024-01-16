using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;

public class PromptEraseAllButton : PromptAction
{
    [Inject]
    protected AnnotationManager annotationManager;

    public UserProfileSelectButtonHandler ButtonHandler;

    internal UserNameID localUser
    { get { return NetworkClient.GetUserName(); } }

    protected override void OnStateChange()
    {
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        //Debug.Log("Clicked: " + AssignedMenuAction.ToString(), this);
        switch (AssignedMenuAction)
        {
            case PromptMenuAction.Prompt:
                _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.Erase));
                break;

            case PromptMenuAction.Cancel:
                ButtonHandler.AllUsersDeselected();

                _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.Erase));
                annotationManager.SetCurrentAnnotationState(AnnotationState.Move, null);

                break;

            case PromptMenuAction.Confirmation:
                annotationManager.SetCurrentAnnotationState(AnnotationState.Move, null);
                AnnoMan_OnEraseAllOnProperty();
                break;
        }
    }

    private async Task AnnoMan_OnEraseAllOnProperty()
    {
        UserNameID userNameID = (ButtonHandler != null) ? ButtonHandler.UsersSelected : NetworkClient.GetUserName();
        Debug.Log("Erase all on property, pressed.", this);
        if (ButtonHandler != null && NetworkClient.GetIsPlayerMasterClient()) //Basically., ButtonHandler is only used if it's a Table user that can delete any layer.
        {
            Debug.Log("Found a button handler.", this);
            if (userNameID != 0)
            {
                Debug.Log("Erasing all - via network | " + userNameID, this);

                NetworkAnnotationObject eraseAllNetworkObject = new NetworkAnnotationObject(
                NetworkEvent.EraseAll,
                annotationManager.GetCurrentPropertyState(),
                (NetworkStorageType.LineRenderer | NetworkStorageType.Icon),
                userNameID.ToString(),
                 0,
                 userNameID,
                 true);

                await networkClient.SendNewNetworkEvent(eraseAllNetworkObject);
                _signalBus.Fire(new Signal_AnnoMan_OnEraseAllOnProperty(annotationManager.GetCurrentPropertyState(), userNameID));

                return;
            }

            if (annotationManager.GetCurrentPropertyState().HasFlag(ShipRoomProperties.GAOverlay))
            {
                _signalBus.Fire(new Signal_AnnoMan_OnEraseAllOnProperty(ShipRoomProperties.GAOverlay, NetworkClient.GetUserName()));
                return;
            }
            else
            {
                if (ButtonHandler.UsersSelected == 0)
                {
                    Debug.Log("No users selected, nothing will be deleted.");
                    return;
                }

                //Debug.Log("Erasing local only from property: " + annotationManager.GetCurrentPropertyState(), this);
                _signalBus.Fire<Signal_AnnoMan_OnEraseAllOnProperty>(new Signal_AnnoMan_OnEraseAllOnProperty(annotationManager.GetCurrentPropertyState(), NetworkClient.GetUserName()));
            }

            ButtonHandler.AllUsersDeselected();
        }
        else if (!NetworkClient.GetIsPlayerMasterClient() || Photon.Pun.PhotonNetwork.OfflineMode)
        {
            Debug.Log("Not masterclient, erasing current player annotations.", this);

            NetworkAnnotationObject eraseAllNetworkObject = new NetworkAnnotationObject(
                            NetworkEvent.EraseAll,
                            annotationManager.GetCurrentPropertyState(),
                            (NetworkStorageType.LineRenderer | NetworkStorageType.Icon),
                            localUser.ToString(),
                            0,
                            localUser,
                            true);

            await networkClient.SendNewNetworkEvent(eraseAllNetworkObject);
            Debug.Log("Successfully sent EraseAll command.", this);

            _signalBus.Fire<Signal_AnnoMan_OnEraseAllOnProperty>(new Signal_AnnoMan_OnEraseAllOnProperty(annotationManager.GetCurrentPropertyState(), NetworkClient.GetUserName()));
        }

        annotationManager.SetCurrentAnnotationState(AnnotationState.Move, null);
        _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.Erase));
    }
}