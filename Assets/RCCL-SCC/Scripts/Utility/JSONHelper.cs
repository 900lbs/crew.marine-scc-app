/*
 * © 900lbs of Creative
 * Creation Date: DATE HERE
 * Date last Modified: MOST RECENT MODIFICATION DATE HERE
 * Name: AUTHOR NAME HERE
 * 
 * Description: DESCRIPTION HERE
 * 
 * Scripts referenced: LIST REFERENCED SCRIPTS HERE
 */

using UnityEngine;

public class JSONHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        JSONArrayWrapper<T> wrapper = JsonUtility.FromJson<JSONArrayWrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class JSONArrayWrapper<T>
    {
        public T[] array;
    }
}
