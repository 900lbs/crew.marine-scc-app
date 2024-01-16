// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-21-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="ShipRoomPropertyHolder.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ShipRoomPropertyHolder.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ShipRoomPropertyHolder : MonoBehaviour
{
	/// <summary>
	/// The assigned property
	/// </summary>
	public ShipRoomProperties AssignedProperty;

	/// <summary>
	/// The maximum capacity
	/// </summary>
	public int MaximumCapacity;
	/// <summary>
	/// The holders
	/// </summary>
	public List<RectTransform> Holders;

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Starts this instance.
	/// </summary>
	void Start()
    {
        if (Holders == null)
        {
            Holders = new List<RectTransform>();
            Holders.AddRange(transform.GetComponentsInChildren<RectTransform>());
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Adds the new object.
	/// </summary>
	/// <param name="newObject">The new object.</param>
	public void AddNewObject(GameObject newObject)
    {
        int holderCount = Holders.Count;
        for (int i = 0; i < holderCount; ++i)
        {
            if (Holders[i].childCount <= MaximumCapacity)
            {
                newObject.transform.parent = Holders[i];
                return;
            }
        }

        GameObject createNewHolder = Instantiate(Holders[0].gameObject, transform);
        
        int holderChildCount =  createNewHolder.transform.childCount;
        for (int i = 0; i < holderChildCount; i++)
        {
            Destroy(createNewHolder.transform.GetChild(i));
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
