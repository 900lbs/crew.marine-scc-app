using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;

public class BrowserReload : MonoBehaviour
{
    public Browser browser;

    public float countdown = 30f;

    public float initCountdown = 30f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(countdown > 0)
        {
            countdown -= Time.deltaTime;
        }

        if(countdown <= 0)
        {
            if(browser.IsLoaded)
            {
                browser.Reload();
                countdown = initCountdown;
            }

            else
            {
                countdown = initCountdown;
            }
        }
	}
}
