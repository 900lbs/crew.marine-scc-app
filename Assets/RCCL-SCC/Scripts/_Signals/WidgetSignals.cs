// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-20-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="WidgetSignals.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Zenject;

/// <summary>
/// Let's subscribers know that this widget's state has changed.
/// </summary>
public class Signal_Widget_StateChanged
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Signal_Widget_StateChanged"/> class.
	/// </summary>
	/// <param name="state">The state.</param>
	/// <param name="customData">The custom data.</param>
	public Signal_Widget_StateChanged(WidgetState state, object customData)
    {
        State = state;
        CustomData = customData;
    }

	/// <summary>
	/// Gets the state.
	/// </summary>
	/// <value>The state.</value>
	public WidgetState State { get; private set; }
	/// <summary>
	/// Gets the custom data.
	/// </summary>
	/// <value>The custom data.</value>
	public object CustomData { get; private set; }
}

/*----------------------------------------------------------------------------------------------------------------------------*/


