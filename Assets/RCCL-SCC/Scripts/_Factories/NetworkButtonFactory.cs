using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;

public class NetworkButtonFactory : IFactory<Object, NetworkAction, NetworkButton>
{    
    readonly DiContainer container;

    [Inject]
    public NetworkButtonFactory(DiContainer _container)
    {
        container = _container;
    }

    public NetworkButton Create(Object prefab, NetworkAction action)
    {
        NetworkButton newNetworkButton = container.InstantiatePrefabForComponent<NetworkButton>(prefab);
        newNetworkButton.NetworkAction = action;
        newNetworkButton.name = action.ToString() + "Button";

        switch (action)
        {
            case NetworkAction.Create:
                newNetworkButton.ButtonText.text = "Create Online";
                return newNetworkButton;


            case NetworkAction.Join:
                newNetworkButton.ButtonText.text = "Connect";
                return newNetworkButton;


            case NetworkAction.OfflineMode:
                newNetworkButton.ButtonText.text = "Work Offline";
                return newNetworkButton;

            case NetworkAction.Rejoin:
            newNetworkButton.ButtonText.text = "Retry";
            return newNetworkButton;

            case NetworkAction.StartOver:
            newNetworkButton.ButtonText.text = "EXIT TO MENU";
            return newNetworkButton;

            default:
                return null;
        }
    }
}
