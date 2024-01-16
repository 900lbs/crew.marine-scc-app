// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-29-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="UserSignals.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I think this may be the first rendition of the signal used in INetworkClientSignals.
/// </summary>
public class Signal_UserProfile_UserActivityUpdated
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Signal_UserProfile_UserActivityUpdated"/> class.
	/// </summary>
	/// <param name="profile">The profile.</param>
	/// <param name="isOnline">if set to <c>true</c> [is online].</param>
	public Signal_UserProfile_UserActivityUpdated(UserProfile profile, bool isOnline)
    {
        Profile = profile;
        IsOnline = isOnline;
    }

	/// <summary>
	/// Gets the profile.
	/// </summary>
	/// <value>The profile.</value>
	public UserProfile Profile { get; }

	/// <summary>
	/// Gets a value indicating whether this instance is online.
	/// </summary>
	/// <value><c>true</c> if this instance is online; otherwise, <c>false</c>.</value>
	public bool IsOnline { get; }
}

/*----------------------------------------------------------------------------------------------------------------------------*/
