using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
public class FloatingCanvasController : _MenuController
{
    [Inject]
    SignalBus _signalBus;

    void Start()
    {
        _signalBus.Subscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        if(signal.PromptType == PromptMenuType.Legends)
            FadeCanvas((signal.PromptType == PromptMenuType.Legends && cg.alpha == 0));
    }
}
