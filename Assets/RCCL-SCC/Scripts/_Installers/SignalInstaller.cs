// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-07-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-27-2019
// ***********************************************************************
// <copyright file="SignalInstaller.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Our Signal installer, be sure to add any new signals to be installed here.
/// Implements the <see cref="Zenject.Installer{SignalInstaller}" />
/// </summary>
/// <seealso cref="Zenject.Installer{SignalInstaller}" />
public class SignalInstaller : Installer<SignalInstaller>
{
    /// <summary>
    /// Installs the bindings.
    /// </summary>
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        DeclareAnnotationManagerSignals();
        DeclareExternalDataSignals();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Declares the annotation manager signals.
    /// </summary>
    public void DeclareAnnotationManagerSignals()
    {
        Container.DeclareSignal<Signal_AnnoMan_OnActiveUserAnnotationsUpdated>().OptionalSubscriber();
        Container.DeclareSignal<Signal_AnnoMan_OnAnnotationStateChanged>().OptionalSubscriber();
        Container.DeclareSignal<Signal_AnnoMan_OnAnnotationToolsChanged>().OptionalSubscriber();
        Container.DeclareSignal<Signal_AnnoMan_OnEraseAllOnProperty>().OptionalSubscriber();
        Container.DeclareSignal<Signal_AnnoMan_OnRoomPropertyChanged>().OptionalSubscriber();
        Container.DeclareSignal<Signal_AnnoMan_OnToggleRoomProperty>().OptionalSubscriber();
        Container.DeclareSignal<Signal_KillCards_ReceiveData>().OptionalSubscriber();
        Container.DeclareSignal<Signal_KillCards_RequestData>().OptionalSubscriber();
        Container.DeclareSignal<Signal_KillCards_SendData>().OptionalSubscriber();
        Container.DeclareSignal<Signal_MainMenu_OnGameStateChanged>().OptionalSubscriber();
        Container.DeclareSignal<Signal_MainMenu_OnPromptMenuChanged>().OptionalSubscriber();
        Container.DeclareSignal<Signal_MainMenu_OnShipInitialize>().OptionalSubscriber();
        Container.DeclareSignal<Signal_MainMenu_OnShipInitialized>().OptionalSubscriber();
        Container.DeclareSignal<Signal_NetworkClient_OnActiveUsersUpdated>().OptionalSubscriber();
        Container.DeclareSignal<Signal_NetworkClient_OnDisconnectionOccured>().OptionalSubscriber();
        Container.DeclareSignal<Signal_NetworkClient_OnJoinRoomFailed>().OptionalSubscriber();
        Container.DeclareSignal<Signal_NetworkClient_OnClientStateChanged>().OptionalSubscriber();
        Container.DeclareSignal<Signal_NetworkClient_OnNetworkEventReceived>().OptionalSubscriber();
        Container.DeclareSignal<Signal_NetworkClient_OnOnlineOfflineToggled>().OptionalSubscriber();
        Container.DeclareSignal<Signal_ProjectManager_OnShipReset>().OptionalSubscriber();
        Container.DeclareSignal<Signal_ShipManager_OnFeatureToolCreated>().OptionalSubscriber();
        Container.DeclareSignal<Signal_UserProfile_UserActivityUpdated>().OptionalSubscriber();
        Container.DeclareSignal<Signal_Widget_StateChanged>().OptionalSubscriber();

    }

    /// <summary>
    /// Declares the external data signals.
    /// </summary>
    public void DeclareExternalDataSignals()
    {
        Container.DeclareSignal<Signal_EniramData_DataUpdated>().OptionalSubscriber();
        Container.DeclareSignal<Signal_HTTP_OnPostCapabilitiesChanged>().OptionalSubscriber();
    }

}
