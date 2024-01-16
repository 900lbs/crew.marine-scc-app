using System;
using UnityEngine;
using Zenject;

public class PromptShareViewButton : PromptAction
{
    [SerializeField] bool isKidTracking;
    ShareReceiveView shareReceiveView;

    [Inject]
    public void Construct(ShareReceiveView shareView)
    {
        shareReceiveView = shareView;
    }

    ShareViewData currentShareData;

    public override void Start()
    {
        base.Start();
        _signalBus.Subscribe<Signal_NetworkClient_OnNetworkEventReceived>(OnNetworkEventReceived);

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnNetworkEventReceived>(OnNetworkEventReceived);

    }

    void OnNetworkEventReceived(Signal_NetworkClient_OnNetworkEventReceived signal)
    {
        if (signal.NetworkObject.GetType() == typeof(NetworkShareObject))
        {
            Debug.Log("Share view accepted type: " + signal.NetworkObject.GetType().ToString());
            NetworkShareObject networkObject = (NetworkShareObject) signal.NetworkObject;
            UserNameID tryUser = 0;
            if (Enum.TryParse(networkObject.PlayerID, out tryUser))
            {
                string message = "";
                message = "You have received a view from <color=#00FFED>" + UserProfileFactory.GetAbbreviatedName(tryUser) + "</color>.";
                Debug.Log(message);

                _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.ShareMyView, message));
            }
            else
            {
                Debug.LogError("Could not parse userID name from shareobject.");
            }

            string data = (string) networkObject.NetData;
            currentShareData = ShareViewData.ConvertFromString(data);

        }
    }

    protected override void BTN_OnClick()
    {
        if (AssignedMenuAction == PromptMenuAction.Confirmation)
        {
            if (currentShareData != null)
            {
                shareReceiveView.ReceiveView(currentShareData);
            }
        }

        _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.ShareMyView));
    }

    protected override void OnStateChange()
    {

    }

}