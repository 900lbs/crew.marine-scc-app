// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-12-2019
// ***********************************************************************
// <copyright file="UserProfileButtonFactory.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
#if SCC_2_5
/// <summary>
/// Interface used by factories that want to create different variations of User Profile buttons.
/// Implements the <see cref="Zenject.IFactory{UnityEngine.Object, UserProfile, UserProfileButton}" />
/// </summary>
/// <seealso cref="Zenject.IFactory{UnityEngine.Object, UserProfile, UserProfileButton}" />
public interface IUserProfileButtonFactory : IFactory<Object, UserProfile, UserProfileButton> { }

/// <summary>
/// Used for the instantiation of login menu user buttons.
/// Implements the <see cref="Zenject.IFactory{UnityEngine.Object, UserProfile, UserProfileButton}" />
/// Implements the <see cref="IUserProfileButtonFactory" />
/// </summary>
/// <seealso cref="Zenject.IFactory{UnityEngine.Object, UserProfile, UserProfileButton}" />
/// <seealso cref="IUserProfileButtonFactory" />
public class UserProfileSignInButtonFactory : IFactory<Object, UserProfile, UserProfileButton>, IUserProfileButtonFactory
{
    #region Injection Construction
    /// <summary>
    /// The container
    /// </summary>
    readonly DiContainer _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileSignInButtonFactory"/> class.
    /// </summary>
    /// <param name="container">The container.</param>
    [Inject]
    public UserProfileSignInButtonFactory(DiContainer container)
    {
        _container = container;
    }

    #endregion

    /// <summary>
    /// Creates the specified prefab.
    /// </summary>
    /// <param name="prefab">The prefab.</param>
    /// <param name="user">The user.</param>
    /// <returns>UserProfileButton.</returns>
    public UserProfileButton Create(Object prefab, UserProfile user)
    {
        UserProfileButton button = _container.InstantiatePrefabForComponent<UserProfileButton>(prefab);
        button.ButtonText.text = user.UserNameID.ToString();
        button.User = user;

        button.transform.localScale = Vector3.one;
        return button;
    }
}

/// <summary>
/// Placeholder, add other userprofile button creation functionality here.
/// </summary>
/// <typeparam name="Object"></typeparam>
/// <typeparam name="UserProfile"></typeparam>
/// <typeparam name="UserProfileButton"></typeparam>
public class UserProfileUserAnnotationButtonFactory : IFactory<Object, UserProfile, UserProfileButton>
{
    readonly DiContainer _container;

    [Inject]
    public UserProfileUserAnnotationButtonFactory(DiContainer container)
    {
        _container = container;
    }

    public UserProfileButton Create(Object prefab, UserProfile user)
    {
        UserProfileButton button = _container.InstantiatePrefabForComponent<UserProfileButton>(prefab);
        button.UserButtonAction = UserAction.ToggleAnnotations;
        button.User = user;
        button.ButtonText.text = user.UserNameID.ToString();
        button.transform.localScale = Vector3.one;

        return button;
    }
}

public class UserProfileUserSelectionButtonFactory : IFactory<Object, UserProfile, UserProfileButton>
{
    readonly DiContainer _container;

    [Inject]
    public UserProfileUserSelectionButtonFactory(DiContainer container)
    {
        _container = container;
    }

    public UserProfileButton Create(Object prefab, UserProfile user)
    {
        UserProfileButton button = _container.InstantiatePrefabForComponent<UserProfileButton>(prefab);
        button.UserButtonAction = UserAction.Select;
        button.User = user;
        button.ButtonText.text = user.UserNameID.ToString();
        button.transform.localScale = Vector3.one;
        button.UpdateSelectedColorBasedOnUser();

        return button;
    }
}
#endif