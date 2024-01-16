using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum CheckmarkType
{
    KillCard
}

public class NetworkCheckmarkObject : INetworkObject
{
    public CancellationTokenSource cts { get; set; }
    public NetworkEvent NetEvent { get; set; }
    public string PlayerID { get; set; }
    public object NetData { get; set; } //Empty
    public string OptionalSearchParameter; /// I.example KillCardStep name string.
    public CheckmarkType CheckmarkAssignedType;
    public bool IsChecked;

    public bool ConvertFromNetworkObject(object[] value)
    {
            object[] values = (object[])value;
        try
        {

            NetEvent = (NetworkEvent)values[0];
            PlayerID = (string)values[1];
            NetData = (string)values[2];
            OptionalSearchParameter = (string)values[3];
            byte convertedCheckmark = (byte)values[4]; // Is received as a byte
            CheckmarkAssignedType = (CheckmarkType)convertedCheckmark; // Cast into an enum type.
            IsChecked = (bool)values[5];
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log("Could not convert to Checkmark object: " + e.Message + " / " + e.StackTrace);
            return false;
            throw;
        }
    }

    public async Task<object[]> ConvertToNetworkObject()
    {
        cts = new CancellationTokenSource();

        try
        {
            if (!cts.IsCancellationRequested)
            {
                await new WaitForBackgroundThread();
                List<object> dataArray = new List<object>()
                {
                    NetEvent,
                    PlayerID,
                    NetData,
                    OptionalSearchParameter,
                    (byte)CheckmarkAssignedType,
                    IsChecked
                };

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
