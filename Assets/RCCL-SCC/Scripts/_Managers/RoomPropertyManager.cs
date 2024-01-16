using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Photon.Pun;

public class RoomPropertyManager
{
    /*
    readonly INetworkClient networkClient;
    readonly BuildSaveGAEdits saveGAEdits;

    public RoomPropertyManager(INetworkClient netClient,
    BuildSaveGAEdits gaEdits)
    {
        networkClient = netClient;
        saveGAEdits = gaEdits;
    }

     public async Task RoomInitialize()
    {
        
        if (!PhotonNetwork.IsMasterClient)
        {
            Dictionary<string, string> GAOverlayDict = new Dictionary<string, string>();

            GAOverlayDict = NetworkClientManager.GetRoomProperty(ShipRoomProperties.GAOverlay);

            foreach (var item in GAOverlayDict)
            {
                Debug.Log("GAOverlay found, Key: " + item.Key + ":" + item.Key.GetType() + " / Value Length: " + item.Value.Length);
                //annotationManager.buildSaveGAEdits.LoadPrefabs(item.Value as string);
                networkClient.OnNewGAOverlayEventReceived?.Invoke(item.Value);
            }


            Dictionary<string, string> AnnotationsDict = new Dictionary<string, string>();
            AnnotationsDict = NetworkClientManager.GetRoomProperty(ShipRoomProperties.Annotations);

            if (AnnotationsDict != null)
            {
                Debug.Log("Found annotations dictionary.");

                List<ShipNetworkObject> cacheList = new List<ShipNetworkObject>();

                foreach (var item in AnnotationsDict)
                {
                    //Debug.Log("Annotations found, Key: " + item.Key + ":" + item.Key.GetType() + " / Value Length: " + item.Value.Length);
                    await new WaitForBackgroundThread();
                    short s;
                    if (short.TryParse(item.Key, out s))
                    {
                        s = short.Parse(item.Key);
                        string[] splitData = AnnotationUtilities.GetAnnotationArray(item.Value);

                        NetworkStorageType storage;
                        try
                        {
                            Enum.TryParse(splitData[0], out storage);
                        }
                        catch (System.Exception)
                        {

                            throw;
                        }

                        ShipNetworkObject netObject = new ShipNetworkObject(NetworkEvent.Create, ShipRoomProperties.Annotations, storage, PhotonNetwork.NickName, s, item.Value, false);

                        if (OnNewNetworkEventReceived == null)
                        {
                            Debug.LogError("No network event listeners found.");
                        }

                        await new WaitForUpdate();
                        OnNewNetworkEventReceived?.Invoke(netObject);

                    }
                    else
                    {
                        Debug.LogError(item.Key.ToString() + " is not a valid key for Annotations room property.");
                    }
                }

            }
            else
            {
                Debug.Log("Annotations property not found.");
            }
        }
        
    } */
}
