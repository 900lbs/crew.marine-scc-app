using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class NetworkConnectivityCheck : MonoBehaviour
{
    [Inject]
    SignalBus _signalBus;

    public bool ShowIfOnline;

    public bool isFade;

    CanvasGroup canvasGroup;

    void Start()
    {
        _signalBus.Subscribe<Signal_NetworkClient_OnOnlineOfflineToggled>(OnlineOfflineToggled);

        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnOnlineOfflineToggled>(OnlineOfflineToggled);
    }

    void OnlineOfflineToggled(Signal_NetworkClient_OnOnlineOfflineToggled signal)
    {
        if (ShowIfOnline)
        {
            if (!isFade)
                gameObject.SetActive(signal.IsOnline);
            else
            {
                canvasGroup.alpha = (signal.IsOnline) ? 1 : 0.25f;
                canvasGroup.interactable = (signal.IsOnline);
            }

        }
        else
        {
            if (!isFade)
                gameObject.SetActive(!signal.IsOnline);
            else
            {
                canvasGroup.alpha = (signal.IsOnline) ? 0.25f : 1f;
                canvasGroup.interactable = (!signal.IsOnline);
            }
        }
    }
}