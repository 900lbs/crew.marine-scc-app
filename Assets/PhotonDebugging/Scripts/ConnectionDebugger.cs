using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDebugger : MonoBehaviour, IDebugger
{
    #region Private Properties

    private bool isMasterClient => PhotonNetwork.IsMasterClient;
    private bool isConnectedToMaster => PhotonNetwork.IsConnectedAndReady;
    private bool isConnectedToRoom => PhotonNetwork.InRoom;
    private string networkClientState => PhotonNetwork.NetworkClientState.ToString();
    private string ipAddress => PhotonNetwork.ServerAddress;
    private string region => PhotonNetwork.CloudRegion;
    private int ping => PhotonNetwork.GetPing();

    #endregion Private Properties

    #region Serialized Private References

    [Header("[Ping Custom Attributes]")]
    [SerializeField]
    private Color normalColor;

    [SerializeField]
    private int normalThreshold;

    [SerializeField]
    private Color warningColor;

    [SerializeField]
    private int warningThreshold;

    [SerializeField]
    private Color criticalColor;

    [SerializeField]
    private int criticalThreshold;

    [Space(5)]
    [Header("[References]")]
    [SerializeField]
    private Toggle masterClientToggle;

    [SerializeField]
    private Toggle connectedToMasterToggle;

    [SerializeField]
    private Toggle connectedToRoomToggle;

    [SerializeField]
    private Text pingText;

    [SerializeField]
    private Text ipText;

    [SerializeField]
    private Text regionText;

    [SerializeField]
    private Text networkClientStateText;

    #endregion Serialized Private References

    #region Private References

    private Image image;

    #endregion Private References

    #region Unity Callbacks

    private void OnEnable()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    #endregion Unity Callbacks

    #region Private Functions

    private void DeterminePing()
    {
        pingText.text = ping.ToString() + " ms";

        if (ping < normalThreshold)
            pingText.color = normalColor;
        if (ping > normalThreshold && ping < criticalThreshold)
            pingText.color = warningColor;
        else if (ping >= criticalThreshold)
            pingText.color = criticalColor;
    }

    #endregion Private Functions

    #region Interface Functions

    /// <summary>
    /// Updates all objects pertaining to connection status.
    /// </summary>
    public void UpdateDebug()
    {
        masterClientToggle.isOn = isMasterClient;
        connectedToMasterToggle.isOn = isConnectedToMaster;
        connectedToRoomToggle.isOn = isConnectedToRoom;

        ipText.text = ipAddress;
        regionText.text = (string.IsNullOrEmpty(region)) ? "NONE" : region.ToUpper();
        networkClientStateText.text = networkClientState;

        DeterminePing();
    }

    public void ChangeOpacity(float val)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, val);
    }

    #endregion Interface Functions
}