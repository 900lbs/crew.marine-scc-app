using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The ShareViewData class is used for creating a class that consist of the current information the user is viewing.
/// Also, in this class you will find a couple of conversion classes to make life easier when it comes to moving the
/// data across the network.
/// </summary>
[System.Serializable]
public class ShareViewData
{
    #region Class Variables
    public Vector2 ParentPos;
    public Vector2 ObjPos;
    public Vector3 Rotation;
    public float ZoomValue = 1;
    public bool Rotated;
    public bool isTablet;
    public string[] DeckFeatures;
    public Dictionary<string, Deck> IsolatedDecks = new Dictionary<string, Deck>();
    #endregion

/// <summary>
/// Constructor for the class itself. Is called whenever a new ShareViewData object is requested.
/// </summary>
/// <param name="savedParentPos"></param>
/// <param name="savedObjPosition"></param>
/// <param name="savedRotation"></param>
/// <param name="savedZoomValue"></param>
/// <param name="savedRotated"></param>
/// <param name="savedDeckFeats"></param>
/// <param name="Dictionary<string"></param>
/// <param name="savedDecks"></param>
    public ShareViewData(Vector2 savedParentPos, Vector2 savedObjPosition, Vector3 savedRotation,
    float savedZoomValue, bool savedRotated, string[] savedDeckFeats, Dictionary<string, Deck> savedDecks)
    {
        ParentPos = savedParentPos;
        ObjPos = savedObjPosition;
        Rotation = savedRotation;
        ZoomValue = (savedZoomValue >= 1f) ? savedZoomValue : 1f;
        Rotated = savedRotated;
        DeckFeatures = savedDeckFeats;

        foreach(KeyValuePair<string, Deck> kvp in savedDecks)
        {
            IsolatedDecks.Add(kvp.Key, kvp.Value);
        }
    }


/// <summary>
/// Static method for converting the ShareViewData class to a string for network transport. A string is 
/// parsed from the object and separated into three main portions for parsing back to an object later. 
/// The first one is zoom data which is primarily made up of information gather from the zoomIntoDeck
/// class. The second is overlayData which holds the selected overlays, and the third is ActiveDeck data
/// which holds which decks are currently active in scene. All is parsed here and converted into a single 
/// string to be sent across the network. 
/// </summary>
/// <param name="data"></param>
/// <returns></returns>
    public static string ConvertToString(ShareViewData data)
    {
        string dataString;

        dataString = data.ParentPos.x.ToString("F3") + "+" + data.ParentPos.y.ToString("F3") + ","
         + data.ObjPos.x.ToString("F3") + "+" + data.ObjPos.y.ToString("F3") + ","
         + "0+0+" + data.Rotation.z.ToString("F3") + ","
         + data.ZoomValue.ToString("F3") + "," + data.Rotated.ToString() + ";";

         for(int i = 0; i < data.DeckFeatures.Length; i++)
         {
             if(i + 1 < data.DeckFeatures.Length)
             {
                 dataString += data.DeckFeatures[i] + ",";
             }

             else
             {
                 dataString += data.DeckFeatures[i];
             }
         }

         dataString += ";";

         dataString += string.Join(",", data.IsolatedDecks.Select(x => x.Key + "=" + x.Value.ToString().Replace(" (Deck)", "")));

        //Debug.Log("Sharemyview: " + dataString);
         return dataString;
    }

/// <summary>
/// This static method converts the string that is received by the user back into a ShareViewData class.
/// The string is split into a single array called parsedData, and then broken down further into the
/// zoomData and overlayData string arrys and the newDict Dictionary. This can be used in conjunction with
/// ReceiveView to quickly extarct and utilize the data.
/// </summary>
/// <param name="data"></param>
/// <returns></returns>
    public static ShareViewData ConvertFromString(string data)
    {
        Debug.Log("Parsing: " + data);
        string[] parsedData = data.Split(";" [0]);

        string[] zoomData = parsedData[0].Split("," [0]);

        string[] overlayData = new string[0];

        Dictionary<string, Deck> newDict = new Dictionary<string, Deck>();

        if(!string.IsNullOrEmpty(parsedData?[1]))
        {
            overlayData = parsedData[1].Split("," [0]);
        }

        if(parsedData[2] != "")
        {
            string[] activeDeckData = parsedData[2].Split("," [0]);

            foreach(string stg in activeDeckData)
            {
                string[] newPair = stg.Split("="[0]);

                newDict.Add(newPair[0], new Deck());
            }
        }

        ShareViewData shareViewData = new ShareViewData(SplitVector(zoomData[0]), SplitVector(zoomData[1]), SplitVector(zoomData[2]),
         StringToFloat(zoomData[3]), StringToBool(zoomData[4]), overlayData, newDict);
        return shareViewData;
    }

/// <summary>
/// Simple string to float converter made to tidy things up
/// </summary>
/// <param name="FloatString"></param>
/// <returns></returns>
    public static float StringToFloat(string FloatString)
    {
        float newFloat;

        float.TryParse(FloatString, out newFloat);

        return newFloat;
    }

/// <summary>
/// Simple string to bool converter made to tidy things up
/// </summary>
/// <param name="BoolString"></param>
/// <returns></returns>
    public static bool StringToBool(string BoolString)
    {
        bool value = true ? (BoolString == "True") : (BoolString == "False");

        return value;
    }

/// <summary>
/// Simple string to Vector converter made to tidy things up.true Works for both Vector2 and Vector3.
/// </summary>
/// <param name="VectorString"></param>
/// <returns></returns>
    public static Vector3 SplitVector(string VectorString)
    {
        string[] conversion = VectorString.Split("+" [0]);
        
        Vector3 newVector = new Vector3();

        if(conversion.Length == 2)
        {
            float.TryParse(conversion[0], out newVector.x);
            float.TryParse(conversion[1], out newVector.y);
        }

        if(conversion.Length == 3)
        {
            float.TryParse(conversion[0], out newVector.x);
            float.TryParse(conversion[1], out newVector.y);
            float.TryParse(conversion[2], out newVector.z);
        }

        return newVector;
    }
}
