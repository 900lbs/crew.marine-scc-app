// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-17-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="ToggleCurrentShipPropertyHoldersButton.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using UnityEngine;

using Zenject;

/// <summary>
/// Simple button that toggles and annotations based on it's assigned property.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
[RequireComponent(typeof(ToggleSwitch))]
public class ToggleCurrentShipPropertyHoldersButton : UI_Button
{
	#region Injection Construction
	/// <summary>
	/// The annotation manager
	/// </summary>
	[Inject]
    AnnotationManager annotationManager;

	#endregion

	/// <summary>
	/// The assigned properties
	/// </summary>
	[EnumFlag] public ShipRoomProperties AssignedProperties;

	/// <summary>
	/// The multiple properties
	/// </summary>
	bool multipleProperties;

	/// <summary>
	/// The toggle
	/// </summary>
	ToggleSwitch toggle;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Handles everything when the state is changed, from colors to images etc.
	/// </summary>
	protected override void OnStateChange()
    {

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Awakes this instance.
	/// </summary>
	protected override void Awake()
    {
		base.Awake();
		
        _signalBus.Subscribe<Signal_AnnoMan_OnToggleRoomProperty>(RoomPropertyActiveToggled);

        toggle = GetComponent<ToggleSwitch>();

        multipleProperties = (((AssignedProperties & (AssignedProperties - 1)) != 0));
    }

	/// <summary>
	/// Called when [destroy].
	/// </summary>
	protected override void OnDestroy()
    {
		base.OnDestroy();

        _signalBus.Unsubscribe<Signal_AnnoMan_OnToggleRoomProperty>(RoomPropertyActiveToggled);

    }

	/// <summary>
	/// Rooms the property active toggled.
	/// </summary>
	/// <param name="signal">The signal.</param>
	void RoomPropertyActiveToggled(Signal_AnnoMan_OnToggleRoomProperty signal)
    {
        if (multipleProperties)
        {
            toggle.Toggle(signal.Property.HasFlag(annotationManager.GetCurrentPropertyState()));
        }
        else if (!multipleProperties)
        {
            toggle.Toggle(signal.Property.HasFlag(AssignedProperties));
        }
    }

	/// <summary>
	/// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
	/// </summary>
	protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        ToggleHolders();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Toggles the holders.
	/// </summary>
	void ToggleHolders()
    {

        if (multipleProperties)
        {
            annotationManager.SetCurrentPropertyActiveState(annotationManager.GetCurrentPropertyState(), !toggle.toggled);
            return;
        }

        annotationManager.SetCurrentPropertyActiveState(AssignedProperties, !toggle.toggled);
    }
}
