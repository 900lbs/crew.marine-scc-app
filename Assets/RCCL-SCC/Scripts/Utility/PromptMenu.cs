using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using Zenject;
public enum PromptMenuType
{
    Erase,
    Reset,
    Quit,
    ShareMyView,
    Legends,
    KidsTracking,
    KidFound
}

public class PromptMenu : _MenuController
{
    #region Injection Construction
    UI_Button.Factory buttonFactory;
    SignalBus _signalBus;

    [Inject]
    public void Construct(UI_Button.Factory buttonFact, SignalBus signal)
    {
        buttonFactory = buttonFact;
        _signalBus = signal;
    }
    #endregion

    public PromptMenuType PromptType;

    public TextMeshProUGUI MessageTextObject;

    [HideInInspector]
    public Prompt TargetPrompt;


    /*----------------------------------------------------------------------------------------------------------------------------*/


    public override void Awake()
    {
        base.Awake();

        FadeCanvas(false);

        _signalBus.Subscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
        _signalBus.Subscribe<Signal_MainMenu_OnShipInitialized>(ShipInitialized);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnShipInitialized>(ShipInitialized);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void AssignMessage(string message)
    {
        MessageTextObject.text = message;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void SpawnButtons(GameObject[] buttons)
    {
        foreach (var item in buttons)
        {
            UI_Button newButton = buttonFactory.Create(item);
            newButton.transform.parent = ButtonsParent;
            newButton.GetComponent<Button>().onClick.AddListener(DestroyPrompt);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void DestroyPrompt()
    {
        Destroy(gameObject);
    }
    protected virtual void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        if(signal.PromptType == PromptType && signal.OptionalParams.Length > 0)
        {
            AssignMessage((string) signal.OptionalParams[0]);
        }

        FadeCanvas((signal.PromptType == PromptType && cg.alpha == 0));
        
    }

    protected virtual void ShipInitialized()
    {

    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, PromptMenu> { }
}
