using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using Zenject;
public interface ISceneInitializer
{
    MainMenu mainMenu { get; set; }
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus { get; set; }

    CancellationTokenSource cts { get; set; }
    

    void InitializeScene(Signal_MainMenu_OnShipInitialize signal);
}
