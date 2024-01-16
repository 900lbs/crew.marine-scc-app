// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="GameStateEntity.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
/// <summary>
/// Our base game state class, new states should derive from here and implement needed functions.
/// </summary>
public abstract class GameStateEntity : IInitializable, ITickable, ILateDisposable
{
    public virtual void Initialize()
    {
        // optionally overridden
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public virtual void Start()
    {
        // optionally overridden
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public virtual void Tick()
    {
        // optionally overridden
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public virtual void LateDispose()
    {
        // optionally overridden
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
