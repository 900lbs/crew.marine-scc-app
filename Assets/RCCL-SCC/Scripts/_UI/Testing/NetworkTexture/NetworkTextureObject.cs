using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class NetworkTextureObject : INetworkObject
{
    [Inject]
    public CancellationTokenSource cts { get; set; }
    public NetworkEvent NetEvent { get; set; }
    public string PlayerID { get; set; }
    public object NetData { get; set; }

    public bool ConvertFromNetworkObject(object[] value)
    {
        try
        {
            object[] values = (object[])value;

            NetEvent = (NetworkEvent)values[0];
            PlayerID = (string)values[1];
            NetData = (object)values[2];
            return true;
        }
        catch (System.Exception)
        {
            Debug.Log("Not a texture object.");
            return false;
            throw;
        }
    }

    public async Task<object[]> ConvertToNetworkObject()
    {
        if (!cts.IsCancellationRequested)
        {
            await new WaitForBackgroundThread();
            object[] messageArray = new object[]
            {
            NetEvent,
            PlayerID,
            NetData,
            };
            await new WaitForUpdate();
            return messageArray;
        }

            await new WaitForUpdate();
            return null;
    }
}
