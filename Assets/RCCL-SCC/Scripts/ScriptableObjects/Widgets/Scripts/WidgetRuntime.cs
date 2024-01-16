// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="WidgetRuntime.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using DG.Tweening;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// The widget runtime component that handles all of the scene logic.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
#if SCC_2_5
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class WidgetRuntime : MonoBehaviour
{
    #region Injection Construction
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;
    /// <summary>
    /// The annotation manager
    /// </summary>
    AnnotationManager annotationManager;
    XMLWriterDynamic.Factory xmlWriterFactory;

    /// <summary>
    /// Constructs the specified signal.
    /// </summary>
    /// <param name="signal">The signal.</param>
    /// <param name="annoMan">The anno man.</param>
    [Inject]
    public void Construct(SignalBus signal, AnnotationManager annoMan, XMLWriterDynamic.Factory xmlWriterFact)
    {
        _signalBus = signal;
        annotationManager = annoMan;
        xmlWriterFactory = xmlWriterFact;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The current widget
    /// </summary>
    public Widget CurrentWidget;

    /// <summary>
    /// The runtime widget rect
    /// </summary>
    public RectTransform RuntimeWidgetRect;

    /// <summary>
    /// The widget cg
    /// </summary>
    [HideInInspector]
    public CanvasGroup widgetCG;

    /// <summary>
    /// The optional toggle
    /// </summary>
    public ToggleSwitch OptionalToggle;

    /// <summary>
    /// The assigned tool
    /// </summary>
    public ActiveAnnotationTools AssignedTool;

    /// <summary>
    /// The x reference
    /// </summary>
    public float xReference;

    public float xScaleAdjustment = 1;

    /// <summary>
    /// The element to slide
    /// </summary>
    public RectTransform ElementToSlide;

    /// <summary>
    /// The in position
    /// </summary>
    public Vector2 inPos;
    /// <summary>
    /// The out position
    /// </summary>
    public Vector2 outPos;

    [SerializeField] bool shouldUseParentAsSiblingSet;

    /// <summary>
    /// The gesture
    /// </summary>
    ScreenTransformGesture gesture;

    XMLWriterDynamic xmlWriter;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        gesture = GetComponent<ScreenTransformGesture>();

        if (gesture != null)
            gesture.StateChanged += GestureStateChanged;

        if (widgetCG == null)
            widgetCG = GetComponent<CanvasGroup>();

        if (RuntimeWidgetRect == null)
            RuntimeWidgetRect = GetComponent<RectTransform>();

        CurrentWidget.Add(CurrentWidget);
        CurrentWidget.AddListener(gameObject, StateChange);
        CurrentWidget.RuntimeReference = this;

        xmlWriter = xmlWriterFactory.Create(gameObject, XMLType.Widgets);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [destroy].
    /// </summary>
    void OnDestroy()
    {
        CurrentWidget.Remove(CurrentWidget);
        CurrentWidget.RemoveListener(gameObject);
        if (gesture != null)
            gesture.StateChanged -= GestureStateChanged;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    void OnValidate()
    {
        if (CurrentWidget && CurrentWidget.RuntimeReference == null)
            CurrentWidget.RuntimeReference = this;

        if (CurrentWidget == null)
            Debug.LogError(name + " is missing it's widget reference, please assign.", this);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Lates the update.
    /// </summary>
    void LateUpdate()
    {
        if ((CurrentWidget.State == WidgetState.Moving) && RuntimeWidgetRect.anchoredPosition.x <= 200)
        {
            if (RuntimeWidgetRect.anchoredPosition.y <= 0)
            {
                //CurrentWidget.State = WidgetState.Idle;
                CurrentWidget.ToggleDocking();
                gesture.Cancel();
            }
        }

        if (xReference != RuntimeWidgetRect.sizeDelta.x)
        {
            xReference = RuntimeWidgetRect.sizeDelta.x * xScaleAdjustment;

            if (CurrentWidget.State == WidgetState.Docked)
            {
                WidgetManager.OnUpdateDockedWidgets(CurrentWidget);
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gestures the state changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="GestureStateChangeEventArgs"/> instance containing the event data.</param>
    void GestureStateChanged(object sender, GestureStateChangeEventArgs args)
    {
        if (args.State == Gesture.GestureState.Began)
        {
            if (CurrentWidget.State == WidgetState.Docked)
            {
                CurrentWidget.Manager.AddOrRemoveDockedWidget(CurrentWidget);
            }

            CurrentWidget.State = WidgetState.Moving;
            if (shouldUseParentAsSiblingSet)
                RuntimeWidgetRect.parent.SetAsLastSibling();
            else
                RuntimeWidgetRect.SetAsLastSibling();
        }
        else if (args.State == Gesture.GestureState.Ended)
        {
            CurrentWidget.State = WidgetState.Idle;
        }
    }

    /// <summary>
    /// Toggles the visibility.
    /// </summary>
    /// <param name="state">The state.</param>
    public async void ToggleVisibility(WidgetState state)
    {
        if (OptionalToggle != null)
        {
            OptionalToggle.Toggle();
        }
        switch (state)
        {
            case WidgetState.Disabled:
                widgetCG.alpha = 0;
                widgetCG.interactable = false;
                widgetCG.blocksRaycasts = false;
                await xmlWriter.AttemptCustomSave((CurrentWidget.FileName + ":Off"));
                await xmlWriter.Save();
                break;

            case WidgetState.Idle:
            case WidgetState.Docked:
                widgetCG.alpha = 1;
                widgetCG.interactable = true;
                widgetCG.blocksRaycasts = true;
                if (state == WidgetState.Idle)
                {
                    await xmlWriter.AttemptCustomSave((CurrentWidget.FileName + ":On"));
                    await xmlWriter.Save();
                }
                break;
        }

        if (CurrentWidget.AssignedTool == ActiveAnnotationTools.Art)
        {
            if (state == WidgetState.Disabled)
                annotationManager.SetCurrentAnnotationState(AnnotationState.Move, null);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Toggles the sliding.
    /// </summary>
    public void ToggleSliding()
    {
        switch (CurrentWidget.State)
        {
            case WidgetState.Disabled:
                ElementToSlide.DOAnchorPos(outPos, 0.35f);
                break;

            case WidgetState.Idle:
                ElementToSlide.DOAnchorPos(inPos, 0.35f);
                break;

            default:
                break;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// States the change.
    /// </summary>
    /// <param name="state">The state.</param>
    void StateChange(WidgetState state)
    {

        switch (state)
        {
            case WidgetState.Disabled:
                annotationManager.SetCurrentAnnotationTools(AssignedTool, false);
                break;

            case WidgetState.Idle:
                annotationManager.SetCurrentAnnotationTools(AssignedTool, true);

                break;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif