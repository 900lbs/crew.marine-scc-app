// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-09-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="IGAEditor.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface IGAEditor
/// </summary>
public interface IGAEditor
{
	/// <summary>
	/// Saves the prefabs.
	/// </summary>
	void SavePrefabs();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Loads the prefabs.
	/// </summary>
	void LoadPrefabs();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Loads the prefabs.
	/// </summary>
	/// <param name="value">The value.</param>
	void LoadPrefabs(string value);
}
