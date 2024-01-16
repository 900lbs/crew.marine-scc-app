// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="StructData.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// Struct DeckArgs
/// </summary>

public struct DeckArgs
{
	/// <summary>
	/// The deck identifier
	/// </summary>
	public string DeckID;

	/// <summary>
	/// The state
	/// </summary>
	public ActiveState State;
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Struct AnnotationNetworkData
/// </summary>
public struct AnnotationNetworkData
{
	/// <summary>
	/// The state
	/// </summary>
	public string State;

	//* Insert other relevant data */

	/// <summary>
	/// Initializes a new instance of the <see cref="AnnotationNetworkData"/> struct.
	/// </summary>
	/// <param name="state">The state.</param>
	public AnnotationNetworkData(AnnotationState state)
    {
        State = state.ToString();
    }
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Simple struct for passing along Timer information.
/// </summary>
public struct Timer
{
	/// <summary>
	/// The name
	/// </summary>
	public string Name;

	/// <summary>
	/// The has started
	/// </summary>
	public bool HasStarted;
	/// <summary>
	/// The has paused
	/// </summary>
	public bool HasPaused;
	/// <summary>
	/// The has reset
	/// </summary>
	public bool HasReset;
	/// <summary>
	/// The start time
	/// </summary>
	public string StartTime;
}

/*------------------------------------------------------------------------------------------------*/

