using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
public class PromptMenuFactory : IFactory<Object, PromptMenu>
{
    readonly PromptMenu.Factory promptMenuFactory;
    readonly DiContainer container;

    [Inject]
    public PromptMenuFactory(PromptMenu.Factory promptMenuFact, DiContainer contain)
    {
        promptMenuFactory = promptMenuFact;
        container = contain;
    }

    public PromptMenu Create(Object prefab)
    {
        PromptMenu newMenu = container.InstantiatePrefabForComponent<PromptMenu>(prefab);
        
        GameObject parent = GameObject.FindGameObjectWithTag("Prompt");
        newMenu.transform.SetParent(parent.transform); //make this better later
        newMenu.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;

        return newMenu;
    }
}
