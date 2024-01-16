// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-28-2019
// ***********************************************************************
// <copyright file="INetworkObject.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Threading;
using System.Threading.Tasks;
using Zenject;
/// <summary>
/// Interface INetworkObject
/// </summary>
public interface INetworkObject
{
    [Inject]
    CancellationTokenSource cts { get; set; }
    /// <summary>
    /// Gets or sets the net event.
    /// </summary>
    /// <value>The net event.</value>
    NetworkEvent NetEvent { get; set; }

    /// <summary>
    /// Gets or sets the player identifier.
    /// </summary>
    /// <value>The player identifier.</value>
    string PlayerID { get; set; }

    /// <summary>
    /// Gets or sets the net data.
    /// </summary>
    /// <value>The net data.</value>
    object NetData { get; set; }

    /// <summary>
    /// Converts from network object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool ConvertFromNetworkObject(object[] value);

    /// <summary>
    /// Converts to network object.
    /// </summary>
    /// <returns>System.Object[].</returns>
    Task<object[]> ConvertToNetworkObject();

}
