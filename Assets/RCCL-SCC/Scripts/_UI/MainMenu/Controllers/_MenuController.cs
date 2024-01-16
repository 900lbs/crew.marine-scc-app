// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="_MenuController.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Zenject;

#if SCC_2_5

[RequireComponent(typeof(CanvasGroup))]
/// <summary>
/// Base controller class, controls the built-in canvas operations such as FadeCanvas.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <remarks>This class and derivatives give us simple control over each facet of the mainmenu.</remarks>
public class _MenuController : MonoBehaviour
{
    #region Injection Construction
    /// <summary>
    /// The main menu
    /// </summary>
    protected MainMenu mainMenu;
    protected NetworkButton.Factory networkButtonFactory;

    /// <summary>
    /// Constructs the specified menu.
    /// </summary>
    /// <param name="menu">The menu.</param>
    [Inject]
    public void Construct(MainMenu menu, NetworkButton.Factory networkButtonFact)
    {
        mainMenu = menu;
        networkButtonFactory = networkButtonFact;
    }

    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the cg.
    /// </summary>
    /// <value>The canvas group.</value>
    public CanvasGroup cg { get; private set; }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public GameObject ButtonPrefab;

    public RectTransform ButtonsParent;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public virtual void Awake()
    {
        if (!cg)
            cg = GetComponent<CanvasGroup>();

    }

    public void SpawnButtons(NetworkAction[] networkActions)
    {
        int buttonParentChildCount = ButtonsParent.childCount;
        for (int i = 0; i < buttonParentChildCount; ++i)
        {
            Destroy(ButtonsParent.GetChild(i).gameObject);
        }

        int networkActionsCount = networkActions.Length;
        for (int i = 0; i < networkActionsCount; ++i)
        {
            NetworkButton newButton = networkButtonFactory.Create(ButtonPrefab, networkActions[i]);
            newButton.transform.SetParent(ButtonsParent);
            newButton.transform.localScale = Vector3.one;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Fades the canvas.
    /// </summary>
    /// <param name="active">if set to <c>true</c> [active].</param>
    public virtual void FadeCanvas(bool active)
    {
        TweenCanvas(active);
    }

    public virtual void FadeChildCanvas(CanvasGroup childCG, bool active)
    {
        TweenCanvas(childCG, active);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Tweens the canvas.
    /// </summary>
    /// <param name="active">if set to <c>true</c> [active].</param>
    void TweenCanvas(bool active)
    {
        //Debug.Log("Tweening: " + name + " = " + active, this);

        cg.DOKill(false);

        if (active)
        {
            if (!mainMenu.MenuCamera.enabled)
                mainMenu.MenuCamera.enabled = true;
            cg.DOFade(1, 0.25f)
            .OnComplete(() => { cg.interactable = true; cg.blocksRaycasts = true; });
        }
        else
        {
            if (mainMenu.MenuCamera.enabled)
                mainMenu.MenuCamera.enabled = false;
            cg.DOFade(0, 0.25f)
            .OnStart(() => { cg.interactable = false; cg.blocksRaycasts = false; });
        }
    }

    void TweenCanvas(CanvasGroup childCG, bool active)
    {
        //Debug.Log("Tweening: " + name + " = " + active, this);

        childCG.DOKill(false);

        if (active)
        {
            if (!mainMenu.MenuCamera.enabled)
                mainMenu.MenuCamera.enabled = true;
            childCG.DOFade(1, 0.25f)
            .OnComplete(() => { childCG.interactable = true; childCG.blocksRaycasts = true; });
        }
        else
        {
            if (mainMenu.MenuCamera.enabled)
                mainMenu.MenuCamera.enabled = false;
            childCG.DOFade(0, 0.25f)
            .OnStart(() => { childCG.interactable = false; childCG.blocksRaycasts = false; });
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif