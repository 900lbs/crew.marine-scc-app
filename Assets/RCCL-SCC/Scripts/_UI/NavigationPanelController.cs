using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Zenject;
public class NavigationPanelController : MonoBehaviour
{
    [Inject]
    SignalBus _signalBus;
    public float ClosedYValue;

    RectTransform panelRect;

    void Start()
    {
        _signalBus.Subscribe<Signal_EniramData_DataUpdated>(DataUpdate);

        panelRect = GetComponent<RectTransform>();
    }

    void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_EniramData_DataUpdated>(DataUpdate);
    }

    void DataUpdate(Signal_EniramData_DataUpdated signal)
    {
        //Debug.Log("Toggling NavigationPanel " + ((signal.Info == null) ? "'Off'" : "'On'"), this);
        TogglePanel(signal.Info != null);
    }

    void TogglePanel(bool value)
    {
        if (NetworkClient.CurrentUserProfile != null)
            panelRect.DOAnchorPos((value && NetworkClient.GetUserName().HasFlag(UserNameID.SafetyCommandCenter)) ? Vector2.zero : new Vector2(0, ClosedYValue), 0.25f, true);
    }
}
