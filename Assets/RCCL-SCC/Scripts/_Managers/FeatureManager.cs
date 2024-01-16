using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FeatureManager
{
    public static Dictionary<string, ShipFeature> ActiveFeaturesDict;
    public static Dictionary<string, ShipFeature> AllFeaturesDict;

    public static string[] GetActiveFeatures
    {
        get
        {
            if(ActiveFeaturesDict == null)
                ActiveFeaturesDict = new Dictionary<string, ShipFeature>();
                
            string[] newList = new string[ActiveFeaturesDict.Count];
            ActiveFeaturesDict.Keys.CopyTo(newList, 0);
            return newList;
        }
    }

    /// <summary>
    /// Should only be used by features when their state is changed.
    /// </summary>
    /// <param name="feature"></param>
    public static void AddOrRemoveFromActiveFeaturesList(ShipFeature feature, bool active)
    {
        if (ActiveFeaturesDict == null)
            ActiveFeaturesDict = new Dictionary<string, ShipFeature>();

        if (ActiveFeaturesDict.ContainsKey(feature.FeatureData.FeatureName) && !active)
        {
            //Debug.Log("Removing feature from active list: " + feature.FeatureData.FeatureName);
            ActiveFeaturesDict.Remove(feature.FeatureData.FeatureName);
        }
        else if(!ActiveFeaturesDict.ContainsKey(feature.FeatureData.FeatureName) && active)
        {
            //Debug.Log("Adding feature to active list: " + feature.FeatureData.FeatureName);
            ActiveFeaturesDict.Add(feature.FeatureData.FeatureName, feature);
        }
    }

    /// <summary>
    /// Filters through the AllFeaturesDict dictionary and sets the ones that match
    /// one of the array's values to Selected and the rest to enabled, use this method for setting features via script.
    /// </summary>
    /// <param name="activeFeatures"></param>
    public static void SetActiveFeaturesManually(string[] activeFeatures)
    {
        //Debugging
        string listOfFeatures = "";
        int activeFeaturesCount = activeFeatures.Length;
        for (int i = 0; i < activeFeaturesCount; ++i)
        {
            listOfFeatures += activeFeatures[i] + ", ";
        }

    
        List<string> features = new List<string>();
        features.AddRange(activeFeatures);
        foreach (var item in AllFeaturesDict)
        {
            if (features.Contains(item.Key))
            {
                item.Value.State = ActiveState.Selected;
            }
            else
            {
                item.Value.State = ActiveState.Enabled;
            }
        }
    }

    /// <summary>
    /// Used by features on initialization to add themselves to the AllFeaturesDict dictionary.
    /// </summary>
    /// <param name="feature"></param>
    public static void AddFeature(ShipFeature feature)
    {

        if (AllFeaturesDict == null)
            AllFeaturesDict = new Dictionary<string, ShipFeature>();

        if (!AllFeaturesDict.ContainsKey(feature.FeatureData.FeatureName))
        {
            AllFeaturesDict.Add(feature.FeatureData.FeatureName, feature);
        }
    }

    /// <summary>
    /// Used by features to remove themselves from the AllFeaturesDict dictionary, i.e when switching to a new ship, remove
    /// before deleting themselves.
    /// </summary>
    /// <param name="feature"></param>
    public static void RemoveFeature(ShipFeature feature)
    {
        Debug.Log("Feature removed: " + feature.name, feature);

        if (AllFeaturesDict == null)
            return;

        if (AllFeaturesDict.ContainsKey(feature.FeatureData.FeatureName))
        {
            AllFeaturesDict.Remove(feature.FeatureData.FeatureName);
        }
    }
}
