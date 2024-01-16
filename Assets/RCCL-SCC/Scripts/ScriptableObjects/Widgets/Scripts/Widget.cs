// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-18-2019
// ***********************************************************************
// <copyright file="Widget.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;
using Zenject;

/// <summary>
/// Enum WidgetState
/// </summary>
public enum WidgetState
{
	Disabled,
	Idle,
	Moving,
	Docked
}

/// <summary>
/// Class Widget.
/// Implements the <see cref="RuntimeSet{Widget}" />
/// </summary>
/// <seealso cref="RuntimeSet{Widget}" />

#if SCC_2_5
[CreateAssetMenu(menuName = "Widgets/Widget", fileName = "New Widget")]
public class Widget : RuntimeSet<Widget>
{
	#region Injection Construction
	/// <summary>
	/// The signal bus
	/// </summary>
	SignalBus _signalBus;

	/// <summary>
	/// Constructs the specified signal.
	/// </summary>
	/// <param name="signal">The signal.</param>
	[Inject]
    public void Construct(SignalBus signal)
    {
        _signalBus = signal;
    }
	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// The on state changed
	/// </summary>
	static Action<WidgetState> OnStateChanged;
	/// <summary>
	/// The manager
	/// </summary>
	public WidgetManager Manager;

	/// <summary>
	/// The state
	/// </summary>
	[ReadOnly]
    [SerializeField]
    WidgetState state;

	/// <summary>
	/// Gets or sets the state.
	/// </summary>
	/// <value>The state.</value>
	public WidgetState State
    {
        get
        {
            return state;
        }

        set
        {

            state = value;

            StateChange();
        }
    }

	/// <summary>
	/// The assigned tool
	/// </summary>
	public ActiveAnnotationTools AssignedTool;

	/// <summary>
	/// The runtime reference
	/// </summary>
	public WidgetRuntime RuntimeReference;

	/// <summary>
	/// The stored position
	/// </summary>
	[ReadOnly]
    public Vector2 StoredPosition;

	/// <summary>
	/// The callbacks
	/// </summary>
	Dictionary<GameObject, Action<WidgetState>> Callbacks = new Dictionary<GameObject, Action<WidgetState>>();

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Add a listener for widget state changes.
	/// </summary>
	/// <param name="listener">The registering gameobject.</param>
	/// <param name="callback">The function to call when the state changes.</param>
	public void AddListener(GameObject listener, Action<WidgetState> callback)
    {
        if (Callbacks == null)
            Callbacks = new Dictionary<GameObject, Action<WidgetState>>();

        if (!Callbacks.ContainsKey(listener))
        {
            Callbacks.Add(listener, callback);
            OnStateChanged += callback;
            //Debug.Log("Added " + listener.ToString() + " to " + name, this);
        }

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Removes the listener.
	/// </summary>
	/// <param name="listener">The listener.</param>
	public void RemoveListener(GameObject listener)
    {
        if (Callbacks.ContainsKey(listener))
        {
            //Debug.Log("Removed listener to " + name, this);
            OnStateChanged -= Callbacks[listener];
            Callbacks.Remove(listener);
        }


    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// States the change.
	/// </summary>
	void StateChange()
    {
        if (Callbacks.Count <= 0)
            return;

        foreach (KeyValuePair<GameObject, Action<WidgetState>> callback in Callbacks)
        {
            callback.Value?.Invoke(state);
        }

        _signalBus.Fire<Signal_Widget_StateChanged>(new Signal_Widget_StateChanged(State, this));
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback before Unity serializes your object.
	/// </summary>
	public override void OnBeforeSerialize()
    {

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback after Unity deserializes your object.
	/// </summary>
	public override void OnAfterDeserialize()
    {
        State = WidgetState.Disabled;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Updates the position.
	/// </summary>
	/// <param name="value">The value.</param>
	public void UpdatePosition(Vector2 value)
    {
        RuntimeReference.RuntimeWidgetRect.anchoredPosition = value;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Toggles the visibility.
	/// </summary>
	public void ToggleVisibility()
    {
        switch (State)
        {
            case WidgetState.Disabled:
				RuntimeReference.RuntimeWidgetRect.SetAsLastSibling();
                State = WidgetState.Idle;
                break;

            case WidgetState.Idle:
				RuntimeReference.RuntimeWidgetRect.SetAsFirstSibling();
                State = WidgetState.Disabled;
                break;
            case WidgetState.Docked:

                State = WidgetState.Disabled;
                Manager.AddOrRemoveDockedWidget(this);

                break;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            RuntimeReference.ToggleVisibility(State);
        }

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Toggles the docking.
	/// </summary>
	public void ToggleDocking()
    {
        Manager.AddOrRemoveDockedWidget(this);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Toggles the sliding.
	/// </summary>
	public void ToggleSliding()
    {
        RuntimeReference.ToggleSliding();

        State = (State == WidgetState.Disabled) ? WidgetState.Idle : WidgetState.Disabled;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif