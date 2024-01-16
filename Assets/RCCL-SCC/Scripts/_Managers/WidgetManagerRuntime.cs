// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="WidgetManagerRuntime.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SCC_2_5
/// <summary>
/// The runtime helper for WidgetManager.cs
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
[RequireComponent(typeof(RectTransform))]
public class WidgetManagerRuntime : MonoBehaviour
{
	/// <summary>
	/// The manager
	/// </summary>
	public WidgetManager Manager;

	/// <summary>
	/// The docking corner
	/// </summary>
	public CornersID DockingCorner;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [enable].
	/// </summary>
	void OnEnable()
    {
        Manager.DockPos = GetRectDockPos();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [disable].
	/// </summary>
	void OnDisable()
    {

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Calculates and assigns the WidgetManager's docking position.
	/// </summary>
	/// <returns>Vector2.</returns>
	Vector2 GetRectDockPos()
    {
        Vector2 dockPos = new Vector2(0, 0);
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetLocalCorners(corners);

        dockPos.x = corners[(int)DockingCorner].x;
        dockPos.x = corners[(int)DockingCorner].y;

        return dockPos;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif