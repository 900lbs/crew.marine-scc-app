using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWidgetDisable : UI_Button
{

    [SerializeField] Widget[] widgets;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        int count = widgets.Length;
        for (int i = 0; i < count; i++)
        {
            if(widgets[i].State != WidgetState.Disabled)
                widgets[i].ToggleVisibility();
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
