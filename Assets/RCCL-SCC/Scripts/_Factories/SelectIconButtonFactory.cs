// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 05-15-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-12-2019
// ***********************************************************************
// <copyright file="SelectIconButtonFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
using Object = UnityEngine.Object;

#if SCC_2_5
/// <summary>
/// Handles the creation of a new SelectIcon button.
/// </summary>
/// <typeparam name="Object"></typeparam>
/// <typeparam name="SafetyIconData"></typeparam>
/// <typeparam name="SelectIcon"></typeparam>
public class SelectIconButtonFactory : IFactory<Object, SafetyIconData, SelectIcon>
{
    #region Injection Construction
    SelectIcon.Factory selectIconFactory;
    readonly DiContainer _container;

    [Inject]
    public SelectIconButtonFactory(SelectIcon.Factory selectIconFact, DiContainer container)
    {
        selectIconFactory = selectIconFact;
        _container = container;
    }

    #endregion

    /*------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Feed the prefab and data through in order to instantiate a new button with all injections.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public SelectIcon Create(Object prefab, SafetyIconData data)
    {
        SelectIcon newIconButton = _container.InstantiatePrefabForComponent<SelectIcon>(prefab);

        newIconButton.IconData = data;

        newIconButton.name = data.IconName + "_Button";

        newIconButton.State = ActiveState.Enabled;

        //newIconButton.PrefabTransform = data.GetSizeDeltaFromPrefab();

        newIconButton.GetComponent<Image>().sprite = newIconButton.IconData.IconImage;

        return newIconButton;
    }


}
#endif