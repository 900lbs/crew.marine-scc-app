using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// ShareReceiveView is used to receive and send view da``` from client to another. This script should be 
/// attached to the ScreenSpaceCamera_Canvas gameobject in the scene to insure proper functionality. Methods
/// and important variables are documented.
/// </summary>
public class ShareReceiveView : UI_Button
{
    #region Injection Construction
    DeckManager deckManager;
    ZoomInToDeck zoomIntoDeck;
    Settings settings;

    [Inject]
    public void Construct(DeckManager deckMan,
    ZoomInToDeck zoomDeck,
    Settings setting)
    {
        deckManager = deckMan;
        zoomIntoDeck = zoomDeck;
        settings = setting;
    }
    #endregion

    //Empty variable for ShareViewData class.
    public ShareViewData savedData;

    /// <summary>
    /// This function takes in the receive ShareViewData class and uses it to set the proper variables to 
    /// simulate the current view of another user. This should be called when a user confirms that they want
    /// to see the view that has been shared with them. It can be used in conjunction with 
    /// ShareViewData.ConvertFromString(), which will parse the string sent via the network and return a 
    /// ShareViewData object.
    /// </summary>
    /// <param name="viewData"></param>
    public void ReceiveView(ShareViewData viewData)
    {
        if (deckManager.ActiveDecks.Count > 0 && deckManager.ActiveDecks.Count < deckManager.AllDecks.Count)
        {
            DeckManager.OnDeckIsolationToggled?.Invoke(false);
        }

        if (viewData.IsolatedDecks != null && viewData.IsolatedDecks.Count >= 1)
        {
            foreach (KeyValuePair<string, Deck> kvp in viewData.IsolatedDecks)
            {
                DeckManager.DeckSelectionChanged(kvp.Key, ActiveState.Selected);
            }

            DeckManager.OnDeckIsolationToggled?.Invoke(true);
        }

        FeatureManager.SetActiveFeaturesManually(viewData.DeckFeatures);

        zoomIntoDeck.DeckRotation(viewData.Rotation, viewData.Rotated);

        zoomIntoDeck.deckHolderPivot.anchoredPosition = viewData.ParentPos;

        zoomIntoDeck.deckholder.anchoredPosition = viewData.ObjPos;

        zoomIntoDeck.canvasScaler.scaleFactor = (viewData.ZoomValue > 1f) ? viewData.ZoomValue : 1f;
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();
        SaveViewToShare();
    }

    protected override void OnStateChange()
    {

    }

    /// <summary>
    /// Use this for saving the current view.true It should be called when the share my view button is called
    /// and will store the current information from zoomIntoDeck.cs, FeautreManager, and DeckManager to a class
    /// that can then be parsed to a string and sent via the network. An example of parsing the class to a string
    /// can be seen below.true Make sure to remove this one since it is currently writing to the streaming assets
    /// folder.
    /// </summary>
    public async void SaveViewToShare()
    {
        savedData = new ShareViewData(zoomIntoDeck.deckHolderPivot.anchoredPosition,
                                    zoomIntoDeck.deckholder.anchoredPosition,
                                    zoomIntoDeck.deckHolderPivot.eulerAngles,
                                    zoomIntoDeck.canvasScaler.scaleFactor,
                                    zoomIntoDeck.annotationManager.Rotated,
                                    FeatureManager.GetActiveFeatures,
                                    deckManager.ActiveDecks);

        string convertedData = ShareViewData.ConvertToString(savedData);
        NetworkShareObject shareObject = new NetworkShareObject(NetworkEvent.Create, NetworkClient.GetUserName().ToString(), convertedData);

        //Debug.Log("Player: " + shareObject.PlayerID + " is sending: " + shareObject.NetEvent.ToString() + " / " + (string)shareObject.NetData, this);
        await networkClient.SendNewNetworkEvent(shareObject);
    }
}
