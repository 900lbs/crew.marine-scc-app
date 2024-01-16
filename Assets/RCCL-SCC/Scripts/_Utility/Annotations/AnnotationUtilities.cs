// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-21-2019
// ***********************************************************************
// <copyright file="AnnotationUtilities.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if SCC_2_5

/// <summary>
/// Created By: Joshua Bowers - 06/14/19
/// Last Edited By: Joshua Bowers - 06/14/19
/// Helper class for annotation logic.
/// </summary>
public static class AnnotationUtilities
{
    /// <summary>
    /// Gets the prefab.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>GameObject.</returns>
    public static GameObject GetPrefab(string name)
    {
        GameObject newPrefab;

        if (name == "UI Line Renderer")
        {
            newPrefab = Resources.Load("Prefabs/" + name) as GameObject;
        }
        else
        {
            //Debug.Log("Attempting to create Icon: " + name);
            newPrefab = Resources.Load("Prefabs/SafetyIconPrefabs/" + name) as GameObject;
        }

        return newPrefab;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the annotation array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String[].</returns>
    public static async Task<string[]> GetAnnotationArray(string value)
    {
        //await new WaitForBackgroundThread();
        string convertedData = value;
        string[] splitValue = convertedData.Split(","[0]);
        //await new WaitForUpdate();
        return splitValue;
    }
}

#endif