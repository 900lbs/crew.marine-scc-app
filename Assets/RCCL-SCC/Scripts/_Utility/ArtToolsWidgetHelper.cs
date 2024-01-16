// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-21-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="ArtToolsWidgetHelper.cs" company="900lbs of Creative">
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
/// Class ArtToolsWidgetHelper.
/// Implements the <see cref="WidgetRuntimeHelper{Signal_AnnoMan_OnRoomPropertyChanged}" />
/// Implements the <see cref="Zenject.ILateDisposable" />
/// </summary>
/// <seealso cref="WidgetRuntimeHelper{Signal_AnnoMan_OnRoomPropertyChanged}" />
/// <seealso cref="Zenject.ILateDisposable" />
[RequireComponent(typeof(WidgetRuntime))]
[System.Obsolete]
public class ArtToolsWidgetHelper : WidgetRuntimeHelper<Signal_AnnoMan_OnRoomPropertyChanged>, ILateDisposable
{
	/// <summary>
	/// The widget state
	/// </summary>
	ActiveState widgetState;
	/// <summary>
	/// The widget
	/// </summary>
	Widget widget;

	/// <summary>
	/// Starts this instance.
	/// </summary>
	void Start()
    {
        //widget.AddListener(gameObject, OnWidgetStateChange);
    }

	/// <summary>
	/// Lates the dispose.
	/// </summary>
	public void LateDispose()
    {

    }

	/// <summary>
	/// Called when [widget state change].
	/// </summary>
	/// <param name="newState">The new state.</param>
	void OnWidgetStateChange(ActiveState newState)
    {
        if(newState == ActiveState.Disabled)
        {
            FireSignal(new Signal_AnnoMan_OnRoomPropertyChanged(ShipRoomProperties.None));
        }
    }
}
#endif