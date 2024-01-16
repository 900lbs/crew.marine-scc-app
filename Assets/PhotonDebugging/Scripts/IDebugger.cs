using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base debugger interface.
/// </summary>
public interface IDebugger
{
    void ChangeOpacity(float val);

    void UpdateDebug();
}