using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class NetworkKillCardObject : INetworkObject
{
    public CancellationTokenSource cts { get; set; }

    public NetworkEvent NetEvent { get; set; }
    public string PlayerID { get; set; }
    public object NetData { get; set; }

    public NetworkKillCardObject()
    {
        cts = new CancellationTokenSource();
    }
    public bool ConvertFromNetworkObject(object[] value)
    {
        try
        {
            object[] values = (object[]) value;

            NetEvent = (NetworkEvent) values[0];
            PlayerID = (string) values[1];
            NetData = (object[]) values[2];
            return true;
        }
        catch (System.Exception)
        {
            Debug.Log("Not a killcard object.");
            return false;
            throw;
        }
    }

    public async Task<object[]> ConvertToNetworkObject()
    {
        await new WaitForBackgroundThread();
        object[] messageArray = new object[]
        {
            NetEvent,
            PlayerID,
            NetData
        };

        await new WaitForUpdate();

        return messageArray;
    }
    public async Task<object[]> ConvertKillCardData(KillCardClass data)
    {
        try
        {
            if (!cts.IsCancellationRequested)
            {
                await new WaitForBackgroundThread();
                List<object> dataArray = new List<object>();

                dataArray.Add(data.title);
                dataArray.Add(data.type);
                dataArray.Add(data.deckNumber);
                dataArray.Add(data.mfz);
                dataArray.Add(data.lastEdit);
                dataArray.Add(data.refPhotos.ToArray());

                Hashtable killCardStepsList = new Hashtable();
                for (int i = 0; i < data.killCardSteps.Count; ++i)
                {
                    killCardStepsList.Add(data.killCardSteps[i].title, data.killCardSteps[i].SerializeForNetwork());
                }
                dataArray.Add(killCardStepsList);

                await new WaitForUpdate();
                return dataArray.ToArray();
            }
        }

        catch (System.Exception)
        {
            cts.Cancel();
            throw;
        }

        await new WaitForUpdate();
        return null;
    }
}

public class NetworkImageObject : INetworkObject
{
    public CancellationTokenSource cts { get; set; }
    public NetworkEvent NetEvent { get; set; }
    public string PlayerID { get; set; }
    public object NetData { get; set; }

    public string KillCardTitle;

    public bool ConvertFromNetworkObject(object[] value)
    {
        try
        {
            object[] values = (object[]) value;

            NetEvent = (NetworkEvent) values[0];
            PlayerID = (string) values[1];
            NetData = (Hashtable) values[2];
            KillCardTitle = (string) values[3];
            Debug.Log("Conversion SUCCESS, NetworkImage.");
            return true;
        }
        catch (System.Exception)
        {
            Debug.Log("Conversion FAILED, NetworkImage.");
            return false;
            throw;
        }
    }

    public async Task<object[]> ConvertToNetworkObject()
    {
        await new WaitForBackgroundThread();
        object[] messageArray = new object[]
        {
            NetEvent,
            PlayerID,
            NetData,
            KillCardTitle
        };

        await new WaitForUpdate();

        return messageArray;
    }
}