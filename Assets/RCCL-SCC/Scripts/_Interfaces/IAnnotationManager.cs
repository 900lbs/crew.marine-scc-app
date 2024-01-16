// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-06-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-18-2019
// ***********************************************************************
// <copyright file="IAnnotationManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if SCC_2_5
/// <summary>
/// Interface used by annotation manager class(es), at this point I don't see
/// us needing to create different versions, unless we're unit testing or different users
/// need different logic.
/// </summary>
public interface IAnnotationManager
{
	/// <summary>
	/// Gets or sets a value indicating whether this instance is ga edit enabled.
	/// </summary>
	/// <value><c>true</c> if this instance is ga edit enabled; otherwise, <c>false</c>.</value>
	bool IsGAEditEnabled { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="IAnnotationManager"/> is rotated.
	/// </summary>
	/// <value><c>true</c> if rotated; otherwise, <c>false</c>.</value>
	bool Rotated { get; set; }
	/// <summary>
	/// Gets or sets the spawned new icon.
	/// </summary>
	/// <value>The spawned new icon.</value>
	IconBehavior SpawnedNewIcon { get; set; }
	/// <summary>
	/// Gets or sets the build save ga edits.
	/// </summary>
	/// <value>The build save ga edits.</value>
	BuildSaveGAEdits buildSaveGAEdits { get; set; }

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the current annotation tools.
	/// </summary>
	/// <returns>ActiveAnnotationTools.</returns>
	ActiveAnnotationTools GetCurrentAnnotationTools();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the current annotation tools.
	/// </summary>
	/// <param name="tool">The tool.</param>
	/// <param name="active">if set to <c>true</c> [active].</param>
	void SetCurrentAnnotationTools(ActiveAnnotationTools tool, bool active);

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the state of the current annotation.
	/// </summary>
	/// <returns>AnnotationState.</returns>
	AnnotationState GetCurrentAnnotationState();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the state of the current annotation.
	/// </summary>
	/// <param name="state">The state.</param>
	/// <param name="extraData">The extra data.</param>
	void SetCurrentAnnotationState(AnnotationState state, object extraData);

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the state of the current property.
	/// </summary>
	/// <returns>ShipRoomProperties.</returns>
	ShipRoomProperties GetCurrentPropertyState();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the state of the current property.
	/// </summary>
	/// <param name="prop">The property.</param>
	void SetCurrentPropertyState(ShipRoomProperties prop);

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the state of the current property active.
	/// </summary>
	/// <returns>ShipRoomProperties.</returns>
	ShipRoomProperties GetCurrentPropertyActiveState();

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the state of the current property active.
	/// </summary>
	/// <param name="prop">The property.</param>
	/// <param name="active">if set to <c>true</c> [active].</param>
	void SetCurrentPropertyActiveState(ShipRoomProperties prop, bool active);

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the color of the current line.
	/// </summary>
	/// <returns>Color.</returns>
	Color GetCurrentLineColor();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the color of the current line.
	/// </summary>
	/// <param name="newColor">The new color.</param>
	void SetCurrentLineColor(Color newColor);

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the current line renderer object.
	/// </summary>
	/// <returns>GameObject.</returns>
	GameObject GetCurrentLineRendererObject();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the current line renderer object.
	/// </summary>
	/// <param name="lineRenderer">The line renderer.</param>
	void SetCurrentLineRendererObject(GameObject lineRenderer);

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the current icon object.
	/// </summary>
	/// <returns>SafetyIconData.</returns>
	SafetyIconData GetCurrentIconObject();

	/*------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the current icon object.
	/// </summary>
	/// <param name="selectIconComponent">The select icon component.</param>
	void SetCurrentIconObject(SelectIcon selectIconComponent);

    /*------------------------------------------------------------------------------------------------*/
}
#endif