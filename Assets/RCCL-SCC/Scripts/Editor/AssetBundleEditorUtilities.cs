using System.Security.Cryptography;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class AssetBundleEditorUtilities
{
    private static ShipVariable[] ships;
    private static ShipFeatureVariable[] features;

    private static bool wasCancelled = false;
    private static bool continueAllPersistent = false;
    private static bool continueAllLegends = false;
    private static bool continueAllFeatures = false;

    /*----------------------------------------------------------------------------------------------------------------------------*/

    [MenuItem("AssetBundleUtilities/AssignAllLabels")]
    public static void AssignAssetLabelsToShip()
    {
        ships = GetAllInstances<ShipVariable>();

        if (!wasCancelled)
        {
            Debug.Log("Found " + ships.Length + " ships.");
            foreach (ShipVariable ship in ships)
            {
                if (ship.Ship.ID != ShipID.None)
                {
                    Debug.Log("<color=green>" + AssignPersistentTextures(ship.Ship.ID, ship.Ship.Decks, ship.Ship.Cabins) + " persistent assets were assigned.</color>");

                    Debug.Log("<color=green>" + AssignLegendsTextures(ship.Ship.ID, ship.Ship.Legends) + " persistent assets were assigned.</color>");

                    //Debug.Log("<color=green>" + AssignAllFeaturesTextures(ship.Ship.ID, ship.Ship.Features) + " persistent assets were assigned.</color>");
                }
            }
        }

        wasCancelled = false;
        continueAllPersistent = false;
        continueAllLegends = false;
        continueAllFeatures = false;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    [MenuItem("AssetBundleUtilities/AssignPersistentLabels")]
    public static void AssignPersistentAssetLabelsToShip()
    {
        ships = GetAllInstances<ShipVariable>();
        List<string> shipLocations = new List<string>();

        if (!wasCancelled)
        {
            Debug.Log("Found " + ships.Length + " ships.");
            foreach (ShipVariable ship in ships)
            {
                if (ship.Ship.ID != ShipID.None)
                {
                    Debug.Log("<color=green>" + AssignPersistentTextures(ship.Ship.ID, ship.Ship.Decks, ship.Ship.Cabins) + " persistent assets were assigned.</color>");
                }
            }
        }
        else
        {
            Debug.Log("Persistent assignment operation cancelled.");
        }
        wasCancelled = false;
        continueAllPersistent = false;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    [MenuItem("AssetBundleUtilities/AssignLegendLabels")]
    public static void AssignLegendAssetLabelsToShip()
    {
        ships = GetAllInstances<ShipVariable>();
        List<string> shipLocations = new List<string>();

        if (!wasCancelled)
        {
            Debug.Log("Found " + ships.Length + " ships.");
            foreach (ShipVariable ship in ships)
            {
                if (ship.Ship.ID != ShipID.None)
                {
                    Debug.Log("<color=green>" + AssignLegendsTextures(ship.Ship.ID, ship.Ship.Legends) + " legend assets were assigned.</color>");
                }
            }
        }
        else
        {
            Debug.Log("Legend assignment operation cancelled.");
        }

        wasCancelled = false;
        continueAllLegends = false;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    [MenuItem("AssetBundleUtilities/AssignFeatureLabels")]
    public static void AssignFeatureAssetLabelsToShip()
    {
        features = GetAllInstances<ShipFeatureVariable>();

        if (!wasCancelled)
        {
            foreach (ShipFeatureVariable feature in features)
            {
                if (feature.data == null)
                {
                    Debug.LogError("Feature name was empty.");
                    return;
                }
                string formattedBundle = (feature.data.FeatureName.ToLower());
                string bundleName = feature.data.Ship.ToString().ToLower() + CONSTANTS.bundleDelimeter + "features" + CONSTANTS.bundleDelimeter + formattedBundle;
                string dataPath = AssetDatabase.GetAssetPath(feature);
                feature.data.AssetBundleName = bundleName;
                AssetImporter.GetAtPath(dataPath).SetAssetBundleNameAndVariant(bundleName, "");
            }
        }
        else
        {
            Debug.Log("Feature assignment operation cancelled.");
        }

        wasCancelled = false;
        continueAllFeatures = false;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Assigns all of the textures that will persist in the scene no matter what. Since these textures are
    /// apart of an 8k atlas, we only need to set them once.
    /// </summary>
    /// <returns></returns>
    private static int AssignPersistentTextures(ShipID id, params object[] textures)
    {
        if (textures == null || wasCancelled)
            return 0;

        int completedAssignment = 0; //Simply used to keep track and display how many assignments were done.

        foreach (var textureArray in textures)
        {
            if (textureArray.GetType() == typeof(Sprite[]))
            {
                List<Sprite> textureSet = new List<Sprite>();
                textureSet.AddRange((Sprite[])textureArray); // Create a casted list reference.

                string previousPath = "";

                for (int i = 0; i < textureSet.Count; i++) // Even though some textures share the same atlas, we don't want to try and figure out which one they belong to, simple previous path check is all we need.
                {
                    string bundleName = id.ToString().ToLower() + "/" + "persistent";
                    string path = AssetDatabase.GetAssetPath(textureSet[i]);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                    if (assetImporter != null)
                    {
                        if (assetImporter.assetBundleName != bundleName) // Unnecessary?
                        {
                            assetImporter.SetAssetBundleNameAndVariant(bundleName, null); // Assign bundle name
                            completedAssignment++;
                            assetImporter.SaveAndReimport(); // Since we're changing the asset dirty, we have to save and reimport it via it's asset importer.
                        }
                        previousPath = path;
                    }
                    else
                    {
                        Debug.LogError("Path not valid: " + path);
                        previousPath = path;
                        continue;
                    }

                    if (continueAllPersistent == false)
                    {
                        int option = EditorUtility.DisplayDialogComplex(id.ToString().ToLower(), textureSet[i].name + " was completed, do you wish to continue?", "Next", "Cancel", "Continue All");

                        switch (option)
                        {
                            case 0:
                                continue;
                            case 1:
                                wasCancelled = true;
                                return completedAssignment;

                            case 2:
                                continueAllPersistent = true;
                                continue;
                            default:
                                Debug.LogError("Unrecognized option.");
                                break;
                        }
                    }
                }
            }
        }
        return completedAssignment;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Run through each legend texture and assign.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static int AssignLegendsTextures(ShipID id, LegendFeatureData[] data)
    {
        if (data == null || wasCancelled)
            return 0;

        int completedAssignment = 0; //Simply used to keep track and display how many assignments were done.

        foreach (LegendFeatureData legend in data)
        {
            string bundleName = id.ToString().ToLower() + "/" + "legend";
            //Debug.Log("Assigning Legend: " + legend.LegendName + " for ship: " + id.ToString(), legend.LegendImage);

            string path = AssetDatabase.GetAssetPath(legend.LegendImage);
            if (path == null)
                Debug.LogError("Could not find AssetPath " + legend.LegendName);

            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("<color=red> AssetImporter could not find an asset at path: " + path + "</color>");
                continue;
            }

            if (assetImporter != null)
            {
                if (assetImporter.assetBundleName != bundleName)
                {
                    Debug.Log(assetImporter.assetBundleName + " is being set to " + bundleName);
                    assetImporter.SetAssetBundleNameAndVariant(bundleName, null);
                    completedAssignment++;
                    assetImporter.SaveAndReimport();
                }
            }
            else
            {
                Debug.LogError("Path not valid: " + path);
                continue;
            }

            assetImporter = null;
            if (continueAllLegends == false)
            {
                int option = EditorUtility.DisplayDialogComplex(id.ToString().ToLower(), legend.LegendImage.name + " was completed, do you wish to continue?", "Next", "Cancel", "Continue All");

                switch (option)
                {
                    case 0:
                        continue;
                    case 1:
                        wasCancelled = true;
                        return completedAssignment;

                    case 2:
                        continueAllLegends = true;
                        continue;
                    default:
                        Debug.LogError("Unrecognized option.");
                        break;
                }
            }
        }
        return completedAssignment;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Goes through each feature and assigns it's assetbundle name according to ship name and feature name.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static int AssignAllFeaturesTextures(ShipID id, params ShipFeatureData[] data)
    {
        if (data == null || wasCancelled)
            return 0;

        int completedAssignment = 0; //Simply used to keep track and display how many assignments were done.

        foreach (ShipFeatureData featureData in data)
        {
            foreach (Sprite image in featureData.Overlays)
            {
                //Debug.Log("Assigning " + featureData.FeatureName, image);
                string bundleName = id.ToString().ToLower() + "/" + featureData.FeatureName.ToLower();
                string path = AssetDatabase.GetAssetPath(image);

                AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                if (assetImporter != null)
                {
                    if (assetImporter?.assetBundleName != bundleName || assetImporter.assetBundleName == null)
                    {
                        assetImporter.SetAssetBundleNameAndVariant(bundleName, null);
                        completedAssignment++;
                        assetImporter.SaveAndReimport();
                    }
                }
                else
                {
                    Debug.LogError("Path not valid: " + path);
                    continue;
                }

                if (continueAllFeatures == false)
                {
                    int option = EditorUtility.DisplayDialogComplex(id.ToString().ToLower(), image.name + " was completed, do you wish to continue?", "Next", "Cancel", "Continue All");

                    switch (option)
                    {
                        case 0:
                            continue;
                        case 1:
                            wasCancelled = true;
                            return completedAssignment;

                        case 2:
                            continueAllFeatures = true;
                            continue;
                        default:
                            Debug.LogError("Unrecognized option.");
                            break;
                    }
                }
            }
        }
        return completedAssignment;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }

    public static void AssignTargetShip(ShipID shipID)
    {
        ships = GetAllInstances<ShipVariable>();

        foreach (var ship in ships)
        {
            if (ship.name == "CurrentShip")
            {
                ship.Ship.ID = shipID;

                string currentShipPath = Application.streamingAssetsPath;

                AssetBundleBuild[] build = new AssetBundleBuild[1];

                build[0].assetBundleName = "currentship";
                build[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(currentShipPath);

                var buildManifest = BuildPipeline.BuildAssetBundles(currentShipPath, build, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
                AssetDatabase.Refresh();
                if (buildManifest == null)
                {
                    Debug.Log("Error in build");
                }

                Debug.Log("AssetBundle " + currentShipPath + " was successful.", buildManifest);
            }
        }
    }
}