using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using AssetBundleBrowser.AssetBundleDataSource;
public class AssetBundleBuilder : EditorWindow
{
    int choiceIndex = 0;
    string[] choices;

    ShipID shipID;
    [MenuItem("RCCL/Asset Bundle Builder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssetBundleBuilder));
    }

    void OnGUI()
    {
        choices = GetNames();

        choiceIndex = EditorGUILayout.Popup(choiceIndex, choices);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Single Bundles");
        if (GUILayout.Button("Build AssetBundle"))
        {
            BuildSpecificBundle(choices, choiceIndex);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Build Complete Ship");

        shipID = (ShipID)EditorGUILayout.EnumFlagsField(shipID);

        if (GUILayout.Button("Build Ship"))
        {
            BuildShipBundle(shipID);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        /*----------------------------------------------------------------------------------------------------------------------------*/

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("All Bundles");
        if (GUILayout.Button("Build All AssetBundles"))
        {
            if (EditorUtility.DisplayDialog("Warning", "This process may take a long time, are you sure?", "Proceed", "Cancel"))
                BuildAllAssetBundles();
        }

        EditorGUILayout.EndVertical();
    }

    static string[] GetNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        return names;
    }

    public bool BuildSpecificBundle(string[] choiceData, int index)
    {
        string path = Application.streamingAssetsPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/" + choiceData[index]);
            Debug.Log("Creating directory for " + choiceData[index] + " to " + path, this);
        }
        AssetBundleBuild[] build = new AssetBundleBuild[1];

        build[0].assetBundleName = choiceData[index];
        build[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(choiceData[index]);

        foreach (string item in build[0].assetNames)
        {
            Debug.Log("Found asset: " + item, this);
        }

        Debug.Log("Building now.", this);
        var buildManifest = BuildPipeline.BuildAssetBundles(path, build, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
        if (buildManifest == null)
        {
            Debug.Log("Error in build");
            return false;
        }
        Debug.Log("AssetBundle " + choiceData[index] + " was successful.", buildManifest);
        return true;
    }

    public bool BuildShipBundle(ShipID ship)
    {
        string path = Application.streamingAssetsPath;

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        List<string> shipBundles = new List<string>();
        string[] allBundles = GetNames();

        foreach (string item in allBundles)
        {
            //Debug.Log("Checking bundle: " + item, this);
            if (item.Contains(ship.ToString().ToLower()))
            {
                //Debug.Log("Huzzah, found a ship subcategory " + item, this);
                shipBundles.Add(item);
            }
        }

        foreach (var item in shipBundles)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = item;
            build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(item);

            for (int i = 0; i < build.assetNames.Length; ++i)
            {
                Debug.Log("Found asset: " + build.assetNames[i], this);
            }
            Debug.Log("Adding new build: " + build.assetBundleName, this);
            builds.Add(build);

        }

        Debug.Log("Building now.", this);
        var buildManifest = BuildPipeline.BuildAssetBundles(path, builds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        AssetDatabase.Refresh();
        if (buildManifest == null)
        {
            Debug.Log("Error in build");
            return false;
        }
        Debug.Log("Assetbundle builds for ship '" + shipID.ToString() + "' were successful.", buildManifest);
        return true;
    }

    public bool BuildAllAssetBundles()
    {
        string bundlePath = Application.streamingAssetsPath;

        ABBuildInfo info = new ABBuildInfo();
        info.outputDirectory = bundlePath;
        info.options = BuildAssetBundleOptions.ChunkBasedCompression;
        info.buildTarget = BuildTarget.StandaloneWindows64;
        var buildManifest = BuildPipeline.BuildAssetBundles(info.outputDirectory, info.options, info.buildTarget);
        AssetDatabase.Refresh();
        if (buildManifest == null)
        {
            Debug.Log("Error in build");
            return false;
        }

        foreach (var assetBundleName in buildManifest.GetAllAssetBundles())
        {
            if (info.onBuild != null)
            {
                info.onBuild(assetBundleName);
            }
        }
        return true;
    }

}
