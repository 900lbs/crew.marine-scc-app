using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
[RequireComponent(typeof(RawImage))]
public class ReceiveTextureObject : MonoBehaviour, IShipNetworkEvents
{
    [Inject]
    NetworkClient networkClient;
    public RawImage ReceiveImageComponent;

    void Start()
    {
        if (ReceiveImageComponent == null)
            ReceiveImageComponent = GetComponent<RawImage>();

        networkClient.AddListener(this);
    }

    void OnDestroy()
    {
        networkClient.RemoveListener(this);
    }

    public void OnNetworkActionEvent(INetworkObject value)
    {
        Debug.Log("Event found, type: " + value.GetType(), this);
        if (value.GetType() == typeof(NetworkTextureObject))
        {
            Debug.Log("Network Texture Received.", this);
            NetworkTextureObject networkTexture = (NetworkTextureObject)value;

            byte[] textureArray = (byte[])networkTexture.NetData;
            Texture2D receivedImage = new Texture2D(2, 2);
            receivedImage.LoadImage(textureArray);

            ReceiveImageComponent.texture = receivedImage;
        }
    }
}
