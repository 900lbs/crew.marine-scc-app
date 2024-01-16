// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="UIHelpers.cs" company="900lbs of Creative">
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
using System.Threading.Tasks;

/// <summary>
/// Class UIHelpers.
/// </summary>
public static class UIHelpers
{

	/*------------------------------------------------------------------------------------------------*/
	/// <summary>
	/// Toggles the color.
	/// </summary>
	/// <param name="component">The component.</param>
	/// <param name="color">The color.</param>
	public async static Task ToggleColor(GameObject component, Color color)
    {

        if (component.GetComponent<Image>())
        {
            Image image = component.GetComponent<Image>();

            image.DOColor(color, 0.25f);
            return;
        }



        if (component.GetComponent<TextMeshProUGUI>())
        {
            TextMeshProUGUI tmp = component.GetComponent<TextMeshProUGUI>();
            
            tmp.DOColor(color, 0.25f);
            return;
        }

        Debug.LogError(component.name + " was not a designated ToggleColor type!", component);
        await new WaitForEndOfFrame();
    }

    /*------------------------------------------------------------------------------------------------*/
}
