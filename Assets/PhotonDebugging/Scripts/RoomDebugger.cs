using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomDebugger : MonoBehaviour, IDebugger
{
    #region Private Properties

    private string roomName => (PhotonNetwork.InRoom) ? PhotonNetwork.CurrentRoom.Name : "Not Joined";

    private string playerName => (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName))
        ? "Not Assigned" : PhotonNetwork.LocalPlayer.NickName;

    private int playerCount => (PhotonNetwork.InRoom) ? (int)PhotonNetwork.CurrentRoom.PlayerCount : 0;

    private int roomPropertyTotal =>
        (PhotonNetwork.InRoom) ? PhotonNetwork.CurrentRoom.CustomProperties.Count : 0;

    #endregion Private Properties

    #region Serialized Private References

    [Header("[References]")]

    [SerializeField]
    private Text roomNameText;

    [SerializeField]
    private Text playerNameText;

    [SerializeField]
    private Text playerCountText;

    [SerializeField]
    private Text roomPropertyTotalText;

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

    private void OnDisable()
    {
    }

    #endregion Unity Callbacks

    #region Interface Functions

    public void UpdateDebug()
    {
        roomNameText.text = roomName;
        playerNameText.text = playerName;
        playerCountText.text = playerCount.ToString();
        roomPropertyTotalText.text = roomPropertyTotal.ToString();
    }

    public void ChangeOpacity(float val)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, val);
    }

    #endregion Interface Functions
}