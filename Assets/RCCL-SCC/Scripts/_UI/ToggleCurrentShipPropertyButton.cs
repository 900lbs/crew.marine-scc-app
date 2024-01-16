// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-09-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="ToggleCurrentShipPropertyButton.cs" company="900lbs of Creative">
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
/// Simple button that toggles our current Ship Property.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class ToggleCurrentShipPropertyButton : UI_Button
{
	#region Injection Construction
	/// <summary>
	/// The annotation manager
	/// </summary>
	[Inject]
    AnnotationManager annotationManager;
	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// The assigned property
	/// </summary>
	[EnumFlag] public ShipRoomProperties AssignedProperty;


	/*----------------------------------------------------------------------------------------------------------------------------*/


	/// <summary>
	/// Handles everything when the state is changed, from colors to images etc.
	/// </summary>
	protected override void OnStateChange()
    {

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
	/// </summary>
	protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        annotationManager.SetCurrentPropertyState(AssignedProperty);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif