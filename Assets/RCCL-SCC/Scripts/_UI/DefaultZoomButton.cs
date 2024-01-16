// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-08-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="DefaultZoomButton.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

#if SCC_2_5
/// <summary>
/// Handles getting our canvas back to default state.
/// Implements the <see cref="UI_Button" />
/// Implements the <see cref="Zenject.IInitializable" />
/// </summary>
/// <seealso cref="UI_Button" />
/// <seealso cref="Zenject.IInitializable" />
public class DefaultZoomButton : UI_Button, IInitializable
{
	#region Injection Construction
	/// <summary>
	/// The pan holder
	/// </summary>
	PanDeckHolder panHolder;
	/// <summary>
	/// The zoom out
	/// </summary>
	ZoomOut zoomOut;

	/// <summary>
	/// Constructs the specified pan deck.
	/// </summary>
	/// <param name="panDeck">The pan deck.</param>
	/// <param name="zoom">The zoom.</param>
	[Inject]
    public void Construct(PanDeckHolder panDeck, ZoomOut zoom)
    {
        panHolder = panDeck;
        zoomOut = zoom;
    }
	#endregion
	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// The color change objects
	/// </summary>
	public ColorProfile ColorChangeObjects;

	/// <summary>
	/// The pan rect
	/// </summary>
	RectTransform panRect;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [enable].
	/// </summary>
	protected override void Awake()
    {
        base.Awake();

        ZoomOut.OnIsZoomedIn += ZoomOut_OnIsZoomedIn;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [disable].
	/// </summary>
	protected override void OnDestroy()
    {
        base.OnDestroy();

        ZoomOut.OnIsZoomedIn -= ZoomOut_OnIsZoomedIn;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public void Initialize()
    {
        panRect = panHolder.GetComponent<RectTransform>();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [validate].
	/// </summary>
	void OnValidate()
    {
        ColorChangeObjects.OnValidate();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Handles everything when the state is changed, from colors to images etc.
	/// </summary>
	protected override void OnStateChange()
    {
        switch (State)
        {
            case ActiveState.Disabled:
                button.interactable = false;
                ColorChangeObjects.StateChange(ActiveState.Disabled);
                break;

            case ActiveState.Enabled:
                button.interactable = true;
                ColorChangeObjects.StateChange(ActiveState.Enabled);
                break;

            case ActiveState.Selected:

                break;
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
	/// </summary>
	protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        zoomOut.EnableZoomOut();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Zooms the out on is zoomed in.
	/// </summary>
	/// <param name="value">if set to <c>true</c> [value].</param>
	void ZoomOut_OnIsZoomedIn(bool value)
    {
        if (value)
        {
            State = ActiveState.Enabled;
        }
        else
        {
            State = ActiveState.Disabled;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif