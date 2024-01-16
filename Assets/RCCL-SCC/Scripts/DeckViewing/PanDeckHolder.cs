/*
 * © 900lbs of Creative
 * Creation Date: DATE HERE
 * Date last Modified: MOST RECENT MODIFICATION DATE HERE
 * Name: AUTHOR NAME HERE
 * 
 * Description: DESCRIPTION HERE
 * 
 * Scripts referenced: LIST REFERENCED SCRIPTS HERE
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Behaviors;

using Zenject;

#if SCC_2_5

public enum PanDeckType
{
    Decks,
    Legends
}

public class PanDeckHolder : MonoBehaviour
{
    [Inject]
    AnnotationManager annotationManager;

    [Inject]
    SignalBus _signalBus;

    public ScreenTransformGesture screenTransformGesture;
    public Transformer transformer;

    public CanvasScaler canvasScaler;

    public GameObject eventSystem;

    public ZoomOut zoomOut;

    public RectTransform selectionHolder;

    Vector3 newPos;

    public float multiplier;

    public PanDeckType PanDeck;

    public bool IsPanningEnabled;

    private void Start()
    {
        if (PanDeck == PanDeckType.Legends)
        {
            IsPanningEnabled = false;
            ToggleTouch(false);
        }
        zoomOut = FindObjectOfType<ZoomOut>();
        _signalBus.Subscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);

    }

    //TODO Fix positioning of selectionHolder
    void Update()
    {
        if (PanDeck == PanDeckType.Decks)
            ToggleTouch(annotationManager.GetCurrentAnnotationState() == AnnotationState.Move && Input.touchCount < 2);
    }

    void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        ToggleTouch((signal.PromptType == PromptMenuType.Legends) && !IsPanningEnabled);

    }

    public void ToggleTouch(bool value)
    {
        screenTransformGesture.enabled = value;
        transformer.enabled = value;
        IsPanningEnabled = value;

        screenTransformGesture.multiplier = multiplier;
    }
}

#elif !SCC_2_5
public class PanDeckHolder : MonoBehaviour
{
    public ScreenTransformGesture screenTransformGesture;
    public Transformer transformer;

    public CanvasScaler canvasScaler;

    public GameObject eventSystem;

    public DrawOnDeck drawOnDeck;
    public ZoomOut zoomOut;

    public RectTransform selectionHolder;

    Vector3 newPos;

    public float multiplier;

    private void Start()
    {
        zoomOut = FindObjectOfType<ZoomOut>();
    }

    //TODO Fix positioning of selectionHolder
    void Update () 
    {
        if(!drawOnDeck.draw && !drawOnDeck.erase && !drawOnDeck.placeIcon && Input.touchCount < 2)
        {
            //eventSystem.GetComponent<TouchScript.Layers.UI.TouchScriptInputModule>().enabled = true;
            screenTransformGesture.enabled = true;
            transformer.enabled = true;

            screenTransformGesture.multiplier = multiplier;
        }

        else
        {
            //eventSystem.GetComponent<TouchScript.Layers.UI.TouchScriptInputModule>().enabled = false;
            screenTransformGesture.enabled = false;
            transformer.enabled = false;
        }
    }
}
#endif