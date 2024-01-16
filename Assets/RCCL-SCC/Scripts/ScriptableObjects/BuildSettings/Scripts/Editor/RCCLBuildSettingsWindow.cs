using System;
using System.Collections.Generic;
using PrimitiveFactory.ScriptableObjectSuite;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

using Zenject;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;
using System.IO;

/// <summary>
/// The RCCLBuildSettings editor script that builds according to the selected settings.
/// </summary>

public class RCCLBuildSettingsWindow : ScriptableObjectEditorWindow<RCCLBuildSettings>
{
    // Name of the object (Display purposes)
    protected override string c_ObjectName
    { get { return "RCCLBuildSettings"; } }

    // Relative path from Project Root
    protected override string c_ObjectFullPath
    { get { return "Assets/Scripts/Scriptable Objects/BuildSettings/Data"; } }

    protected Object focusedObject;

    protected Photon.Pun.ServerSettings Server;

    private ShipVariable CurrentShip;

    private static ProjectSettings tempProjectSettings;

    [MenuItem("RCCL/BuildSettings")]
    public static void ShowWindow()
    {
        RCCLBuildSettingsWindow window = GetWindow<RCCLBuildSettingsWindow>("RCCLBuildSettings Editor");
        window.Show();
    }

    protected override void OnSelectionChanged(RCCLBuildSettings oldObj, RCCLBuildSettings newObj)
    {
        base.OnSelectionChanged(oldObj, newObj);

        focusedObject = newObj;
    }

    protected override void DrawCustomFunctions()
    {
        if (focusedObject != null)
        {
            DrawCustomFunction("Build (" + focusedObject.name + ")", BuildFocused);
            DrawCustomFunction("Setup (" + focusedObject.name + ") Settings Only.", SetupFocused);
        }
    }

    private void BuildFocused()
    {
        try
        {
            RCCLBuildSettings buildObject = (RCCLBuildSettings)focusedObject;
            BuildSequence(buildObject, false);
        }
        catch
        {
            if (focusedObject == null)
            {
                Debug.LogError("Build object not selected.");
            }
            throw;
        }
    }

    private void SetupSettingsOnly(RCCLBuildSettings settings)
    {
        BuildSequence(settings, true);
    }

    private void BuildSequence(RCCLBuildSettings buildSettings, bool isDryRun)
    {
        Object[] ships = Object.FindObjectsOfType(typeof(ShipVariable));

        foreach (var ship in (ShipVariable[])ships)
        {
            if (ship.name.Contains("CurrentShip") && !isDryRun)
            {
                CurrentShip = ship;
                CurrentShip.ResetShip();
            }
        }

        tempProjectSettings = buildSettings.ProjectSettings;

        List<ScriptableObjectInstaller> installers = new List<ScriptableObjectInstaller>();
        installers.Add(buildSettings.SceneSettings);
        buildSettings.ProjectContext.ScriptableObjectInstallers = installers;
        buildSettings.ProjectContext.GetComponent<ProjectInstaller>().CurrentProjectSettings = buildSettings.ProjectSettings;

        string buildNameSuffix = "";

        // Append a suffix using the target ship abbreviation code, empty otherwise
        buildNameSuffix = (buildSettings.ProjectSettings.TargetShip == null) ? "" : ("-" + buildSettings.ProjectSettings.TargetShip.Ship.ShipAbbreviation);

        PlayerSettings.productName = string.Concat(buildSettings.BuildName, buildNameSuffix);
        PlayerSettings.bundleVersion = buildSettings.BuildVersion;

        QualitySettings.antiAliasing = buildSettings.ProjectSettings.AntiAliasing;

        Server = Resources.Load<Photon.Pun.ServerSettings>("PhotonServerSettings");
        Server.AppSettings.AppVersion = buildSettings.PhotonVersion;

        // Check if we have a target ship, update settings accordingly
        // TODO: If Multiship ever hits production, make sure these assignments are made upon selecting a ship.
        if (buildSettings.ProjectSettings.TargetShip != null)
        {
            buildSettings.SceneSettings.Settings.XmlBase.eVar139 = buildSettings.ProjectSettings.TargetShip.Ship.ShipAbbreviation;

            if (!buildSettings.ProjectSettings.MultishipEnabled)
            {
                RCCLBuildSettings.TargetShip = buildSettings.ProjectSettings.TargetShip.Ship.ID.ToString();
                Debug.Log("Target Ship selected: " + RCCLBuildSettings.TargetShip);

                AssetBundleEditorUtilities.AssignTargetShip(buildSettings.ProjectSettings.TargetShip.Ship.ID);
            }
            else
            {
                RCCLBuildSettings.TargetShip = "";
            }
        }

        DataIO.CreatePhotonSettings();

        // Build options
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.locationPathName = string.Concat("Builds/", buildSettings.BuildVersion, "/", buildSettings.BuildName, "/", buildSettings.BuildName, buildNameSuffix, ".exe");
        options.target = BuildTarget.StandaloneWindows64;
        options.scenes = new[] { AssetDatabase.GetAssetPath(buildSettings.MainMenu), AssetDatabase.GetAssetPath(buildSettings.Application) };
        options.options = (buildSettings.ProjectSettings.DebugEnabled == true) ? (BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.ConnectWithProfiler) : BuildOptions.None;

        if (isDryRun)
            return;

        string killCardLocation = "Builds/" + buildSettings.BuildVersion + "/" + buildSettings.BuildName + "/" + buildSettings.BuildName + "_Data/";
        BuildKillcards(RCCLBuildSettings.TargetShip.ToString(), killCardLocation);

        // Begin the actual build
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes at location: " + summary.outputPath);

            //DeleteUnusedAssets(settings, summary);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed");
        }
    }

    /// <summary>
    /// Post-build function that finds and deletes unnecessary StreamingAssets.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="pathToBuiltProject">Path to the built project.</param>
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (tempProjectSettings.MultishipEnabled)
            return;

        string streamingAssetsPath = null;
        string root = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), Path.GetFileNameWithoutExtension(pathToBuiltProject) + "_Data");
        string[] shipNames = Enum.GetNames(typeof(ShipID));

        streamingAssetsPath = Path.Combine(root, "StreamingAssets/");

        for (int i = 0; i < shipNames.Length; i++)
        {
            if (shipNames[i] != RCCLBuildSettings.TargetShip)
            {
                string combinedPath = streamingAssetsPath + shipNames[i].ToLower();
                if (Directory.Exists(combinedPath))
                {
                    Debug.Log("Deleting streaming asset folder: " + combinedPath);
                    Directory.Delete(combinedPath, true);
                }
                else
                {
                    Debug.Log("Streaming asset path not found: " + combinedPath);
                }
            }
        }
    }

    private void SetupFocused()
    {
        try
        {
            RCCLBuildSettings buildObject = (RCCLBuildSettings)focusedObject;
            SetupSettingsOnly(buildObject);
        }
        catch
        {
            if (focusedObject == null)
            {
                Debug.LogError("Build object not selected.");
            }
            throw;
        }
    }

    private void BuildKillcards(string ship, string buildPath)
    {
        try
        {
            string killCardPath = "D:/_Github/RCCL - SCC/KillCards/" + ship + "/KillCard_Photos";
            if (File.Exists(killCardPath))
            {
                Debug.Log("Killcards found, copying to new build now.");
                File.Copy(killCardPath, buildPath);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Could not copy Killcards to new build: " + e.Message);
            throw;
        }
    }
}