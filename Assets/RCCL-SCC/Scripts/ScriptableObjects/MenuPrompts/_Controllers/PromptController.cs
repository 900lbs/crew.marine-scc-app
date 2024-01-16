using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptController : UI_Button
{
    public Prompt MenuPrompt;

    [Space(5f)]
    [Header("Optional XML")]
    public XMLType OptionalXMLType;
    public XMLWriterDynamic OptionalXMLWriter;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();
        Debug.Log("Clicked to create menu prompt", this);
        MenuPrompt.CreatePrompt();
    }

    protected override void OnStateChange()
    {
        switch (State)
        {
            case ActiveState.Disabled:
            case ActiveState.Enabled:
            case ActiveState.Selected:
                break;
        }
    }
}
