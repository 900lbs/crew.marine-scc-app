using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Zenject;
[RequireComponent(typeof(TextMeshProUGUI))]
public class ShipName : MonoBehaviour
{

    SignalBus _signalBus;
    [Inject]
    public void Construct(SignalBus signal)
    {
        _signalBus = signal;
    }

    TextMeshProUGUI ShipTitle;

    private void Awake()
    {
        if (ShipTitle == null)
            ShipTitle = GetComponent<TextMeshProUGUI>();

        _signalBus.Subscribe<Signal_MainMenu_OnShipInitialize>(ShipInitialize);
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnShipInitialize>(ShipInitialize);
    }

    void ShipInitialize(Signal_MainMenu_OnShipInitialize signal)
    {
        ShipTitle.text = signal.Ship.ToString().ToUpper();
    }

}
