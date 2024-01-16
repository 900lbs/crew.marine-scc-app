// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-29-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-24-2019
// ***********************************************************************
// <copyright file="UserProfileButtonHandler.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;
#if SCC_2_5
/// <summary>
/// Class UserProfileButtonHandler.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class UserProfileButtonHandler : MonoBehaviour
{
    #region Injection Construction
    /// <summary>
    /// The user profile manager
    /// </summary>
    protected UserProfileManager userProfileManager;

    NetworkClient networkClient;

    [EnumFlag]
    [Inject(Id = "AvailableUserLogin")]
    public UserNameID availableUserNameIDs;

    /// <summary>
    /// Constructs the specified user profile man.
    /// </summary>
    /// <param name="userProfileMan">The user profile man.</param>
    [Inject]
    public void Construct(UserProfileManager userProfileMan, NetworkClient netClient)
    {
        userProfileManager = userProfileMan;
        networkClient = netClient;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// The user profile button factory
    /// </summary>
    [Inject(Optional = true, Id = UserAction.Login)]
    IUserProfileButtonFactory userProfileLoginButtonFactory;

    /// <summary>
    /// The user profile button factory
    /// </summary>
    [Inject(Optional = true, Id = UserAction.ToggleAnnotations)]
    IUserProfileButtonFactory userProfileActiveUserAnnotationsFactory;
    /// <summary>
    /// The user profile button factory
    /// </summary>
    [Inject(Optional = true, Id = UserAction.Select)]
    IUserProfileButtonFactory userProfileSelectUserFactory;

    /// <summary>
    /// The is abbreviated
    /// </summary>
    public bool IsAbbreviated;

    public UserAction UserProfileButtonType;

    /// <summary>
    /// The user profile button prefab
    /// </summary>
    public GameObject UserProfileButtonPrefab;

    /// <summary>
    /// The maximum spawn per parent
    /// </summary>
    public int MaximumSpawnPerParent;

    /// <summary>
    /// The spawn parent
    /// </summary>
    [Tooltip("Leave empty if you want the buttons to spawn under this object.")]
    public Transform[] SpawnParent;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public virtual async void Start()
    {
        await WaitForLoad();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Waits for load.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    public virtual async Task WaitForLoad()
    {
        while (userProfileManager.GetAllUserProfiles() == null && Application.isPlaying)
        {
            await new WaitForEndOfFrame();
        }

        await SpawnUserButtons(userProfileManager.GetAllUserProfiles());
        await new WaitForEndOfFrame();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Spawns the user buttons, needs to be cleaned up and reworked, duct taped atm.
    /// </summary>
    /// <param name="profiles">The profiles.</param>
    protected virtual async Task SpawnUserButtons(List<UserProfile> profiles)
    {
        int userProfileCount = profiles.Count;

        for (int i = 0; i < userProfileCount; ++i)
        {
            if (availableUserNameIDs.HasFlag(profiles[i].UserNameID) || IsAbbreviated)
            {
                //Debug.Log("Found available user: " + item.UserNameID, this);
                UserProfileButton button = DetermineCorrectFactoryToUse().Create(UserProfileButtonPrefab, profiles[i]);

                button.ButtonText.text = (IsAbbreviated) ? UserProfileFactory.GetAbbreviatedName(profiles[i].UserNameID) : UserProfileFactory.GetFullName(profiles[i].UserNameID);

                if (SpawnParent.Length > 0)
                {
                    for (int u = 0; u < SpawnParent.Length; u++)
                    {
                        if (SpawnParent[u].childCount < MaximumSpawnPerParent)
                        {
                            button.transform.SetParent(SpawnParent[u]);
                            button.transform.localScale = Vector3.one;
                        }
                    }
                }
                else
                {
                    button.transform.SetParent(transform);
                    button.transform.localScale = Vector3.one;
                }
            }
        }


        await new WaitForEndOfFrame();
    }

    public virtual void UserSelected(UserNameID user) { }

    protected IUserProfileButtonFactory DetermineCorrectFactoryToUse()
    {

        switch (UserProfileButtonType)
        {
            case UserAction.Login:
                return userProfileLoginButtonFactory;

            case UserAction.ToggleAnnotations:
                return userProfileActiveUserAnnotationsFactory;

            case UserAction.Select:
                return userProfileSelectUserFactory;
        }

        return null;
    }
    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif