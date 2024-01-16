// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-30-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-26-2019
// ***********************************************************************
// <copyright file="BridgeState.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Photon.Realtime;
using Zenject;
#if SCC_2_5
/// <summary>
/// This state is called anytime there is an important connection event
/// Implements the <see cref="GameStateEntity" />
/// </summary>
/// <seealso cref="GameStateEntity" />
public class BridgeState : GameStateEntity
{
	#region Injection Construction
	/// <summary>
	/// The main menu
	/// </summary>
	readonly MainMenu mainMenu;
	/// <summary>
	/// The asynchronous
	/// </summary>
	readonly ASyncProcessor async;

	/// <summary>
	/// Initializes a new instance of the <see cref="BridgeState"/> class.
	/// </summary>
	/// <param name="_menu">The menu.</param>
	/// <param name="asyn">The asyn.</param>
	[Inject]
    public BridgeState(MainMenu _menu, ASyncProcessor asyn)
    {
        mainMenu = _menu;
        async = asyn;
    }

	/// <summary>
	/// The message
	/// </summary>
	public string Message;

	/// <summary>
	/// The is bridging
	/// </summary>
	bool isBridging = false;
	/// <summary>
	/// The count
	/// </summary>
	int count = 0;

	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	public override void Initialize()
    {
        //Debug.Log("BridgeState Initialized");
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Starts this instance.
	/// </summary>
	public async override void Start()
    {
        mainMenu.BridgingControl.FadeCanvas(true);

        //Debug.Log("BridgeState Started: " + mainMenu.CurrentSceneState.ToString());
        isBridging = true;

        await Bridging();
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Ticks this instance.
	/// </summary>
	public override void Tick()
    {
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Lates the dispose.
	/// </summary>
	public override void LateDispose()
    {
        isBridging = false;
        mainMenu.BridgingControl.FadeCanvas(false);
        //Debug.Log("BridgeState Disposed: " + mainMenu.CurrentSceneState.ToString());
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Bridgings this instance.
	/// </summary>
	/// <returns>Task.</returns>
	async Task Bridging()
    {
        while (isBridging)
        {
            await new WaitForBackgroundThread();

            count++;

            if (count > 2)
                count = 0;

            await new WaitForUpdate();

            mainMenu.BridgingControl.UpdateConnectionText(DetermineConnectingText(count));

            await new WaitForSeconds(0.65f);
        }

    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Determines the connecting text.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>System.String.</returns>
	string DetermineConnectingText(int index)
    {
        switch (mainMenu.CurrentSceneState)
        {
            case SceneState.Connecting:
            case SceneState.Loading:
                Message = "Please Wait";
                break;
            case SceneState.Idle:
                Message = "";
                break;
        }
        switch (index)
        {
            case 0:
                return Message + ".";
            case 1:
                return Message + "..";
            case 2:
                return Message + "...";
            default:
                return Message;
        }
    }


	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Class Factory.
	/// Implements the <see cref="Zenject.PlaceholderFactory{BridgeState}" />
	/// </summary>
	/// <seealso cref="Zenject.PlaceholderFactory{BridgeState}" />
	public class Factory : PlaceholderFactory<BridgeState> { }

}
#endif