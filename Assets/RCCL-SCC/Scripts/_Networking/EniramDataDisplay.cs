// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-27-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-27-2019
// ***********************************************************************
// <copyright file="EniramDataDisplay.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Zenject;
using Photon.Pun;

/// <summary>
/// Class EniramDataDisplay.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class EniramDataDisplay : MonoBehaviour
{
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;

    /// <summary>
    /// Constructs the specified signal.
    /// </summary>
    /// <param name="signal">The signal.</param>
    [Inject]
    public void Construct(SignalBus signal)
    {
        _signalBus = signal;
    }

    /// <summary>
    /// The sog
    /// </summary>
    public TextMeshProUGUI SOG;
    /// <summary>
    /// The STW
    /// </summary>
    public TextMeshProUGUI STW;
    /// <summary>
    /// The wind direction text
    /// </summary>
    public TextMeshProUGUI windDirectionText;
    /// <summary>
    /// The wind speed text
    /// </summary>
    public TextMeshProUGUI windSpeedText;

    /// <summary>
    /// The dir arrow
    /// </summary>
    public Image dirArrow;
    /// <summary>
    /// The ship
    /// </summary>
    public Image ship;

    /// <summary>
    /// The new heading
    /// </summary>
    Vector3 newHeading;
    /// <summary>
    /// The new wind dir
    /// </summary>
    Vector3 newWindDir;

    /// <summary>
    /// The information
    /// </summary>
    public infoJson info;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    private void Start()
    {
        _signalBus.Subscribe<Signal_EniramData_DataUpdated>(CheckData);
    }

    /// <summary>
    /// Called when [destroy].
    /// </summary>
    private void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_EniramData_DataUpdated>(CheckData);
    }

    /// <summary>
    /// Checks the data.
    /// </summary>
    /// <param name="jsonResult">The json result.</param>
    public void CheckData(Signal_EniramData_DataUpdated jsonResult)
    {
        //Debug.Log("Received eniram data: " + jsonResult.Info, this);
        if (jsonResult.Info != null && !PhotonNetwork.OfflineMode)
        {
            info = jsonResult.Info;

            windSpeedText.text = info.windSpeed.ToString("#0.0") + "<size=16> kn</size>";
            windDirectionText.text = info.windDirection.ToString("#00.0");
            SOG.text = info.sog.ToString("#0.0") + "<size=16> kt</size>";
            if (info.stw < 0)
            {
                info.stw = 0;
            }
            STW.text = info.stw.ToString("#0.0") + "<size=16> kt</size>";

            newHeading.Set(0, 0, (float)info.heading * -1);
            ship.rectTransform.eulerAngles = newHeading;

            newWindDir.Set(0, 0, (float)info.windDirection * -1);
            dirArrow.rectTransform.eulerAngles = newWindDir;

            info = null;
        }
    }
}
