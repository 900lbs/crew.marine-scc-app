// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-21-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="EraseAllAnnotationsButton.cs" company="900lbs of Creative">
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
/// Handles the functionality for erasing all annotations based on a specific property.
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class EraseAllAnnotationsButton : UI_Button
{
    #region Injection Construction

    /// <summary>
    /// The annotation manager
    /// </summary>
    AnnotationManager annotationManager;


    /// <summary>
    /// Constructs the specified anno man.
    /// </summary>
    /// <param name="annoMan">The anno man.</param>
    [Inject]
    public void Construct(AnnotationManager annoMan)
    {
        annotationManager = annoMan;
    }
    #endregion

    public UserNameID EraseTheseUsers;
    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Handles everything when the state is changed, from colors to images etc.
    /// </summary>
    protected override void OnStateChange()
    {

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Automatically listens to our button and has a disable timer built in, use 0 for no disabling timer.
    /// </summary>
    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();
        AnnoMan_OnEraseAllOnProperty();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Annoes the man on erase all on property.
    /// </summary>
    void AnnoMan_OnEraseAllOnProperty()
    {
        //Send out the delete command locally first
        _signalBus.Fire<Signal_AnnoMan_OnEraseAllOnProperty>(new Signal_AnnoMan_OnEraseAllOnProperty(annotationManager.GetCurrentPropertyState(), EraseTheseUsers));

        // Let all other players know to delete all annotations under this user ID
        NetworkAnnotationObject eraseAllNetworkObject = new NetworkAnnotationObject(
        NetworkEvent.EraseAll,
        annotationManager.GetCurrentPropertyState(),
        (NetworkStorageType.LineRenderer | NetworkStorageType.Icon),
        NetworkClient.GetUserName().ToString(),
        0,
        NetworkClient.GetIsPlayerMasterClient().ToString(),
        true);

        networkClient.SendNewNetworkEvent(eraseAllNetworkObject);

        //Set our annotation state back to Move.
        annotationManager.SetCurrentAnnotationState(AnnotationState.Move, null);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif