// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="RuntimeSet.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PrimitiveFactory.ScriptableObjectSuite;

/// <summary>
/// Class RuntimeSet.
/// Implements the <see cref="PrimitiveFactory.ScriptableObjectSuite.ScriptableObjectExtended" />
/// Implements the <see cref="UnityEngine.ISerializationCallbackReceiver" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="PrimitiveFactory.ScriptableObjectSuite.ScriptableObjectExtended" />
/// <seealso cref="UnityEngine.ISerializationCallbackReceiver" />
public abstract class RuntimeSet<T> : ScriptableObjectExtended,
ISerializationCallbackReceiver
{
	//Probably useless, this is just in case there are more than one of this
	//particular class type.
	/// <summary>
	/// The items
	/// </summary>
	public List<T> Items = new List<T>();

	/// <summary>
	/// Adds the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	public virtual void Add(T value)
    {
        if (!Items.Contains(value))
            Items.Add(value);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Removes the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	public virtual void Remove(T value)
    {
        if (Items.Contains(value))
            Items.Remove(value);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Equalses the specified value x.
	/// </summary>
	/// <param name="valueX">The value x.</param>
	/// <param name="valueY">The value y.</param>
	/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
	public virtual bool Equals(T valueX, T valueY)
    {
        if (valueX.GetType() == valueY.GetType())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback after Unity deserializes your object.
	/// </summary>
	public abstract void OnAfterDeserialize();

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback before Unity serializes your object.
	/// </summary>
	public abstract void OnBeforeSerialize();

}