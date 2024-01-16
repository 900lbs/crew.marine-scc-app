using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal_KillCards_RequestData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_MainMenu_OnShipInitialize"/> class.
    /// </summary>
    /// <param name="shipID">The ship identifier.</param>
    public Signal_KillCards_RequestData(string query)
    {
        Query = query;
    }

    /// <summary>
    /// The ship
    /// </summary>
    public string Query;
}

public class Signal_KillCards_SendData
{
    public Signal_KillCards_SendData(KillCardClass card)
    {
        Card = card;
    }

    public KillCardClass Card;
}

public class Signal_KillCards_ReceiveData
{
    public Signal_KillCards_ReceiveData(KillCardClass card)
    {
        Card = card;
    }

    public KillCardClass Card;
}