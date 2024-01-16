using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptLegendsButton : PromptAction
{
    [SerializeField] ToggleSwitch toggle;

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();
        toggle.Toggle();
    }

    public override void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal) { }


}
