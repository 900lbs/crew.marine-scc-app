using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataIO
{
    #region Constants

    public const string PHOTON = "PhotonSettings";

    #endregion Constants

    #region Static Variables

    public static string PHOTON_PATH = Path.Combine(Application.streamingAssetsPath, PHOTON);

    #endregion Static Variables

    #region Public Static Functions

    /// <summary>
    /// Gets the photon settings from the Streaming Assets file.
    /// </summary>
    /// <returns>A PhotonSettings.</returns>
    public static PhotonSettings GetPhotonSettings()
    {
        PhotonSettings photonSettings = new PhotonSettings();
        ReadConfig(out photonSettings);
        return photonSettings;
    }

    /// <summary>
    /// Creates a JSON file in Streaming Assets based on given parameters
    /// </summary>
    /// <param name="photonSettings"></param>
    public static void CreatePhotonSettings(PhotonSettings photonSettings = null)
    {
        if (photonSettings == null)
            photonSettings = new PhotonSettings();

        Debug.Log($"Saving {photonSettings.ServerSettings} to {PHOTON_PATH}");

        ServerSettings serverSettings = photonSettings.ServerSettings;
        string currentConfigJSON = JsonConvert.SerializeObject(serverSettings, Formatting.Indented);
        File.WriteAllText(PHOTON_PATH, currentConfigJSON);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    #endregion Public Static Functions

    #region Private Static Functions

    /// <summary>
    /// Reads the JSON file with the current Photon Settings.
    /// </summary>
    /// <param name="photonSettings"></param>
    /// <returns></returns>
    private static bool ReadConfig(out PhotonSettings photonSettings)
    {
        // Try to load resource that exists in the project.
        if (File.Exists(PHOTON_PATH))
        {
            photonSettings = JsonConvert.DeserializeObject<PhotonSettings>(File.ReadAllText(PHOTON_PATH));
            Debug.Log($"Found {photonSettings} at {PHOTON_PATH}");
            return true;
        }
        else
        {
            photonSettings = new PhotonSettings();
            return false;
        }
    }

    #endregion Private Static Functions
}