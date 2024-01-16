using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PropertiesDebugger : MonoBehaviour, IDebugger
{
    #region Private References

    private Image image;

    private Dictionary<string, GameObject> propertyObjects;

    #endregion Private References

    #region Serialized Private References

    [SerializeField]
    private GameObject propertyTextPrefab;

    [SerializeField]
    private RectTransform content;

    #endregion Serialized Private References

    #region Private Functions

    private void UpdateProperties()
    {
        if (!PhotonNetwork.InRoom)
            return;
        if (PhotonNetwork.CurrentRoom.CustomProperties == null)
            return;
        if (propertyObjects == null)
            propertyObjects = new Dictionary<string, GameObject>();

        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        lock (roomProperties)
        {
            foreach (var property in propertyObjects)
            {
                if (!roomProperties.ContainsKey(property.Key))
                {
                    var propertyStash = property;
                    Destroy(property.Value);

                    propertyObjects.Remove(propertyStash.Key);
                }
            }

            foreach (var property in roomProperties)
            {
                if (!propertyObjects.ContainsKey(property.Key.ToString()))
                {
                    GameObject spawnPrefab = Instantiate(propertyTextPrefab, content.transform);

                    spawnPrefab.GetComponent<Text>().text = CustomPropertyOutput(property.Value);
                    propertyObjects.Add(property.Key.ToString(), spawnPrefab);
                }
            }
        }
    }

    /// <summary>
    /// Used to customize the type of property being read.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string CustomPropertyOutput(object value)
    {
        try
        {
            return (string)value;
        }
        catch
        {
        }

        try
        {
            Dictionary<string, string> output = (Dictionary<string, string>)value;

            string combinedOutput = "";

            foreach (var item in output)
            {
                combinedOutput += " / " + item.Value;
            }

            return combinedOutput;
        }
        catch
        {
        }

        return "Custom Property Not Found.";
    }

    #endregion Private Functions

    #region Unity Callbacks

    private void OnEnable()
    {
        UpdateProperties();
    }

    #endregion Unity Callbacks

    public void ChangeOpacity(float val)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, val);
    }

    public void UpdateDebug()
    {
        UpdateProperties();
    }
}