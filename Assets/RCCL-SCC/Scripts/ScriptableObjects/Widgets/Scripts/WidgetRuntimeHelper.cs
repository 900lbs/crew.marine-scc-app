using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class WidgetRuntimeHelper<T> : MonoBehaviour
{
    #region Injection Construction
    SignalBus _signalBus { get; set; }

    void Construct(SignalBus signal)
    {
        _signalBus = signal;
    }
    #endregion
    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void FireSignal(T signal)
    {
        _signalBus.Fire<T>(signal);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
