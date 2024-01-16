using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ShipFeatureVariable))]
public class ShipFeatureEditor : Editor
{
    int index;
    string[] allBundles;

    private void OnEnable()
    {
        allBundles = AssetDatabase.GetAllAssetBundleNames();
    }

    public override void OnInspectorGUI()
    {
        ShipFeatureVariable data = (ShipFeatureVariable)target;
        DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Assign AssetBundle");

        if (GUILayout.Button("Assign AssetBundle"))
        {
            AssignAssetBundle(data);
        }
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
    
    internal void AssignAssetBundle(ShipFeatureVariable data)
    {
        if (data.data == null)
        {
            Debug.LogError("Feature name was empty.");
            return;
        }
        string bundleName = data.data.Ship.ToString().ToLower() + CONSTANTS.bundleDelimeter + "features" + CONSTANTS.bundleDelimeter + data.data.FeatureName;
        string assetPath = AssetDatabase.GetAssetPath(data);
        data.data.AssetBundleName = bundleName;
        AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, "");
    }
}
