// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-25-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-27-2019
// ***********************************************************************
// <copyright file="SceneInstaller.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using Zenject;

/// <summary>
/// Class SceneInstaller.
/// Implements the <see cref="Zenject.MonoInstaller" />
/// </summary>
/// <seealso cref="Zenject.MonoInstaller" />
public class SceneInstaller : MonoInstaller
{
    public SceneSequenceBinding AssignedSceneSequenceBinding;
    /// <summary>
    /// Installs the bindings.
    /// </summary>
    public override void InstallBindings()
    {
        InstallSceneSequencer();
        //InstallEniram();
    }

    /// <summary>
    /// Installs the scene sequencer.
    /// </summary>
    void InstallSceneSequencer()
    {
        switch (AssignedSceneSequenceBinding)
        {
            case SceneSequenceBinding.Release:
                Container.BindInterfacesAndSelfTo<SceneSequenceManager>()
                .AsSingle()
                .NonLazy();
                break;
            case SceneSequenceBinding.NetworkingTextureTest:
                Container.BindInterfacesAndSelfTo<TextureOverNetworkSceneManager>()
                .AsSingle()
                .NonLazy();
                break;

        }
    }

    /// <summary>
    /// Installs the eniram.
    /// </summary>
    // void InstallEniram()
    // {
    //     Container.BindInterfacesAndSelfTo<EniramDataFeed>()
    //     .AsSingle()
    //     .NonLazy();
    // }
}

public enum SceneSequenceBinding
{
    Release,
    NetworkingTextureTest
}