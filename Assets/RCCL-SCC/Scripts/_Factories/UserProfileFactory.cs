// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-12-2019
// ***********************************************************************
// <copyright file="UserProfileFactory.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Zenject;
/// <summary>
/// Class UserProfileFactory.
/// Implements the <see cref="Zenject.IFactory{AuthorityID, ShipID, UserNameID, UserProfile}" />
/// </summary>
/// <seealso cref="Zenject.IFactory{AuthorityID, ShipID, UserNameID, UserProfile}" />
public class UserProfileFactory : IFactory<AuthorityID, ShipID, UserNameID, UserProfile>
{
    #region Injection Construction
    /// <summary>
    /// The profile factory
    /// </summary>
    UserProfile.Factory profileFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileFactory"/> class.
    /// </summary>
    /// <param name="profileFact">The profile fact.</param>
    [Inject]
    public UserProfileFactory(UserProfile.Factory profileFact)
    {
        profileFactory = profileFact;
    }

    #endregion

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Creates the specified authority.
    /// </summary>
    /// <param name="authority">The authority.</param>
    /// <param name="ship">The ship.</param>
    /// <param name="userNameID">The user name identifier.</param>
    /// <returns>UserProfile.</returns>
    public UserProfile Create(AuthorityID authority, ShipID ship, UserNameID userNameID)
    {
        UserProfile newProfile = profileFactory.Create(authority, ship, userNameID);

        newProfile.AbbreviatedName = GetAbbreviatedName(userNameID);
        newProfile.Ship = ship;
        newProfile.UserNameID = userNameID;

        return newProfile;
    }

    /// <summary>
    /// Gets the name of the abbreviated.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>System.String.</returns>
    public static string GetAbbreviatedName(UserNameID userName)
    {
        switch (userName)
        {
            case UserNameID.SafetyCommandCenter:
                return "SCC";
            case UserNameID.EngineControlRoom:
                return "ECR";
            case UserNameID.StagingArea:
                return "SA";
            case UserNameID.ForwardControlPoint:
                return "FCP";
            case UserNameID.EvacuationControlCenter:
                return "ECC";
            case UserNameID.IncidentCommandCenter:
                return "ICC";
            case UserNameID.ShoresideOperations:
                return "SO";
            default:
                return "";
        }
    }

    /// <summary>
    /// Gets the full name.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>System.String.</returns>
    public static string GetFullName(UserNameID userName)
    {
        var sb = new StringBuilder();

        char previousChar = char.MinValue; // Unicode '\0'

        foreach (char c in userName.ToString())
        {
            if (char.IsUpper(c))
            {
                // If not the first character and previous character is not a space, insert a space before uppercase

                if (sb.Length != 0 && previousChar != ' ')
                {
                    sb.Append(' ');
                }
            }

            sb.Append(c);

            previousChar = c;
        }

        return sb.ToString();
    }
}
