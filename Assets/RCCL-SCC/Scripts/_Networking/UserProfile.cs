// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-29-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="UserProfile.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[System.Serializable]
/// <summary>
/// All data pertaining to a user.
/// </summary>
/// <remarks>These profile are created via UserProfileFactory
/// I.E UserProfile user = new UserProfileFactory.Create(arg);</remarks>
public class UserProfile
{
    #region Injection Construction
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfile"/> class.
    /// </summary>
    /// <param name="signal">The signal.</param>
    [Inject]
    public UserProfile(SignalBus signal)
    {
        _signalBus = signal;
    }

    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfile"/> class.
    /// </summary>
    public UserProfile() { }

    /// <summary>
    /// The abbreviated name
    /// </summary>
    [ReadOnly] public string AbbreviatedName;

    /// <summary>
    /// The authority
    /// </summary>
    public AuthorityID Authority
    {
        get
        {
            if (UserNameID == UserNameID.SafetyCommandCenter)
                return AuthorityID.Master;

            if (UserNameID == UserNameID.IncidentCommandCenter)
                return AuthorityID.ShoreTable;

            else
                return AuthorityID.Tablet;
        }
    }

    public Color DefaultColor;

    /// <summary>
    /// The ship
    /// </summary>
    public ShipID Ship;

    /// <summary>
    /// The user name identifier
    /// </summary>
    public UserNameID UserNameID;

    public void DebugUserProfile()
    {
        /* Debug.Log("UserName: " + UserNameID.ToString()
        + " / Authority: " + Authority.ToString()); */
    }

    /// <summary>
    /// Class Factory.
    /// Implements the <see cref="Zenject.PlaceholderFactory{AuthorityID, ShipID, UserNameID, UserProfile}" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{AuthorityID, ShipID, UserNameID, UserProfile}" />
    public class Factory : PlaceholderFactory<AuthorityID, ShipID, UserNameID, UserProfile> { }
}
