// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-10-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="_AnnotationClasses.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using Zenject;

#if SCC_2_5

public interface AnnotationSave
{
    /// <summary>
    /// This field is required to be the first parameter sent in the network string or array.
    /// </summary>
    /// <value>The type of the storage.</value>
    NetworkStorageType StorageType { get; set; }

    /// <summary>
    /// Gets the single line.
    /// </summary>
    /// <returns>System.String.</returns>
    string GetSingleLine();
    /// <summary>
    /// Sets the by single line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>Task.</returns>
    Task<bool> SetBySingleLine(string value, string playerID);
    /// <summary>
    /// Sets the by array.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>Task.</returns>
    Task SetByArray(string[] array, string playerID);

}

/// <summary>
/// Icon annotation internal data.
/// Implements the <see cref="AnnotationSave" />
/// </summary>
/// <seealso cref="AnnotationSave" />
[System.Serializable]
public class NewIconSave : AnnotationSave
{
    /// <summary>
    /// This field is required to be the first parameter sent in the network string or array.
    /// </summary>
    /// <value>The type of the storage.</value>
    public NetworkStorageType StorageType { get; set; }
    /// <summary>
    /// The prefab name
    /// </summary>
    public string PrefabName;
    /// <summary>
    /// The player identifier
    /// </summary>
    public string PlayerID;
    /// <summary>
    /// The identifier
    /// </summary>
    public short ID;
    /// <summary>
    /// The deck identifier
    /// </summary>
    public string DeckID;
    /// <summary>
    /// The current position
    /// </summary>
    public Vector3 CurrentPosition;
    /// <summary>
    /// The text
    /// </summary>
    public string Text;
    /// <summary>
    /// The property
    /// </summary>
    public ShipRoomProperties Prop;

    internal CancellationTokenSource localC;

    /// <summary>
    /// Gets the single line.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GetSingleLine()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(StorageType);
        sb.Append(",");
        sb.Append(PrefabName);
        sb.Append(",");
        sb.Append(PlayerID);
        sb.Append(",");
        sb.Append(ID);
        sb.Append(",");
        sb.Append(DeckID);
        sb.Append(",");
        sb.Append(CurrentPosition.ToString("F3"));
        sb.Append(",");
        sb.Append(Prop.ToString());
        /*         string networkString = StorageType + ","
                         + PrefabName + ","
                         + PlayerID + ","
                         + ID + ","
                         + DeckID + ","
                         + CurrentPosition.ToString("F3") + ","
                         + Prop.ToString(); */

        if (!string.IsNullOrEmpty(Text))
        {
            sb.Append("," + Text);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Sets the by single line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>Task.</returns>
    public async Task<bool> SetBySingleLine(string value, string playerID)
    {
        localC = new CancellationTokenSource();

        string[] array = value.Split(","[0]);

        if (!localC.IsCancellationRequested)
        {
            try
            {
                NetworkStorageType storage;
                Enum.TryParse(array[0], out storage);
                StorageType = storage;
            }
            catch
            {
                Debug.LogError("Could not parse enum: " + array[0]);
                localC.Cancel();
                return false;
            }

            PrefabName = array[1];
            PlayerID = array[2];
            if(!short.TryParse(array[3], out ID))
                Debug.LogError("Incorrect ID, must be a short.");
            DeckID = array[4];
            CurrentPosition = NetworkUtilities.StringToVector3(new string[3] { array[5], array[6], array[7] });

            try
            {
                Enum.TryParse(array[8], out Prop);
            }
            catch
            {
                Debug.LogError("NetworkUtilities could not parse a valid ShipRoomProperty from: " + array[8]);
                localC.Cancel();
                return false;
            }

            if (array.Length > 9)
                Text = array[9];

            await new WaitForEndOfFrame();
        }

        return true;

    }
    /// <summary>
    /// Sets the by array.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>Task.</returns>
    public async Task SetByArray(string[] array, string playerID)
    {
        localC = new CancellationTokenSource();

        if (!localC.IsCancellationRequested)
        {
            try
            {
                NetworkStorageType storage;
                Enum.TryParse(array[0], out storage);
                StorageType = storage;
            }
            catch
            {
                Debug.LogError("Could not parse enum: " + array[0]);
                localC.Cancel();
            }

            PrefabName = array[1];
            PlayerID = array[2];
            ID = short.Parse(array[3]);
            DeckID = array[4];
            CurrentPosition = NetworkUtilities.StringToVector3(new string[3] { array[5], array[6], array[7] });
            try
            {
                Enum.TryParse(array[8], out Prop);
            }
            catch
            {
                Debug.LogError("NetworkUtilities could not parse a valid ShipRoomProperty from: " + array[8]);
                localC.Cancel();
                return;
            }

            if (array.Length >= 10)
                Text = array[9];

            await new WaitForEndOfFrame();
        }


    }
}



/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// LineRenderer annotation internal data.
/// Implements the <see cref="AnnotationSave" />
/// </summary>
/// <seealso cref="AnnotationSave" />
[System.Serializable]
public class NewLineRendererSave : AnnotationSave
{
    /// <summary>
    /// This field is required to be the first parameter sent in the network string or array.
    /// </summary>
    /// <value>The type of the storage.</value>
    public NetworkStorageType StorageType { get; set; }
    /// <summary>
    /// The prefab name
    /// </summary>
    public string PrefabName;
    /// <summary>
    /// The player identifier
    /// </summary>
    public string PlayerID;
    /// <summary>
    /// The identifier
    /// </summary>
    public short ID;
    /// <summary>
    /// The deck identifier
    /// </summary>
    public string DeckID;
    /// <summary>
    /// The color
    /// </summary>
    public string Color;
    /// <summary>
    /// The thickness
    /// </summary>
    public string Thickness;
    /// <summary>
    /// The is highlighter
    /// </summary>
    public string IsHighlighter;
    /// <summary>
    /// The points
    /// </summary>
    public Vector2[] Points;
    /// <summary>
    /// The props
    /// </summary>
    public ShipRoomProperties Props;

