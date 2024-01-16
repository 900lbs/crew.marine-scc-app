using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace NineHundredLBS.Photon
{
    public class PhotonDebugger : MonoBehaviour
    {
        #region Properties

        private bool isEnabled;

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                ToggleDebugger(value);

                if (isEnabled != value)
                {
                    isEnabled = value;

                    Debug.LogFormat("Photon Debugging is <color=orange>{0}</color>", isEnabled);
                }
            }
        }

        private bool isNetworkStatisticsEnabled;

        public bool IsNetworkStatisticsEnabled
        {
            get => PhotonNetwork.NetworkStatisticsEnabled;
            set
            {
                if (isNetworkStatisticsEnabled != value)
                    isNetworkStatisticsEnabled = value;
            }
        }

        #endregion Properties

        #region Public Variables

        [Header("Settings")]
        public bool EnabledOnStart = true;

        #endregion Public Variables

        #region Private Variables

        private float tickTimer;

        #endregion Private Variables

        #region Serialized Private Variables

        [Tooltip("The first key that you have to hold down.")]
        [SerializeField]
        private KeyCode key;

        [Tooltip("The second key that you have to press.")]
        [SerializeField]
        private KeyCode keyDown;

        [SerializeField]
        private float TickSpeed = 0.1f;

        #endregion Serialized Private Variables

        #region Serialized Private References

        [SerializeField]
        private GameObject Modules;

        [SerializeField]
        private ConnectionDebugger connectionDebugger;

        [SerializeField]
        private RoomDebugger roomDebugger;

        [SerializeField]
        private MessageDebugger messageDebugger;

        [SerializeField]
        private PropertiesDebugger propertiesDebugger;

        #endregion Serialized Private References

        #region Public Functions

        public void OnOpacityChanged(float val)
        {
            connectionDebugger.ChangeOpacity(val);
            roomDebugger.ChangeOpacity(val);
            messageDebugger.ChangeOpacity(val);
        }

        #endregion Public Functions

        #region Private Functions

        private void ToggleDebugger(bool enabled)
        {
            if (enabled)
            {
                //If our debugger is enabled, enable all Photon debugging as well.
                PhotonNetwork.PhotonServerSettings.PunLogging = PunLogLevel.Full;
                PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging = ExitGames.Client.Photon.DebugLevel.ALL;
                PhotonNetwork.PhotonServerSettings.EnableSupportLogger = true;

                PhotonNetwork.NetworkStatisticsEnabled = true; // This will run regardless of these scripts, make sure it turns off/on

                if (connectionDebugger == null)
                    connectionDebugger = GetComponentInChildren<ConnectionDebugger>();
                if (roomDebugger == null)
                    roomDebugger = GetComponentInChildren<RoomDebugger>();
                if (messageDebugger == null)
                    messageDebugger = GetComponentInChildren<MessageDebugger>();

                // After all Photon Debug features have been enabled, turn the modules on
                Modules.SetActive(true);
            }
            else
            {
                // Set every debug feature back to error-only, much less overhead.
                PhotonNetwork.PhotonServerSettings.PunLogging = PunLogLevel.ErrorsOnly;
                PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging = ExitGames.Client.Photon.DebugLevel.ERROR;
                PhotonNetwork.PhotonServerSettings.EnableSupportLogger = false;

                PhotonNetwork.NetworkStatisticsEnabled = false;

                // After all Photon debug features are set back to production, turn the modules off completely.
                Modules.SetActive(false);
            }
        }

        #endregion Private Functions

        #region Unity Callbacks

        private void OnEnable()
        { }

        private void OnDisable()
        {
            ToggleDebugger(false);
        }

        private void Start()
        {
            IsEnabled = EnabledOnStart;
        }

        private void Update()
        {
            if (Input.GetKey(key) && Input.GetKeyDown(keyDown))
            {
                IsEnabled = !IsEnabled;
            }

            if (IsEnabled)
            {
                tickTimer += Time.deltaTime;

                if (tickTimer > TickSpeed)
                {
                    connectionDebugger?.UpdateDebug();
                    roomDebugger?.UpdateDebug();
                    messageDebugger?.UpdateDebug();
                    propertiesDebugger?.UpdateDebug();
                    tickTimer = 0;
                }
            }
        }

        private void FixedUpdate()
        {
        }

        #endregion Unity Callbacks
    }
}