// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-28-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="EniramClasses.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

/// <summary>
/// Class TokenClassName.
/// </summary>
[Serializable]
public class TokenClassName
{
	/// <summary>
	/// The access token
	/// </summary>
	public string access_token;
}

/// <summary>
/// Class infoJson.
/// </summary>
[Serializable]
public class infoJson
{
	/// <summary>
	/// The wind speed
	/// </summary>
	public double windSpeed;
	/// <summary>
	/// The wind direction
	/// </summary>
	public double windDirection;
	/// <summary>
	/// The heading
	/// </summary>
	public double heading;
	/// <summary>
	/// The STW
	/// </summary>
	public double stw;
	/// <summary>
	/// The sog
	/// </summary>
	public double sog;
}

/// <summary>
/// Class RootObject.
/// </summary>
[Serializable]
public class RootObject
{
	/// <summary>
	/// The information
	/// </summary>
	public infoJson[] info;
}
