using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Demo : MonoBehaviour
{
    [Inject]
    NetworkClient networkClient;

    [Inject]
    SignalBus _signalBus;
    public GameObject tracker;
    public GameObject trackerPrefab;
    public GameObject[] content;
    public GameObject parentObj;

    public Transform shipContainer;

    public Vector3 pos = new Vector3(1079, 775, 0);
    public float zoomLevel = 11;

    public ButtonPressed searchButton;

    public RectTransform childInfoLeft;
    public RectTransform childInfoRight;

    public TextMeshProUGUI markText;
    public TextMeshProUGUI statusText;

    public TMP_InputField firstName;
    public TMP_InputField lastName;

    public Image statusBox;
    public Image statusFrame;
    public Image tabletButtonFrame;

    public Color defStatusCol;
    public Color defFrameCol;
    public Color badStatusCol;
    public Color defStatusTextCol;
    public Color badStatusTextCol;
    public Color foundTabletCol;

    public float childLeftEndPos;
    public float childRightEndPos;
    public float animSpeed = 0.5f;

    public bool missing;

    public ZoomInToDeck zoomInToDeck;

    public Button markAsFoundTablet;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _signalBus.Subscribe<Signal_NetworkClient_OnNetworkEventReceived>(NetworkEventReceived);
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnNetworkEventReceived>(NetworkEventReceived);
    }

    public void NetworkEventReceived(Signal_NetworkClient_OnNetworkEventReceived signal)
    {
        if (signal.NetworkObject.GetType() == typeof(NetworkChildFoundObject))
        {
            UserNameID tryID = 0;
            Enum.TryParse(signal.NetworkObject.PlayerID, out tryID);
            string message = ("<size=31><color=#00FF27>child found</color></size>" + Environment.NewLine + Environment.NewLine + "<size=28><color=#00FFED>" + UserProfileFactory.GetAbbreviatedName(tryID).ToString() + " has marked child as found</color></size>");
            _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.KidFound, message));
        }
    }

    public void ChildSelect()
    {
        tracker = Instantiate(trackerPrefab, shipContainer.Find("DeckMap_06"));
        childInfoLeft.DOAnchorPosX(0, animSpeed);
        childInfoRight.DOAnchorPosX(0, animSpeed);
        zoomInToDeck.stopZoom = false;
        StartCoroutine(zoomInToDeck.DoubleTapToZoom(pos, zoomLevel));
        DeckManager.DeckSelectionChanged("06", ActiveState.Selected);
    }

    public void EnableTrackerTablet()
    {
        tracker = Instantiate(trackerPrefab, shipContainer.Find("DeckMap_06"));
        DeckManager.DeckSelectionChanged("06", ActiveState.Selected);

        MarkAsMissing();
    }

    public void MarkStatus()
    {
        if (!missing)
        {
            MarkAsMissing();
        }

        else
        {
            MarkAsFound();
        }

        missing = !missing;
    }

    //Put network code here for share location, we will hook it into the button in scene.

    public void MarkAsMissing()
    {
        markText.text = "mark as found";
        statusText.text = "missing";
        statusBox.color = badStatusCol;
        statusFrame.color = badStatusCol;
        statusText.color = badStatusTextCol;
        tracker.GetComponent<Demo_Tracker>().ToggleColor();
    }

    public void MarkAsFound()
    {
        markText.text = "mark as missing";
        statusText.text = "location known";
        statusBox.color = defStatusCol;
        statusFrame.color = defFrameCol;
        statusText.color = defStatusTextCol;
        tracker.GetComponent<Demo_Tracker>().ToggleColor();
    }

    public async void MarkAsFoundTablet()
    {
        markText.color = foundTabletCol;
        tabletButtonFrame.color = foundTabletCol;
        statusText.text = "location known";
        statusBox.color = defStatusCol;
        statusFrame.color = defFrameCol;
        statusText.color = defStatusTextCol;
        tracker.GetComponent<Demo_Tracker>().ToggleColor();

        NetworkChildFoundObject networkChildFoundObject = new NetworkChildFoundObject();

        await networkClient.SendNewNetworkEvent(networkChildFoundObject);
    }

    public void ResetChildTracking()
    {
        searchButton.Highlight();

        firstName.text = "";
        lastName.text = "";

        childInfoRight.DOAnchorPosX(childRightEndPos, 0);
        childInfoLeft.DOAnchorPosX(childLeftEndPos, 0);

        DeckManager.DeckSelectionChanged("06", ActiveState.Enabled);

        MarkAsFound();

        foreach (GameObject go in content)
        {
            go.SetActive(false);
        }

        Destroy(tracker);

        //parentObj.SetActive(false);
    }

    public void ResetChildTrackingTablet()
    {
        DeckManager.DeckSelectionChanged("06", ActiveState.Enabled);

        Destroy(tracker);

        markAsFoundTablet.interactable = true;

        //parentObj.SetActive(false);
    }
}