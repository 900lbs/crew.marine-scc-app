using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class TextureOverNetworkSceneManager : ISceneInitializer, ILateDisposable
{
    public MainMenu mainMenu { get; set; }
    public SignalBus _signalBus { get; set; }
    public CancellationTokenSource cts { get; set; }

    public NetworkClient networkClient;
    public TextureOverNetworkSceneManager(MainMenu menu, SignalBus signal, CancellationTokenSource c, NetworkClient netClient)
    {
        mainMenu = menu;
        _signalBus = signal;
        cts = c;
        networkClient = netClient;
        _signalBus.Subscribe<Signal_MainMenu_OnShipInitialize>(InitializeScene);
    }

    /// <summary>
    /// Lates the dispose.
    /// </summary>
    public void LateDispose()
    {
        _signalBus.Unsubscribe<Signal_MainMenu_OnShipInitialize>(InitializeScene);
    }

    public void InitializeScene(Signal_MainMenu_OnShipInitialize signal)
    {
        mainMenu.CurrentSceneState = SceneState.Loading;
        mainMenu.ChangeState(GameState.Bridging);

        //Initialize network settings
        networkClient.InitializeNetworkSettings("TextureTesting");

        /*----------------------------------------------------------------------------------------------------------------------------*/


        //Set MainMenu to userselection for signing in.
        mainMenu.ChangeState(GameState.UserSelection);
        mainMenu.CurrentSceneState = SceneState.Idle;
    }
}
