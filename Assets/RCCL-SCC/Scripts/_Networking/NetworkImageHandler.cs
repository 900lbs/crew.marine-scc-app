using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using System.Threading.Tasks;

public class NetworkImageHandler : MonoBehaviourPun
{
    #region Properties
    public const string HEADER_IMAGE_UPDATE = "up_img";
    public const string HEADER_IMAGE_END = "end_img";

    /// <summary>
    /// Network : Image bytes lenght (by packet)
    /// </summary>
    public const int IMAGE_LENGTH = 1000;

    /// <summary>
    /// Network : Header bytes lenght (by packet)
    /// </summary>
    public const int HEADER_LENGTH = 402;

    Dictionary<string, byte[]> imageBytes;

    #endregion


/* 
    [PunRPC]
    public async Task RPCImageReceived(string name, byte[] imageData)
    {
        byte[] recBuffer = new byte[IMAGE_LENGTH + HEADER_LENGTH];
        //int bufferSize = IMAGE_LENGTH + HEADER_LENGTH;
        int dataSize = 0;
        byte[] header = new byte[];

        for (int i = 0; i < header.Length; i++)
        {
            header[i] = imageData[i];
        }

        string header_type = name.Split(';')[0]; // header
        string header_imageID = name.Split(';')[1]; // ID

        if (header_type == HEADER_IMAGE_UPDATE || header_type == HEADER_IMAGE_END)
        {
            byte[] newImageBytes = null;

            if (!imageBytes.TryGetValue(header_imageID, out newImageBytes))
            {
                newImageBytes = new byte[dataSize - HEADER_LENGTH];
                for (int i = 0; i < newImageBytes.Length; i++)
                {
                    newImageBytes[i] = recBuffer[i + HEADER_LENGTH];
                }
                //Debug.Log("newBytes " + finalImageBytes.Length + " + 0");

                imageBytes.Add(header_imageID, newImageBytes);
            }
            else
            {
                newImageBytes = new byte[imageBytes[header_imageID].Length + dataSize - HEADER_LENGTH];
                //Debug.Log("newBytes " + finalImageBytes.Length +" + "+ dataSize);

                for (int i = 0; i < imageBytes[header_imageID].Length; i++)
                {
                    newImageBytes[i] = imageBytes[header_imageID][i];
                }

                for (int i = imageBytes[header_imageID].Length; i < newImageBytes.Length; i++)
                {
                    newImageBytes[i] = recBuffer[i + HEADER_LENGTH - imageBytes[header_imageID].Length];
                }

                imageBytes[header_imageID] = newImageBytes;
            }

            if (header_type == HEADER_IMAGE_END)
            {
                //Debug.Log("bytes lenght actual : " + imageBytes[imageID].Length);

                ServerManager server = (ServerManager)GameObject.FindObjectOfType(typeof(ServerManager));
                if (server != null)
                    server.SaveNewImage(imageBytes[header_imageID]);
            }
        }
    } */
}
