// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-10-2019
// ***********************************************************************
// <copyright file="ShipUIVariable.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ShipUIVariable.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />

[System.Serializable]
[CreateAssetMenu(menuName = "Ship UI Settings/New UI Settings", fileName = "New Ship UI Settings")]
public class ShipUIVariable : ScriptableObject
{
	/// <summary>
	/// The settings
	/// </summary>
	public ShipUIData Settings = new ShipUIData(Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, null, null);
}