    internal CancellationTokenSource localC;

    /// <summary>
    /// Gets the single line.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GetSingleLine()
    {
        StringBuilder singleLine = new StringBuilder();

        singleLine.Append(StorageType.ToString()); //0
        singleLine.Append(",");
        singleLine.Append(PrefabName); //1
        singleLine.Append(",");
        singleLine.Append(PlayerID); //2
        singleLine.Append(",");
        singleLine.Append(ID.ToString()); //3
        singleLine.Append(",");
        singleLine.Append(DeckID); //4
        singleLine.Append(",");
        singleLine.Append(Color); //5
        singleLine.Append(",");
        singleLine.Append(Thickness); //6
        singleLine.Append(",");
        singleLine.Append(IsHighlighter);  //7
        singleLine.Append(",");
        singleLine.Append(Props); //8
        singleLine.Append(",");

        int pointsLength = Points.Length;

        if (pointsLength > 0)
        {
            string[] points = new string[pointsLength];

            for (int k = 0; k < pointsLength; ++k)
            {
                points[k] = Points[k].x.ToString() + "=" + Points[k].y.ToString();
                singleLine.Append(points[k]); //9+
                if (k < points.Length - 1)
                    singleLine.Append(",");
            }
        }


        return singleLine.ToString();
    }

    /// <summary>
    /// Sets the by single line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>Task.</returns>
    /// <exception cref="System.Exception">
    /// </exception>
    public async Task<bool> SetBySingleLine(string value, string playerID)
    {
        await new WaitForBackgroundThread();

        localC = new CancellationTokenSource();

        if (!localC.IsCancellationRequested)
        {
            string[] array = value.Split(","[0]);

            if (array.Length > 2)
            {
                try
                {
                    NetworkStorageType storage;
                    Enum.TryParse(array?[0], out storage);
                    StorageType = storage;
                    PrefabName = array?[1];

                    PlayerID = (string.IsNullOrEmpty(playerID) ? array?[2] : playerID);
                }
                catch (System.Exception)
                {
                    localC.Cancel();
                    return false;
                }

                try
                {
                    ID = short.Parse(array?[3]);
                    DeckID = array?[4];
                    Color = array?[5];
                    Thickness = array?[6];
                    IsHighlighter = array?[7];
                    Enum.TryParse(array?[8], out Props);

                }
                catch (System.Exception)
                {
                    localC.Cancel();
                    return false;
                }

                await new WaitForUpdate();

                Vector2[] linePoints = new Vector2[array.Length - 9];
                int index = 0;
                try
                {
                    for (int i = 0; i < linePoints.Length; i++)
                    {
                        index = i;
                        string[] vectorPoint = array?[i + 9].Split("="[0]);

                        float.TryParse(vectorPoint?[0], out linePoints[i].x);
                        float.TryParse(vectorPoint?[1], out linePoints[i].y);
                    }

                    Points = linePoints;
                }
                catch (System.Exception)
                {
                    localC.Cancel();
                    return false;
                }

            }
            await new WaitForEndOfFrame();
        }

        return true;
    }

    /// <summary>
    /// Sets the by array.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="playerID">The player identifier.</param>
    /// <returns>Task.</returns>
    /// <exception cref="System.Exception">
    /// </exception>
    public async Task SetByArray(string[] array, string playerID)
    {
        await new WaitForBackgroundThread();

        var localC = new CancellationTokenSource();

        if (!localC.IsCancellationRequested)
        {
            try
            {
                NetworkStorageType storage;
                Enum.TryParse(array[0], out storage);
                StorageType = storage;
                PrefabName = array[1];
                PlayerID = (string.IsNullOrEmpty(playerID) ? array[2] : playerID);
            }
            catch (System.Exception)
            {
                localC.Cancel();
                return;
            }

            try
            {
                ID = short.Parse(array[3]);
                DeckID = array[4]; //Debug.Log("DeckID: " + DeckID);
                Color = array[5]; //Debug.Log("Color: " + Color);
                Thickness = array[6];
                IsHighlighter = array[7];
                Enum.TryParse(array[8], out Props); //Debug.Log("Properties: " + Props);

            }
            catch (System.Exception)
            {
                localC.Cancel();
                return;
            }
            await new WaitForUpdate();

            Vector2[] linePoints = new Vector2[array.Length - 9];
            try
            {
                for (int i = 0; i < linePoints.Length; i++)
                {

                    string[] vectorPoint = array[i + 9].Split("="[0]);

                    float.TryParse(vectorPoint[0], out linePoints[i].x);
                    float.TryParse(vectorPoint[1], out linePoints[i].y);
                }

                Points = linePoints;
            }
            catch (System.Exception)
            {
                localC.Cancel();
                return;
            }

            await new WaitForEndOfFrame();
        }
    }
}

/*----------------------------------------------------------------------------------------------------------------------------*/

#endif