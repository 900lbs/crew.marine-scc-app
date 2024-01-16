// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="ConnectionController.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using TMPro;

#if SCC_2_5
/// <summary>
/// Controls the connection dialog window.
/// Implements the <see cref="_MenuController" />
/// </summary>
/// <seealso cref="_MenuController" />
[RequireComponent(typeof(Canvas))]
public class ConnectionController : _MenuController
{
	/// <summary>
	/// The connection text
	/// </summary>
	[NaughtyAttributes.Required] public TextMeshProUGUI ConnectionText;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Awakes this instance.
	/// </summary>
	public override void Awake()
    {
        base.Awake();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Updates the connection text.
	/// </summary>
	/// <param name="text">The text.</param>
	public void UpdateConnectionText(string text)
    {
        ConnectionText.text = text;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif