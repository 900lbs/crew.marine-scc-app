using System.Threading;
using Zenject;

using Sirenix.OdinInspector;

public class ProjectInstaller : MonoInstaller
{
    public ShipVariable CurrentShip;

    public ProjectSettings CurrentProjectSettings;

    public override void InstallBindings()
    {
        InstallSharedSceneData();
        InstallHelpers();
        InstallTaskFunctionality();
    }

    private void InstallSharedSceneData()
    {
        SignalInstaller.Install(Container);

        Container.BindInstance(CurrentShip)
        .WithId("CurrentShip")
        .AsSingle()
        .NonLazy();

        Container.Bind<ProjectSettings>()
        .FromInstance(CurrentProjectSettings)
        .AsSingle()
        .NonLazy();
    }

    private void InstallHelpers()
    {
        Container.Bind<ProjectManager>()
        .FromNewComponentOnRoot()
        .AsSingle()
        .NonLazy();
    }

    private void InstallTaskFunctionality()
    {
        Container.Bind<CancellationTokenSource>()
        .FromNew()
        .AsSingle();
    }
}

[System.Serializable]
public struct ProjectSettings
{
    public bool DebugEnabled;
    public bool DevEnabled;
    public bool MultishipEnabled;
    public ShipVariable TargetShip;
    public byte AntiAliasing;
}