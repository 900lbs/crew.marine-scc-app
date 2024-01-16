// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="MultiSceneManager.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

/// <summary>
/// Class MultiSceneManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MultiSceneManager : MonoBehaviour
{
	#region Injection Construction
	/// <summary>
	/// The scene loader
	/// </summary>
	[Inject]
    readonly ZenjectSceneLoader sceneLoader;

	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Awakes this instance.
	/// </summary>
	void Awake()
    {
#if !UNITY_EDITOR
        LoadShipScene(1);
#endif
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Loads the ship scene.
	/// </summary>
	/// <param name="scene">The scene.</param>
	public void LoadShipScene(int scene)
    {
        LoadSceneAdditively(scene, false);
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Loads the scene additively.
	/// </summary>
	/// <param name="sceneName">Name of the scene.</param>
	public static void LoadSceneAdditively(int sceneName, bool unloadScene)
    {
		if(unloadScene)
			SceneManager.UnloadSceneAsync(sceneName);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}