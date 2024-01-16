using UnityEngine;
using PrimitiveFactory.ScriptableObjectSuite;

using Zenject;

public class RCCLBuildSettings : ScriptableObjectExtended
{
    [Header("Build Settings")]
    public string BuildName;

    public string BuildVersion;

    public string PhotonVersion;

    [Header("Scene Settings")]
    [Tooltip("The scene object itself that holds the project context for installers.")]
    public ProjectContext ProjectContext;

    [Tooltip("Settings for the scene that effect that specific RCCL scene.")]
    public SettingsInstaller SceneSettings;

    [Tooltip("Specific project settings that affect all scenes.")]
    public ProjectSettings ProjectSettings;

    [Header("Scenes")]
    public Object MainMenu;

    public Object Application;

    public static string TargetShip = "";
}