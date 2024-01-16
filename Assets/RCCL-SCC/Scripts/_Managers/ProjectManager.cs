using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Handles all generic cross-scene functionality, such as task cancellation.
/// </summary>
public class ProjectManager : MonoBehaviour
{
    [Inject(Id = "CurrentShip")]
    ShipVariable CurrentShip;

    CancellationTokenSource c;

    ProjectSettings projectSettings;

    SignalBus _signalBus;

    [Inject]
    public void Construct(CancellationTokenSource cts, ProjectSettings settings, SignalBus signal)
    {
        c = cts;
        projectSettings = settings;
        _signalBus = signal;
    }
    // Update is called once per frame
    void OnDestroy()
    {
        ForceCleanup();
    }

    public async Task CleanupScene()
    {
        c.Cancel();
        await CleanupSceneAssets();
    }

    private void OnApplicationQuit()
    {
        c.Cancel();
    }

    public async void ForceCleanup()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        await CleanupSceneAssets();
    }

    async Task CleanupSceneAssets()
    {
        _signalBus.TryFire<Signal_ProjectManager_OnShipReset>(new Signal_ProjectManager_OnShipReset());

        await CleanupProjectAssets();
    }

    async Task CleanupProjectAssets()
    {

        #if UNITY_EDITOR
        //Implement logic to only reset if necessary, such as Shore to Ship
        Debug.Log("Resetting CurrentShip variable", this);

        if (projectSettings.DevEnabled || projectSettings.MultishipEnabled)
            CurrentShip.ResetShip();
        await new WaitForEndOfFrame();
        #endif
    }
}