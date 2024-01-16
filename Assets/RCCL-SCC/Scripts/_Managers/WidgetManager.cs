// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="WidgetManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class WidgetManager.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// Implements the <see cref="UnityEngine.ISerializationCallbackReceiver" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />
/// <seealso cref="UnityEngine.ISerializationCallbackReceiver" />

#if SCC_2_5
[CreateAssetMenu(menuName = "Widgets/Manager", fileName = "New Widget Manager")]

public class WidgetManager : ScriptableObject,
ISerializationCallbackReceiver
{
	/// <summary>
	/// The on update docked widgets
	/// </summary>
	public static Action<Widget> OnUpdateDockedWidgets;
	/// <summary>
	/// The save data
	/// </summary>
	public bool SaveData = false;
	/// <summary>
	/// The docked widgets
	/// </summary>
	public List<Widget> DockedWidgets;

	/// <summary>
	/// The dock position
	/// </summary>
	public Vector2 DockPos;

	/// <summary>
	/// The spacing x
	/// </summary>
	float SpacingX = 414;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called after deserialization (i.e after we stop play), set SaveData to true if you wish data to be saved on quit.
	/// </summary>
	public void OnAfterDeserialize()
    {
        if (!SaveData)
        {
            DockedWidgets = null;
            DockPos = new Vector2(0, 0);
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback before Unity serializes your object.
	/// </summary>
	public void OnBeforeSerialize() { }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [enable].
	/// </summary>
	void OnEnable()
    {
        OnUpdateDockedWidgets += UpdateDockedWidgets;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [disable].
	/// </summary>
	void OnDisable()
    {
        OnUpdateDockedWidgets -= UpdateDockedWidgets;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Add a docked widget if it's not listed already, otherwise assume that it was already docked and has been undocked.
	/// </summary>
	/// <param name="value">The widget in question.</param>
	public void AddOrRemoveDockedWidget(Widget value)
    {
        if (DockedWidgets == null)
            DockedWidgets = new List<Widget>();


        if (!DockedWidgets.Contains(value))
        {
            Vector2 newStoredPosition = value.RuntimeReference.GetComponent<RectTransform>().anchoredPosition;
            value.StoredPosition = newStoredPosition;

            DockedWidgets.Add(value);
            value.State = WidgetState.Docked;
        }

        else
        {
            value.UpdatePosition(value.StoredPosition);
            value.State = (value.State == WidgetState.Disabled) ? WidgetState.Disabled : WidgetState.Idle;
            DockedWidgets.Remove(value);
        }

        UpdateDockedWidgets(value);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Let all listening Widgets know that there has been an update.
	/// </summary>
	/// <param name="value">The value.</param>
	public void UpdateDockedWidgets(Widget value)
    {
        for (int i = 0; i < DockedWidgets.Count; i++)
        {
            int index = DockedWidgets.IndexOf(DockedWidgets[i]);
            DockedWidgets[i].UpdatePosition(GetDockPosition(index));
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Returns position based on index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>Vector2.</returns>
	public Vector2 GetDockPosition(int index)
    {
        SpacingX = 0;

        for (int i = 0; i < index; i++)
        {
            SpacingX += DockedWidgets[i].RuntimeReference.xReference;
        }

        Vector2 newDockPos = DockPos;
        newDockPos.x = SpacingX / 2;
        return newDockPos;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif