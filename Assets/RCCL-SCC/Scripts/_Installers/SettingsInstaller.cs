// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="SettingsInstaller.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Security.AccessControl;
using UnityEngine;
using Zenject;

/// <summary>
/// Class SettingsInstaller.
/// Implements the <see cref="Zenject.ScriptableObjectInstaller" />
/// </summary>
/// <seealso cref="Zenject.ScriptableObjectInstaller" />
[CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]

public class SettingsInstaller : ScriptableObjectInstaller, ISerializationCallbackReceiver
{
    [Header("Overall")]
    [SerializeField]
    [EnumFlag]
    UserNameID AvailableUserLoginIds;

    [Space(10f)]
    [Header("Scene")]
    /// <summary>
    /// The settings
    /// </summary>
    [SerializeField]
    public Settings Settings;

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the bindings.
    /// </summary>
    public override void InstallBindings()
    {
        Container.BindInstance(Settings);
        Container.BindInstance(AvailableUserLoginIds).WithId("AvailableUserLogin").AsSingle();

        InstallFactories();
        InstallAnalytics();

        Application.targetFrameRate = Settings.sceneSettings.TargetFrameRate;

    }

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Installs the factories.
    /// </summary>
    void InstallFactories()
    {
        Container.BindFactory<XMLClose, XMLClose.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLCloseConfirmation, XMLCloseConfirmation.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLIsolate, XMLIsolate.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLMainPage, XMLMainPage.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLMinimize, XMLMinimize.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLOverlays, XMLOverlays.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLTimers, XMLTimers.Factory>().WhenInjectedInto<XMLBaseFactory>();
        Container.BindFactory<XMLWidgets, XMLWidgets.Factory>().WhenInjectedInto<XMLBaseFactory>();

        Container.BindFactory<UnityEngine.Object, XMLType, XMLWriterDynamic, XMLWriterDynamic.Factory>()
            .FromFactory<XMLWriterFactory>();
    }

    void OnEnable()
    {
        DeviceOrientationManager.OnScreenOrientationChanged += ScreenOrientationChanged;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        DeviceOrientationManager.OnScreenOrientationChanged -= ScreenOrientationChanged;

    }

    /// <summary>
    /// Installs the analytics.
    /// </summary>
    void InstallAnalytics()
    {
        Container.Bind<XMLBaseFactory>().AsSingle();
    }

    void ScreenOrientationChanged(ScreenOrientation orientation)
    {
        if (Settings.IsTabletUser)
            Display.displays[0].SetRenderingResolution(2736, 1824);
        else
            Display.displays[0].SetRenderingResolution(1920, 1080);
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        
    }
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Global settings
/// </summary>
[System.Serializable]
public class Settings
{
    /// <summary>
    /// The scene settings
    /// </summary>
    [SerializeField]
    public SceneSettings sceneSettings;
    /// <summary>
    /// The XML base
    /// </summary>
    [Header("XML")]
    [Space(2.5f)]
    [SerializeField]
    public xmlBase XmlBase;

    [Header("Misc.")]
    [Space(2.5f)]
    public bool IsTabletUser;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    /// <param name="scene">The scene.</param>
    /// <param name="xmlBaseRef">The XML base reference.</param>
    [Inject]
    public Settings(SceneSettings scene,
        xmlBase xmlBaseRef)
    {
        sceneSettings = scene;
        XmlBase = xmlBaseRef;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Settings that pertain to scene settings only.
    /// </summary>
    [System.Serializable]
    public class SceneSettings
    {
        /// <summary>
        /// The target frame rate
        /// </summary>
        public int TargetFrameRate;
    }
}