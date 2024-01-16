using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSettings
{
    /// <summary>
    /// The default Cloud setup
    /// </summary>
    public PhotonSettings()
    {
        this.ServerSettings = ScriptableObject.CreateInstance<ServerSettings>();
        this.ServerSettings.AppSettings = new Photon.Realtime.AppSettings();
        this.ServerSettings.AppSettings.AppIdRealtime = "80e6144f-1a01-4d7a-8048-4d68789cd59d";
        this.ServerSettings.AppSettings.AppVersion = "2.5";
        this.ServerSettings.AppSettings.UseNameServer = true;
        this.ServerSettings.AppSettings.Port = 0;
        this.ServerSettings.AppSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
        this.ServerSettings.StartInOfflineMode = false;
        this.ServerSettings.RunInBackground = true;
#if UNITY_EDITOR
        this.ServerSettings.DisableAutoOpenWizard = true;
#endif
    }

    public ServerSettings ServerSettings;
}