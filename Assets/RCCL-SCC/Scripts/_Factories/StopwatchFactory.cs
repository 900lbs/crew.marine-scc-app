using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;
public class StopwatchFactory : IFactory<Object, StopWatch>
{
    [Inject]
    DiContainer container;

    public StopWatch Create(Object prefab)
    {
        StopWatch newStopwatch = container.InstantiatePrefabForComponent<StopWatch>(prefab);
        newStopwatch.name = newStopwatch.titleText.text;
        return newStopwatch;
    }
}
