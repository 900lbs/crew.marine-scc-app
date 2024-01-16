// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="NetworkUtilities.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
#if SCC_2_5
/// <summary>
/// Created By: Joshua Bowers - 06/14/19
/// Last Edited By: Joshua Bowers - 06/14/19
/// Purpose: Utility class that handles uniformly packaging and breaking down network data.
/// </summary>
public static class NetworkUtilities
{
    /// <summary>
    /// Creates the network object.
    /// </summary>
    /// <param name="newNetEvent">The new net event.</param>
    /// <param name="newNetOptionalRoomProp">The new net optional room property.</param>
    /// <param name="newNetStorageType">New type of the net storage.</param>
    /// <param name="newPlayerID">The new player identifier.</param>
    /// <param name="newNetID">The new net identifier.</param>
    /// <param name="newNetData">The new net data.</param>
    /// <param name="newSetDirty">if set to <c>true</c> [new set dirty].</param>
    /// <returns>NetworkAnnotationObject.</returns>
    public static NetworkAnnotationObject CreateNetworkObject(NetworkEvent newNetEvent, ShipRoomProperties newNetOptionalRoomProp, NetworkStorageType newNetStorageType, string newPlayerID, short newNetID, object newNetData, bool newSetDirty)
    {
        NetworkAnnotationObject newObject = new NetworkAnnotationObject
        (
        newNetEvent,
        newNetOptionalRoomProp,
        newNetStorageType,
        newPlayerID,
        newNetID,
        newNetData,
        newSetDirty
        );

        return newObject;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Converts the object to network object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>NetworkAnnotationObject.</returns>
    public static NetworkAnnotationObject ConvertObjectToNetworkObject(object value)
    {
        object[] values = (object[])value;
        NetworkAnnotationObject ToNetworkObject = new NetworkAnnotationObject
        (
            (NetworkEvent)values[0],
            (ShipRoomProperties)values[1],
            (NetworkStorageType)values[2],
            (string)values[3],
            (short)values[4],
            (object)values[5],
            (bool)values[6]
        );

        return ToNetworkObject;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    /// Converts to line rend save.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>NewLineRendererSave.</returns>
    [Obsolete]
    public static NewLineRendererSave ConvertToLineRendSave(string[] array, string playerID)
    {
        NewLineRendererSave newSave = new NewLineRendererSave();

        try
        {
            NetworkStorageType storage;
            Enum.TryParse(array[0], out storage);
            newSave.StorageType = storage;
        }
        catch
        {
            Debug.LogError("Could not parse enum: " + array[0]);
        }

        newSave.PrefabName = array[1];

        try
        {
            newSave.ID = short.Parse(array[2]);
        }
        catch (System.Exception)
        {
            Debug.LogError("Could not part array object to short: " + array[2]);
            throw;
        }
        newSave.DeckID = array[3];
        newSave.Color = array[4];
        newSave.Thickness = array[5];
        newSave.IsHighlighter = array[6];
        newSave.PlayerID = playerID;

        Vector2[] linePoints = new Vector2[array.Length - 8];
        for (int i = 0; i < linePoints.Length; i++)
        {
            string[] vectorPoint = array[i + 8].Split("="[0]);

            linePoints[i].x = float.Parse(vectorPoint[0]);
            linePoints[i].y = float.Parse(vectorPoint[1]);
        }
        newSave.Points = linePoints;

        try
        {
            Enum.TryParse(array[7], out newSave.Props);
            return newSave;
        }
        catch
        {
            Debug.LogError("NetworkUtilities could not parse a valid ShipRoomProperty from: " + array[7]);
            return null;
        }

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Converts to icon save.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>NewIconSave.</returns>
    [Obsolete]
    public static NewIconSave ConvertToIconSave(string[] array, string playerID)
    {
        NewIconSave newSave = new NewIconSave();

        try
        {
            NetworkStorageType storage;
            Enum.TryParse(array[0], out storage);
            newSave.StorageType = storage;
        }
        catch
        {
            Debug.LogError("Could not parse enum: " + array[0]);
        }

        newSave.PrefabName = array[1];
        newSave.PlayerID = array[2];
        newSave.ID = short.Parse(array[3]);
        newSave.DeckID = array[4];
        newSave.CurrentPosition = StringToVector3(new string[3] { array[5], array[6], array[7] });
        try
        {
            Enum.TryParse(array[8], out newSave.Prop);
        }
        catch
        {
            Debug.LogError("NetworkUtilities could not parse a valid ShipRoomProperty from: " + array[8]);
            return null;
        }
        newSave.Text = array[9];
        return newSave;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Strings to vector3.
    /// </summary>
    /// <param name="sVector">The s vector.</param>
    /// <returns>Vector3.</returns>
    public static Vector3 StringToVector3(string sVector)
    {
        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log("Attempting to convert: " + sVector + " to a Vector3.");

        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Strings to vector3.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>Vector3.</returns>
    public static Vector3 StringToVector3(string[] vector)
    {
        for (int i = 0; i < vector.Length; i++)
        {
            if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                Debug.Log("Attempting to convert to vector coordinate:" + vector[i]);

            if (vector[i].StartsWith("("))
            {
                vector[i] = vector[i].Substring(1, vector[i].Length - 1);
            }

            else if (vector[i].EndsWith(")"))
            {
                vector[i] = vector[i].Substring(0, vector[i].Length - 2);
            }

            if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                        Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                Debug.Log("Vector coordinate result: " + vector[i]);
        }

        float x = 0;
        float y = 0;
        float z = 0;

        if (!float.TryParse(vector[0], out x))
        {
            Debug.LogError("Could not parse: " + vector[0]);
        }
        if (!float.TryParse(vector[1], out y))
        {
            Debug.LogError("Could not parse: " + vector[0]);
        }
        if (!float.TryParse(vector[2], out z))
        {
            Debug.LogError("Could not parse: " + vector[0]);
        }
        // split the items


        // store as a Vector3
        Vector3 result = new Vector3(x, y, z);

        return result;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/
}
#endif