// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-15-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-18-2019
// ***********************************************************************
// <copyright file="IAnnotationManagerSignals.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Let's subscribers know that the annotation state has changed.
/// </summary>
public class Signal_AnnoMan_OnAnnotationStateChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_AnnoMan_OnAnnotationStateChanged"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public Signal_AnnoMan_OnAnnotationStateChanged(AnnotationState state)
    {
        annotationState = state;
        //Debug.Log("Annotation state changed to " + annotationState.ToString());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_AnnoMan_OnAnnotationStateChanged"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="icon">The icon.</param>
    public Signal_AnnoMan_OnAnnotationStateChanged(AnnotationState state, object icon)
    {
        annotationState = state;
        selectIcon = icon;
        //Debug.Log("Annotation state changed to " + annotationState.ToString() + ", from icon: " + icon, icon as Object);
    }

    /// <summary>
    /// Gets the state of the annotation.
    /// </summary>
    /// <value>The state of the annotation.</value>
    public AnnotationState annotationState { get; private set; }

    /// <summary>
    /// Gets the select icon.
    /// </summary>
    /// <value>The select icon.</value>
    public object selectIcon { get; private set; }
}

/// <summary>
/// Let's subscribers know that the annotation tools have changed.
/// </summary>
public class Signal_AnnoMan_OnAnnotationToolsChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_AnnoMan_OnAnnotationToolsChanged"/> class.
    /// </summary>
    /// <param name="tool">The tool.</param>
    public Signal_AnnoMan_OnAnnotationToolsChanged(ActiveAnnotationTools tool)
    {
        ActiveTools = tool;
    }

    /// <summary>
    /// The active tools
    /// </summary>
    public ActiveAnnotationTools ActiveTools;
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Let's subscribers know that the current property state has changed.
/// </summary>
public class Signal_AnnoMan_OnRoomPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_AnnoMan_OnRoomPropertyChanged"/> class.
    /// </summary>
    /// <param name="prop">The property.</param>
    public Signal_AnnoMan_OnRoomPropertyChanged(ShipRoomProperties prop)
    {
        roomProperty = prop;
    }

    /// <summary>
    /// Gets the room property.
    /// </summary>
    /// <value>The room property.</value>
    public ShipRoomProperties roomProperty { get; private set; }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Let's subscribers know that a room property has been toggled.
/// </summary>
/// <remarks>This is used for things like turning off specific annotations under the property.</remarks>
public class Signal_AnnoMan_OnToggleRoomProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_AnnoMan_OnToggleRoomProperty"/> class.
    /// </summary>
    /// <param name="prop">The property.</param>
    public Signal_AnnoMan_OnToggleRoomProperty(ShipRoomProperties prop)
    {
        Property = prop;
    }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <value>The property.</value>
    public ShipRoomProperties Property { get; private set; }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Let's subscribers know that eraseall has been called on a specific property.
/// </summary>
public class Signal_AnnoMan_OnEraseAllOnProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_AnnoMan_OnEraseAllOnProperty"/> class.
    /// </summary>
    /// <param name="prop">The property.</param>
    public Signal_AnnoMan_OnEraseAllOnProperty(ShipRoomProperties prop, UserNameID user)
    {
        //Debug.Log("Erasing all annotations from user: " + user.ToString());
        Property = prop;
        User = user;
    }

    /// <summary>
    /// The property
    /// </summary>
    public ShipRoomProperties Property;

    public UserNameID User;
}

/*----------------------------------------------------------------------------------------------------------------------------*/

public class Signal_AnnoMan_OnActiveUserAnnotationsUpdated
{
    public Signal_AnnoMan_OnActiveUserAnnotationsUpdated(UserNameID users)
    {
        Users = users;
    }

    public UserNameID Users { get; private set; }
}