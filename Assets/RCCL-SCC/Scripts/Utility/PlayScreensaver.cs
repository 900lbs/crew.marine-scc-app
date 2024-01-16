using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScreensaver : MonoBehaviour
{
    public string filePath;

    public float timer;
    public float initTime = 1800f;

    public bool sleep;

	// Use this for initialization
	void Start ()
    {
        filePath = Application.streamingAssetsPath + "/Mystify.scr";
        timer = initTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(timer >= 0 && Input.touchCount == 0 && !sleep)
        {
            timer -= Time.deltaTime;
        }

        if(Input.touchCount > 0)
        {
            timer = initTime;
            sleep = false;
        }

		if(Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl) || timer < 0)
        {
            System.Diagnostics.Process.Start(filePath);
            sleep = true;

            timer = initTime;
        }
	}
}
