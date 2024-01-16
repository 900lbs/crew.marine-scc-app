using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
public class UI_ButtonFactory : IFactory<Object, UI_Button>
{
    DiContainer container;

    public UI_ButtonFactory(DiContainer contain)
    {
        container = contain;
    }
    public UI_Button Create(Object prefab)
    {
        UI_Button newButton = container.InstantiatePrefabForComponent<UI_Button>(prefab);

        return newButton;
    }
}
