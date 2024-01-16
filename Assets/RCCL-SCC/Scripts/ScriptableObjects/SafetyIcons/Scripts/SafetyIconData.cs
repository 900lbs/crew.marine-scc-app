// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-09-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="SafetyIconData.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

/// <summary>
/// Created By: Joshua Bowers - 06/14/19
/// Last Edited By: Joshua Bowers - 06/14/19
/// Purpose: Scriptable Object data class for Icons.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />
[CreateAssetMenu(menuName = "Icons/SafetyIcon", fileName = "New Safety Icon")]
[System.Serializable]
public class SafetyIconData : ScriptableObject
{
	/// <summary>
	/// The icon name
	/// </summary>
	[SerializeField] [ReadOnly] string iconName = "";
	/// <summary>
	/// Gets or sets the name of the icon.
	/// </summary>
	/// <value>The name of the icon.</value>
	public string IconName { get { return iconName; } set { iconName = value; } }

	/// <summary>
	/// The assigned ships
	/// </summary>
	[EnumFlag]
    public ShipID AssignedShips;

	/// <summary>
	/// The icon image
	/// </summary>
	[SerializeField] Sprite iconImage;
	/// <summary>
	/// Gets or sets the icon image.
	/// </summary>
	/// <value>The icon image.</value>
	public Sprite IconImage { get { return iconImage; } set { iconImage = value; } }

	/// <summary>
	/// The icon size
	/// </summary>
	[SerializeField] Vector2 iconSize;
	/// <summary>
	/// Gets or sets the size of the icon.
	/// </summary>
	/// <value>The size of the icon.</value>
	public Vector2 IconSize { get { return iconSize; } set { iconSize = value; } }

	/// <summary>
	/// The assigned icon type
	/// </summary>
	[SerializeField] IconType assignedIconType;

	/// <summary>
	/// Gets or sets the type of the assigned.
	/// </summary>
	/// <value>The type of the assigned.</value>
	public IconType AssignedType { get { return assignedIconType; } set { assignedIconType = value; } }

	/// <summary>
	/// The assigned properties
	/// </summary>
	[EnumFlag]
    public ShipRoomProperties AssignedProperties;

	/// <summary>
	/// The prefab reference
	/// </summary>
	[SerializeField]
    public GameObject PrefabReference;

	/// <summary>
	/// Initializes a new instance of the <see cref="SafetyIconData"/> class.
	/// </summary>
	/// <param name="newIconName">New name of the icon.</param>
	/// <param name="newIconImage">The new icon image.</param>
	/// <param name="newIconSize">New size of the icon.</param>
	/// <param name="newShipRoomProperties">The new ship room properties.</param>
	/// <param name="newIconType">New type of the icon.</param>
	/// <param name="newPrefabReference">The new prefab reference.</param>
	public SafetyIconData(string newIconName, Sprite newIconImage, Vector2 newIconSize,
    ShipRoomProperties newShipRoomProperties, IconType newIconType, GameObject newPrefabReference)
    {

        iconName = newIconName;
        iconImage = newIconImage;
        iconSize = newIconSize;
        AssignedProperties = newShipRoomProperties;
        assignedIconType = newIconType;
        PrefabReference = newPrefabReference;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Assigns the name.
	/// </summary>
	/// <param name="newName">The new name.</param>
	public void AssignName(string newName)
    {
        iconName = newName;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Called when [validate].
	/// </summary>
	void OnValidate()
    {
        if (string.IsNullOrEmpty(iconName) || iconName != name)
        {
            iconName = name;
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the size delta from prefab.
	/// </summary>
	/// <returns>Vector2.</returns>
	public Vector2 GetSizeDeltaFromPrefab()
    {
        return PrefabReference.GetComponent<RectTransform>().anchoredPosition;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Gets the scale from prefab.
	/// </summary>
	/// <returns>Vector3.</returns>
	public Vector3 GetScaleFromPrefab()
    {
        return PrefabReference.GetComponent<RectTransform>().localScale;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
