using UnityEngine;
using PrimitiveFactory.ScriptableObjectSuite;

using Zenject;
public class PromptAction : UI_Button
{
    public PromptMenuType PromptMenuType;
    public PromptMenuAction AssignedMenuAction;

    public virtual void Start()
    {
        _signalBus.Subscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    protected override void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }
    protected override void BTN_OnClick()
    {
        _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType));
    }

    protected override void OnStateChange()
    {

    }

    public virtual void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal) { }
}