// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 06-27-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-27-2019
// ***********************************************************************
// <copyright file="ExternalShipDataSignals.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Signal_EniramData_DataUpdated.
/// </summary>
public class Signal_EniramData_DataUpdated
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Signal_EniramData_DataUpdated"/> class.
    /// </summary>
    /// <param name="info">The information.</param>
    public Signal_EniramData_DataUpdated(infoJson info)
    {
        Info = info;
    }

    /// <summary>
    /// Gets the information.
    /// </summary>
    /// <value>The information.</value>
    public infoJson Info { get; private set; }
}

public class Signal_HTTP_OnPostCapabilitiesChanged
{
    public Signal_HTTP_OnPostCapabilitiesChanged(bool isActivated)
    {
		IsActivated = isActivated;
    }

    public bool IsActivated { get; private set; }
}
