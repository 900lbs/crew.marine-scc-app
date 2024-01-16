// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="Button_ControlWidget.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
/// <summary>
/// Struct WidgetToggle
/// </summary>
[System.Serializable]
public struct WidgetToggle
{
    /// <summary>
    /// The toggle rect
    /// </summary>
    public RectTransform ToggleRect;
    /// <summary>
    /// The target position
    /// </summary>
    public Vector2 TargetPosition;
}

#if SCC_2_5
/// <summary>
/// Created By: Joshua Bowers - 03/13/19
/// Last Edited By: Joshua Bowers - 06/14/19
/// Purpose: Multi-use button that controls whichever selected widget.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class Button_ControlWidget : UI_Button
{
    #region Injection Construction
    /// <summary>
    /// The annotation manager
    /// </summary>
    AnnotationManager annotationManager;

    /// <summary>
    /// Constructs the specified anno man.
    /// </summary>
    /// <param name="annoMan">The anno man.</param>
    [Inject]
    public void Construct(AnnotationManager annoMan)
    {
        annotationManager = annoMan;
    }

    #endregion
    /// <summary>
    /// The widget state
    /// </summary>
    [SerializeField][ReadOnly] WidgetState widgetState;

    /// <summary>
    /// The optional assigned properties
    /// </summary>
    [EnumFlag] public ShipRoomProperties OptionalAssignedProperties;

    /// <summary>
    /// The assigned widget interaction
    /// </summary>
    public WidgetInteraction AssignedWidgetInteraction;

    /// <summary>
    /// The current widget
    /// </summary>
    public Widget CurrentWidget;

    /// <summary>
    /// The optional move object
    /// </summary>
    public WidgetToggle OptionalMoveObject;

    /// <summary>
    /// The optional move object cached position
    /// </summary>
    Vector2 optionalMoveObjectCachedPos;

    /// <summary>
    /// The color profiles
    /// </summary>
    public ColorProfile[] ColorProfiles;

    bool isLegendsToggled;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Required to validate the color profiles, this way we can initialize the colors in editor and choose visually.
    /// </summary>
    void OnValidate()
    {
        if (CurrentWidget == null)
        {
            Debug.LogError(name + " is missing it's widget reference, please assign.", this);
            return;
        }

        int colorProfilesCount = ColorProfiles.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            ColorProfiles[i].OnValidate();
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [enable].
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        CurrentWidget.AddListener(gameObject, StateChange);
        if (OptionalAssignedProperties != 0)
            _signalBus.Subscribe<Signal_AnnoMan_OnRoomPropertyChanged>(AnnoMan_OnRoomPropertyChanged);

        _signalBus.Subscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
        if (OptionalMoveObject.ToggleRect != null)
            optionalMoveObjectCachedPos = OptionalMoveObject.ToggleRect.anchoredPosition;

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [disable].
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        CurrentWidget.RemoveListener(gameObject);
        if (OptionalAssignedProperties != 0)
            _signalBus.TryUnsubscribe<Signal_AnnoMan_OnRoomPropertyChanged>(AnnoMan_OnRoomPropertyChanged);

        _signalBus.TryUnsubscribe<Signal_MainMenu_OnPromptMenuChanged>(PromptMenuChanged);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        ButtonInteraction(AssignedWidgetInteraction);
    }

    /// <summary>
    /// Gross
    /// </summary>
    /// <param name="signal"></param>
    void AnnoMan_OnRoomPropertyChanged(Signal_AnnoMan_OnRoomPropertyChanged signal)
    {
        if (OptionalAssignedProperties.HasFlag(ShipRoomProperties.GAOverlay))
        {
            ColorChange((signal.roomProperty.HasFlag(OptionalAssignedProperties) ? ActiveState.Enabled : ActiveState.Disabled));
            //Debug.Log(name + " property: " + OptionalAssignedProperties + " / Received Signal property: " + signal.roomProperty, this);
        }
    }

    void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        if (signal.PromptType == PromptMenuType.Legends && AssignedWidgetInteraction == WidgetInteraction.Toggle)
        {
            isLegendsToggled = !isLegendsToggled;
            button.interactable = !isLegendsToggled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Buttons the interaction.
    /// </summary>
    /// <param name="interaction">The interaction.</param>
    void ButtonInteraction(WidgetInteraction interaction)
    {
        switch (interaction)
        {
            case WidgetInteraction.Dock:
                if (CurrentWidget.State != WidgetState.Disabled)
                    CurrentWidget.ToggleDocking();
                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            case WidgetInteraction.Toggle:
                if (OptionalAssignedProperties != 0 && !annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties))
                {
                    annotationManager.SetCurrentPropertyState(OptionalAssignedProperties);
                    if (CurrentWidget.State != WidgetState.Disabled && OptionalAssignedProperties.HasFlag(ShipRoomProperties.GAOverlay))
                        return;
                }

                if (OptionalAssignedProperties != 0 && annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties) &&
                    CurrentWidget.State != WidgetState.Disabled)
                {
                    annotationManager.SetCurrentPropertyState(0);
                    CurrentWidget.ToggleVisibility();
                    return;
                }

                CurrentWidget.ToggleVisibility();
                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            case WidgetInteraction.ToggleIfDisabled:

                if (OptionalAssignedProperties != 0 && !annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties))
                {
                    annotationManager.SetCurrentPropertyState(OptionalAssignedProperties);
                    if (CurrentWidget.State != WidgetState.Disabled && OptionalAssignedProperties.HasFlag(ShipRoomProperties.GAOverlay))
                        return;
                }

                if (OptionalAssignedProperties != 0 && annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties) &&
                    CurrentWidget.State != WidgetState.Disabled)
                {
                    annotationManager.SetCurrentPropertyState(0);
                    CurrentWidget.ToggleVisibility();
                    return;
                }
                if (CurrentWidget.State == WidgetState.Disabled)
                    CurrentWidget.ToggleVisibility();
                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            case WidgetInteraction.Slide:
                if (OptionalAssignedProperties != 0 && !annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties))
                {
                    annotationManager.SetCurrentPropertyState(OptionalAssignedProperties);
                    if (CurrentWidget.State != WidgetState.Disabled && OptionalAssignedProperties.HasFlag(ShipRoomProperties.GAOverlay))
                        return;
                }

                if (OptionalAssignedProperties != 0 && annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties) &&
                    CurrentWidget.State != WidgetState.Disabled)
                {
                    annotationManager.SetCurrentAnnotationState(AnnotationState.Move, null);
                    annotationManager.SetCurrentPropertyState(0);
                    Debug.Log("Sliding: " + CurrentWidget.name, this);
                    CurrentWidget.ToggleSliding();
                    return;
                }

                Debug.Log("Sliding: " + CurrentWidget.name, this);
                CurrentWidget.ToggleSliding();
                break;

                /*----------------------------------------------------------------------------------------------------------------------------*/

            default:
                Debug.LogError(interaction.ToString() + " called on " + name + " but is not a viable option.", this);
                break;

        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// States the change.
    /// </summary>
    /// <param name="state">The state.</param>
    public void StateChange(WidgetState state)
    {

        if (OptionalMoveObject.ToggleRect != null)
            OptionalToggle(state != WidgetState.Disabled);

        switch (AssignedWidgetInteraction)
        {
            case WidgetInteraction.Dock:

                switch (state)
                {
                    case WidgetState.Disabled:

                        ColorChange(ActiveState.Disabled);

                        break;
                    case WidgetState.Docked:

                        ColorChange(ActiveState.Enabled);
                        break;
                    case WidgetState.Idle:
                    case WidgetState.Moving:

                        ColorChange(ActiveState.Selected);
                        break;
                }
                break;

            case WidgetInteraction.Toggle:
            case WidgetInteraction.Slide:

                if (state == WidgetState.Disabled)
                {
                    ColorChange(ActiveState.Disabled);
                }

                else
                {
                    if ((OptionalAssignedProperties != 0 && annotationManager.GetCurrentPropertyState().HasFlag(OptionalAssignedProperties) || OptionalAssignedProperties == 0))
                        ColorChange(ActiveState.Enabled);
                }
                break;
        }
        widgetState = state;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Ghetto AF but whatever.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    void OptionalToggle(bool value)
    {
        if (value && OptionalMoveObject.ToggleRect.anchoredPosition != OptionalMoveObject.TargetPosition)
        {
            OptionalMoveObject.ToggleRect.DOAnchorPos(OptionalMoveObject.TargetPosition, 0.25f);
        }
        else if (!value && OptionalMoveObject.ToggleRect.anchoredPosition != optionalMoveObjectCachedPos)
        {
            OptionalMoveObject.ToggleRect.DOAnchorPos(optionalMoveObjectCachedPos, 0.25f);
        }
    }

    void ColorChange(ActiveState state)
    {
        int colorProfilesCount = ColorProfiles.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            ColorProfiles[i].StateChange(state);
        }
    }

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange() { }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif