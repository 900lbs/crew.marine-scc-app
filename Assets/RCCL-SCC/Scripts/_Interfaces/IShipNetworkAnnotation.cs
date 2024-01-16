// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-28-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 03-28-2019
// ***********************************************************************
// <copyright file="IShipNetworkAnnotation.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
/// <summary>
/// Interface IShipNetworkAnnotation
/// </summary>
public interface IShipNetworkAnnotation
{
	/// <summary>
	/// Our annotation listener
	/// </summary>
	/// <param name="value">The value.</param>
	void OnNewAnnotationReceived(string value);
}
