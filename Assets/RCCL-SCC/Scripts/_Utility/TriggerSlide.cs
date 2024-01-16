using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSlide : UI_Button
{
    [SerializeField] SlideUIElement[] slideUIElement;
    [SerializeField] Widget[] widgetsToSlide;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();
        for (int i = 0; i < slideUIElement.Length; i++)
        {
            if (slideUIElement[i].state == SlideUIElement.State.isOut)
                slideUIElement[i].SlideElement();
        }

        for (int i = 0; i < widgetsToSlide.Length; i++)
        {
            if(widgetsToSlide[i].State == WidgetState.Idle)
                widgetsToSlide[i].ToggleSliding();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnStateChange()
    {
    }
}
