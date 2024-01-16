// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 02-05-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="ButtonHighlight.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Zenject;

#if SCC_2_5

/// <summary>
/// Handles the art widget buttons, mainly tells the annotation manager what state we're now in and listens for whether it's the
/// active button or not.
/// Implements the <see cref="UI_Button" />
/// Implements the <see cref="Zenject.ILateDisposable" />
/// </summary>
/// <seealso cref="UI_Button" />
/// <seealso cref="Zenject.ILateDisposable" />
/// <remarks>This is because we now have other objects calling in state changes, buttons need to be ready in every way.</remarks>
public class ButtonHighlight : UI_Button
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
    /// <param name="signal">The signal.</param>
    [Inject]
    public void Construct(AnnotationManager annoMan)
    {
        annotationManager = annoMan;
    }

    #endregion

    /// <summary>
    /// The assigned annotation state
    /// </summary>
    public AnnotationState AssignedAnnotationState;

    /// <summary>
    /// The profiles
    /// </summary>
    public ColorProfile[] Profiles;

    /// <summary>
    /// The highlighted
    /// </summary>
    public bool highlighted;
    /// <summary>
    /// The erase all
    /// </summary>
    public bool eraseAll;

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [validate].
    /// </summary>
    void OnValidate()
    {
        int colorProfilesCount = Profiles.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            Profiles[i].OnValidate();
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /*     protected override void UIManager_OnInitialize()
		{
		} */

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        _signalBus.Subscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnotationStateChanged);
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange() { }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Lates the dispose.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        _signalBus.Unsubscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnotationStateChanged);
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        annotationManager.SetCurrentAnnotationState(AssignedAnnotationState, null);
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Annotations the state changed.
    /// </summary>
    /// <param name="stateData">The state data.</param>
    void AnnotationStateChanged(Signal_AnnoMan_OnAnnotationStateChanged stateData)
    {
        if (AssignedAnnotationState == stateData.annotationState)
        {
            Selected(true);
        }
        else
        {
            Selected(false);
        }
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Selecteds the specified toggled.
    /// </summary>
    /// <param name="toggled">if set to <c>true</c> [toggled].</param>
    void Selected(bool toggled)
    {
        SetProfileState((toggled) ? ActiveState.Selected : ActiveState.Enabled);
    }

    /// <summary>
    /// Sets the state of the profile.
    /// </summary>
    /// <param name="state">The state.</param>
    void SetProfileState(ActiveState newState)
    {
        int colorProfilesCount = Profiles.Length;
        for (int i = 0; i < colorProfilesCount; ++i)
        {
            Profiles[i].StateChange(newState);
        }
    }

}

#endif