// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-18-2019
// ***********************************************************************
// <copyright file="ProjectVersionGetterSetter.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/// <summary>
/// Very simple class that updates a TMP object with the current version.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
[RequireComponent(typeof(TextMeshProUGUI))]
public class ProjectVersionGetterSetter : MonoBehaviour
{
	/// <summary>
	/// The text mesh
	/// </summary>
	TextMeshProUGUI textMesh;

	/// <summary>
	/// Called when [enable].
	/// </summary>
	void OnEnable()
    {
        if(!textMesh)
            textMesh = GetComponent<TextMeshProUGUI>();

        textMesh.text = "Version: " + Application.version;
    }
}
