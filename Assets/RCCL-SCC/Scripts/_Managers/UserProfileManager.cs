// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-17-2019
// ***********************************************************************
// <copyright file="UserProfileManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

using DG.Tweening;
using Zenject;

/// <summary>
/// Class UserProfileManager.
/// Implements the <see cref="Zenject.IInitializable" />
/// </summary>
/// <seealso cref="Zenject.IInitializable" />
public class UserProfileManager : IInitializable
{
    #region Injection Construction
    /// <summary>
    /// The network client
    /// </summary>
    INetworkClient networkClient;
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileManager"/> class.
    /// </summary>
    /// <param name="netClient">The net client.</param>
    /// <param name="signal">The signal.</param>
    [Inject]
    public UserProfileManager(INetworkClient netClient, SignalBus signal)
    {
        networkClient = netClient;
        _signalBus = signal;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The current user
    /// </summary>
    public UserProfile CurrentUser;

    /// <summary>
    /// The user profiles
    /// </summary>
    List<UserProfile> UserProfiles;
    /// <summary>
    /// The active users
    /// </summary>
    UserNameID ActiveUsers;


    Tween TimerCountdownTween;

    const float TimerCountdown = 5;


    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public async void Initialize()
    {
        UserProfiles = new List<UserProfile>();

        foreach (UserNameID id in Enum.GetValues(typeof(UserNameID)))
        {
            UserProfile newPlayer = new UserProfile();
            newPlayer.UserNameID = id;

            UserProfiles.Add(newPlayer);
        }

/*         TimerCountdownTween = DOVirtual.Float(0, TimerCountdown, TimerCountdown, OnTimerCountdownUpdate)
        .OnStart(async () => { await CheckActiveUsers(); })
        .SetLoops(-1)
        .OnStepComplete(async () => { await CheckActiveUsers(); }); */
    }

    async Task CheckActiveUsers()
    {
        await networkClient.FireAllActivePlayers();

        await new WaitForSeconds(5);

    }

    public void OnTimerCountdownUpdate(float val)
    { }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets all user profiles.
    /// </summary>
    /// <returns>List&lt;UserProfile&gt;.</returns>
    public List<UserProfile> GetAllUserProfiles()
    {
        return UserProfiles;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the active users.
    /// </summary>
    /// <returns>UserNameID.</returns>
    public UserNameID GetActiveUsers()
    {
        return ActiveUsers;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/


    /// <summary>
    /// Gets the user profile.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>UserProfile.</returns>
    public UserProfile GetUserProfile(UserNameID userName)
    {
        return UserProfiles.Find(x => x.UserNameID == userName);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Activates the user.
    /// </summary>
    /// <param name="user">The user.</param>
    public void ActivateUser(UserNameID user)
    {
        if (!ActiveUsers.HasFlag(user))
        {
            ActiveUsers = ActiveUsers | user;
            _signalBus.Fire<Signal_UserProfile_UserActivityUpdated>(new Signal_UserProfile_UserActivityUpdated(GetUserProfile(user), true));
            Debug.Log("Active Users: " + ActiveUsers.ToString());
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Deactivates the user.
    /// </summary>
    /// <param name="user">The user.</param>
    public void DeactivateUser(UserNameID user)
    {
        if (ActiveUsers.HasFlag(user))
        {
            ActiveUsers = ActiveUsers ^ user;
            _signalBus.Fire<Signal_UserProfile_UserActivityUpdated>(new Signal_UserProfile_UserActivityUpdated(GetUserProfile(user), false));
            Debug.Log("Active Users: " + ActiveUsers.ToString());
        }
    }


    /*----------------------------------------------------------------------------------------------------------------------------*/

}
