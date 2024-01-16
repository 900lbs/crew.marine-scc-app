// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-16-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-24-2019
// ***********************************************************************
// <copyright file="UIInstaller.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;

#if SCC_2_5

/// <summary>
/// Installs ship scene UI
/// Implements the <see cref="Zenject.MonoInstaller" />
/// </summary>
/// <seealso cref="Zenject.MonoInstaller" />
public class UIInstaller : MonoInstaller
{
    /// <summary>
    /// The annotation manager
    /// </summary>
    [Header("Managers")]
    public GameObject AnnotationMan;
    /// <summary>
    /// The icon manager
    /// </summary>
    public SafetyIconManager IconManager;
    /// <summary>
    /// The pan deck
    /// </summary>
    public PanDeckHolder PanDeck;

    public ShareReceiveView ShareMyView;


    /// <summary>
    /// The UI settings
    /// </summary>
    [Space(5f)]
    [Header("Settings")]
    public ShipUIVariable UISettings;
    /// <summary>
    /// The UI objects class that contains necessary spawn points.
    /// </summary>
    [SerializeField] public UIObjects UIObjectsClass;

    /// <summary>
    /// The annotation builder
    /// </summary>
    [Space(5f)]
    [Header("Builders")]
    public GameObject AnnotationBuilder;

    /// <summary>
    /// The line renderer prefab
    /// </summary>
    [Space(5)]
    [Header("UI Prefabs")]
    public GameObject LineRendererPrefab;
    /// <summary>
    /// The icon button prefab
    /// </summary>
    public GameObject IconButtonPrefab;

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the bindings.
    /// </summary>
    public override void InstallBindings()
    {
        BindFactories();
        InstallUIManager();
        InstallAnnotationManager();
        InstallIconManager();
        InstallDrawing();
        InstallShareView();
        InstallAnnotationComponents();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the UI manager.
    /// </summary>
    public void InstallUIManager()
    {

        // Construct and Bind UIManager as our singleton instance immediately.
        Container.BindInterfacesAndSelfTo<UIManager>()
        .AsSingle()
        .WithArguments(UISettings, UIObjectsClass)
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the annotation manager.
    /// </summary>
    void InstallAnnotationManager()
    {
        Container.BindInterfacesAndSelfTo<AnnotationManager>()
        .FromComponentOn(AnnotationMan)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the icon manager.
    /// </summary>
    void InstallIconManager()
    {
        Container.Bind<SafetyIconManager>()
        .FromComponentOn(IconManager.gameObject)
        .AsSingle()
        .NonLazy();

    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the drawing.
    /// </summary>
    void InstallDrawing()
    {
        Container.BindInterfacesAndSelfTo<BuildSaveGAEdits>()
        .FromComponentOn(AnnotationMan)
        .AsSingle()
        .NonLazy();

        Container.Bind<PanDeckHolder>()
        .FromComponentOn(PanDeck.gameObject)
        .AsSingle()
        .NonLazy();
    }

    void InstallShareView()
    {
        Container.Bind<ShareReceiveView>()
        .FromComponentOn(ShareMyView.gameObject)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the annotation components.
    /// </summary>
    void InstallAnnotationComponents()
    {
        Container.BindInterfacesAndSelfTo<AnnotationBuilder>()
        .FromComponentInNewPrefab(AnnotationBuilder)
        .AsSingle()
        .NonLazy();
    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Binds the factories.
    /// </summary>
    void BindFactories()
    {		
        Container.BindFactory<NewLineRendererSave, LineBehavior, LineRendererFactory>()
        .FromComponentInNewPrefab(LineRendererPrefab);

        Container.BindFactory<Object, NewIconSave, IconBehavior, IconBehavior.Factory>()
        .FromFactory<IconFactory>();

        Container.BindFactory<Object, SafetyIconData, SelectIcon, SelectIcon.Factory>()
        .FromFactory<SelectIconButtonFactory>();

        Container.BindFactory<Object, StopWatch, StopWatch.Factory>()
        .FromFactory<StopwatchFactory>();

        Container.BindFactoryCustomInterface<Object, UserProfile, UserProfileButton, UserProfileButton.Factory, IUserProfileButtonFactory>()
        .WithId(UserAction.ToggleAnnotations)
        .FromFactory<UserProfileUserAnnotationButtonFactory>();

        Container.BindFactoryCustomInterface<Object, UserProfile, UserProfileButton, UserProfileButton.Factory, IUserProfileButtonFactory>()
        .WithId(UserAction.Select)
        .FromFactory<UserProfileUserSelectionButtonFactory>();
    }

    /*------------------------------------------------------------------------------------------------*/

    void BindMisc()
    {
        Container.Bind<ColorProfile>()
        .AsTransient();
    }

}
#endif