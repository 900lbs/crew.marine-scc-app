// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-08-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="VerticalViewButton.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Zenject;

#if SCC_2_5
/// <summary>
/// Class VerticalViewButton.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class VerticalViewButton : UI_Button
{
    #region Injection Construction
    /// <summary>
    /// The zoom in to deck
    /// </summary>
    IZoomInDeck zoomInToDeck;
    /// <summary>
    /// The annotation manager
    /// </summary>
    AnnotationManager annotationManager;

    /// <summary>
    /// Constructs the specified zoom deck.
    /// </summary>
    /// <param name="zoomDeck">The zoom deck.</param>
    /// <param name="anoMan">The ano man.</param>
    [Inject]
    public void Construct(IZoomInDeck zoomDeck, AnnotationManager anoMan)
    {
        zoomInToDeck = zoomDeck;
        annotationManager = anoMan;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The color change objects
    /// </summary>
    public ColorProfile ColorChangeObjects;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    bool IsRotated = false;

    /// <summary>
    /// Called when [validate].
    /// </summary>
    void OnValidate()
    {
        ColorChangeObjects.OnValidate();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        zoomInToDeck.rotateThemDecks();

        State = (annotationManager.Rotated) ? ActiveState.Selected : ActiveState.Enabled;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange()
    {
        if (disableTween != null)
        {
            if (disableTween.IsPlaying())
            {
                disableTween.OnComplete(() =>
                {
                    button.interactable = (State == ActiveState.Disabled) ? false : true;
                    //Debug.Log("Tween was already playing, set " + name + "'s button interactable to " + button.interactable + " after completion.", this);
                });
            }
        }

        else
        {
            button.interactable = (State == ActiveState.Disabled) ? false : true;
        }

        switch (State)
        {
            case ActiveState.Disabled:
                break;

            case ActiveState.Enabled:
                ColorChangeObjects.StateChange(ActiveState.Enabled);
                break;
            case ActiveState.Selected:
                ColorChangeObjects.StateChange(ActiveState.Selected);
                break;
        }
    }

    void Update()
    {
        if(IsRotated != annotationManager.Rotated)
        {
            State = (annotationManager.Rotated) ? ActiveState.Selected : ActiveState.Enabled;
            IsRotated = annotationManager.Rotated;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /*     protected override void UIManager_OnInitialize()
        {
            State = ActiveState.Enabled;
        } */

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif