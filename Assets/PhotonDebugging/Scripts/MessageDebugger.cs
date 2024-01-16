using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageDebugger : MonoBehaviour, IOnEventCallback, IDebugger
{
    #region Private Variables

    private int eventsReceived = 0;

    #endregion Private Variables

    #region Private Properties

    private string networkclientDebug => PhotonNetwork.NetworkStatisticsToString();

    #endregion Private Properties

    #region Serialized Private References

    [Header("[References]")]

    [SerializeField]
    private Text networkclientDebugText;

    [SerializeField]
    private Text eventDebugText;

    [SerializeField]
    private Text eventsReceivedText;

    #endregion Serialized Private References

    #region Private References

    private Image image;

    #endregion Private References

    #region Unity Functions

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

        if (image == null)
            image = GetComponent<Image>();
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion Unity Functions

    //private void

    #region Interface Functions

    public void UpdateDebug()
    {
        networkclientDebugText.text = networkclientDebug;
        eventsReceivedText.text = eventsReceived.ToString();
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200)
            return;

        Debug.Log("Event <color=orange>received</color>");
        eventDebugText.text = photonEvent.ToStringFull();
        eventsReceived++;
    }

    public void ChangeOpacity(float val)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, val);
    }

    #endregion Interface Functions
}