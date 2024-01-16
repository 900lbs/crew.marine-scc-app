using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
public class SendTextureOverNetwork : UI_Button
{
    [Inject]
    KillCardManager killCardManager;
    
    [HideInInspector]
    public KillCardClass ExampleKillCard;
    public Texture2D TextureToSend;
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();
        //SendTexture();
        SendExampleKillCard();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnStateChange()
    {
        throw new System.NotImplementedException();
    }

    internal async void SendExampleKillCard()
    {
        NetworkKillCardObject exampleNetworkKillCard = new NetworkKillCardObject();
        KillCardClass newKillCard = killCardManager.currentKillCard;
        exampleNetworkKillCard.NetEvent = NetworkEvent.Create;
        exampleNetworkKillCard.PlayerID = NetworkClient.GetUserName().ToString();
        exampleNetworkKillCard.NetData = await exampleNetworkKillCard.ConvertKillCardData(newKillCard);
        networkClient.SendNewNetworkEvent(exampleNetworkKillCard);
    }

    internal void SendTexture()
    {
        Texture2D texture = TextureToSend;
        
        byte[] textureBytes = texture.EncodeToPNG();

        NetworkTextureObject networkTextureObject = new NetworkTextureObject();
        networkTextureObject.NetEvent = NetworkEvent.Create;
        networkTextureObject.PlayerID = NetworkClient.GetUserName().ToString();
        networkTextureObject.NetData = textureBytes;

        networkClient.SendNewNetworkEvent(networkTextureObject);
        Debug.Log("Sending Texture.", this);
    }
}
