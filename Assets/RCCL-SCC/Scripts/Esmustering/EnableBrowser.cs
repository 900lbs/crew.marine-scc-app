/*
 * © 900lbs of Creative
 * Creation Date: DATE HERE
 * Date last Modified: MOST RECENT MODIFICATION DATE HERE
 * Name: AUTHOR NAME HERE
 * 
 * Description: DESCRIPTION HERE
 * 
 * Scripts referenced: LIST REFERENCED SCRIPTS HERE
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

public class EnableBrowser : MonoBehaviour
{
    public GameObject browser;

    public Button button;

    public bool browserOn;

    // Use this for initialization
    void Start()
    {
        button.onClick.AddListener(enabletheBrowser);
    }

    public void enabletheBrowser()
    {
        if (!Photon.Pun.PhotonNetwork.OfflineMode)
        {
            browserOn = !browserOn;
            browser.GetComponent<BrowserUpdater>().SetURL();
            browser.SetActive(browserOn);
        }
    }
}
