// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-13-2019
// ***********************************************************************
// <copyright file="UserProfileController.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if SCC_2_5
/// <summary>
/// Controls the UserProfile menu.
/// Implements the <see cref="_MenuController" />
/// </summary>
/// <seealso cref="_MenuController" />
public class UserProfileController : _MenuController
{
    public CanvasGroup messageCG;
    public TextMeshProUGUI MessageTextObject;
    public Button AcceptButton;

    void Start()
    {
        AcceptButton.onClick.AddListener(() => FadeChildCanvas(messageCG, false));
    }

    void OnDestroy()
    {
        AcceptButton.onClick.RemoveListener(() => FadeChildCanvas(messageCG, false)); 
    }

    // Called from the main menu when a joined room fails.
    public void PromptJoinFailed(bool isPrompted)
    {
         FadeChildCanvas(messageCG, isPrompted);
    }
}
#endif