using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkRoomPropertyHandler
{
    private static CancellationTokenSource c;

    [Inject]
    public NetworkRoomPropertyHandler()
    {
        c = new CancellationTokenSource();
        //c = cts;
    }

    private static Dictionary<string, string> prop;

    private CancellationTokenSource localC;

    public static async Task SpawnAllProperties(NetworkClient networkClient)
    {
        Debug.Log("<color=cyan>Spawning all properties.</color>");
        prop = new Dictionary<string, string>();

        if (!c.IsCancellationRequested)
        {
            try
            {
                await new WaitForBackgroundThread();
                Hashtable props = NetworkClientManager.GetRoomProperties();
                List<NetworkAnnotationObject> objectsToSend = new List<NetworkAnnotationObject>();

                lock (props)
                {
                    foreach (var item in props)
                    {
                        lock (item.Value)
                        {
                            ShipRoomProperties key = 0;
                            if (!Enum.TryParse(item.Key.ToString(), out key))
                            {
                                /* if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                                    Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                                    Debug.Log(item.Key.ToString() + " was not a valid Ship Room Property."); */
                                continue;
                            }

                            switch (key)
                            {
                                case ShipRoomProperties.Annotations:

                                    prop = (Dictionary<string, string>)item.Value;
                                    if (prop != null && prop.Count > 0)
                                    {
                                        string[] annotations = new string[prop.Values.Count];
                                        prop.Values.CopyTo(annotations, 0);

                                        foreach (var anno in annotations)
                                        {
                                            if (anno != null)
                                            {
                                                string[] data = AnnotationUtilities.GetAnnotationArray(anno).Result;
                                                for (int i = 0; i < data.Length; i++)
                                                {
                                                    if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                                                        Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                                                        Debug.Log("Data: " + data[i]);
                                                }
                                                NetworkStorageType storageType = 0;

                                                if (!Enum.TryParse(data[0], true, out storageType))
                                                {
                                                    if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                                                       Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
                                                        Debug.Log("Storage type '" + data[0] + "' was not found for property: .");
                                                    continue;
                                                }
                                                else
                                                {
                                                    NetworkAnnotationObject annotationObject = new NetworkAnnotationObject(NetworkEvent.Create,
                                                        key,
                                                        storageType,
                                                        data[2],
                                                        short.Parse(data[3]),
                                                        anno,
                                                        true);

                                                    objectsToSend.Add(annotationObject);
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                await new WaitForUpdate();

                for (int i = 0; i < objectsToSend.Count; i++)
                {
                    await networkClient.SendNewNetworkEventReceived(objectsToSend[i]);
                    await new WaitForSecondsRealtime(0.25f);
                }
            }
            catch (System.Exception e)
            {
                await new WaitForUpdate();
                Debug.Log("Room property error: " + e.InnerException + " / " + e.StackTrace);
                MainMenu.ChangeGameState?.Invoke(GameState.Room);
                c.Cancel();
                throw;
            }
        }
        MainMenu.ChangeGameState?.Invoke(GameState.Room);
    }
}