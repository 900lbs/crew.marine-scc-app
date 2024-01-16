using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipVariable))]
public class ShipVariableEditor : Editor
{
    SerializedProperty features;
    string[] bundles;
    void OnEnable()
    {
        bundles = AssetDatabase.GetAllAssetBundleNames();

        features = serializedObject.FindProperty("Ship").FindPropertyRelative("Features");
    }

    public override void OnInspectorGUI()
    {
        ShipVariable data = (ShipVariable)serializedObject.targetObject;

        serializedObject.Update();

        DrawDefaultInspector();

        List<string> shipBundles = new List<string>();

        if (GUILayout.Button("Load Data"))
        {
            features.ClearArray();
            foreach (string bundle in bundles)
            {
                if (bundle.Contains(data.Ship.ID.ToString().ToLower()) && bundle.Contains("feature"))
                {
                    shipBundles.Add(bundle);
                }
            }

            for (int i = 0; i < shipBundles.Count; i++)
            {
                 features.InsertArrayElementAtIndex(i);
                 features.GetArrayElementAtIndex(i).stringValue = shipBundles[i];
            }
        }



        serializedObject.ApplyModifiedProperties();
    }

    /*         if (GUILayout.Button("Create Feature Scriptable Object Instances"))
            {
                CreateScriptableObject(data);
            } */


    /*     internal void CreateScriptableObject(ShipVariable data)
        {
            string filePath = "features/" + data.Ship.ID.ToString().ToLower();

            foreach (ShipFeatureData item in data.Ship.Features)
            {
                ShipFeatureVariable feature = ScriptableObjectUtility.CreateAsset<ShipFeatureVariable>(filePath, item.FeatureName);
                feature.data = item;
                feature.data.Ship = data.Ship.ID;

                string assetPath = AssetDatabase.GetAssetPath(feature);
                string bundleName = feature.data.Ship.ToString().ToLower() + "/features/" + feature.data.FeatureName.ToLower();
                AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, "");    
            }
        }
    } */
}
public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string folderPath, string name) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string filePath = "Assets/Scripts/Scriptable Objects/Ships/Data/" + folderPath;

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(filePath + "/" + name + ".asset");
        Debug.Log("Unique asset name and path: " + assetPathAndName);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }
}
