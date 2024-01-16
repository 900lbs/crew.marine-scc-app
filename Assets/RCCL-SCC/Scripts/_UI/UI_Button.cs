// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="UI_Button.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;
using Zenject;
/// <summary>
/// Generic button class that stores information needed by all of our, wait for it, buttons.
/// Implements <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />

#if SCC_2_5
[RequireComponent(typeof(Button))]
public abstract class UI_Button : MonoBehaviour
{

    #region Injection Construction
    /// <summary>
    /// The network client
    /// </summary>
    protected NetworkClient networkClient;

    protected Settings sceneSettings;

    /// <summary>
    /// The signal bus
    /// </summary>
    protected SignalBus _signalBus;

    /// <summary>
    /// Constructs the specified anno man.
    /// </summary>
    /// <param name="annoMan">The anno man.</param>
    /// <param name="signal">The signal.</param>
    /// <param name="netClient">The net client.</param>
    [Inject]
    public void Construct(SignalBus signal, Settings scene, NetworkClient netClient)
    {
        _signalBus = signal;
        sceneSettings = scene;
        networkClient = netClient;
    }

    #endregion
    [SerializeField]
    [ReadOnly]
    ActiveState state;
    /// <summary>
    /// Gets or sets the ActiveState.
    /// </summary>
    /// <value>The state.</value>
    public ActiveState State
    {
        get
        {
            return state;
        }
        set
        {
            if (state != value)
            {
                state = value;
                OnStateChange();
            }
        }
    }

    /// <summary>
    /// The disable timer
    /// </summary>
    [Tooltip("Set to 0 for no disable after click.")]
    public float DisableTimer = 0.25f;

    /// <summary>
    /// The button
    /// </summary>
    [HideInInspector]
    public Button button;

    /// <summary>
    /// Gets the disable tween.
    /// </summary>
    /// <value>The disable tween.</value>
    public Tween disableTween { get; private set; }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [enable].
    /// </summary>
    protected virtual void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(BTN_OnClick);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [disable].
    /// </summary>
    protected virtual void OnDestroy()
    {
        button?.onClick.RemoveListener(BTN_OnClick);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected virtual void BTN_OnClick()
    {
        if (DisableTimer != 0)
        {
            button.interactable = false;
            EnableButton(DisableTimer);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected abstract void OnStateChange();

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Enables the button.
    /// </summary>
    /// <param name="timer">The timer.</param>
    protected void EnableButton(float timer)
    {
        disableTween = DOVirtual.Float(0, timer, timer, OnDisableButtonUpdate)
        .OnComplete(() => button.interactable = true);

        disableTween.Play();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Called when [disable button update].
    /// </summary>
    /// <param name="val">The value.</param>
    void OnDisableButtonUpdate(float val) { }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public class Factory : PlaceholderFactory<Object, UI_Button> { }
}
#endif