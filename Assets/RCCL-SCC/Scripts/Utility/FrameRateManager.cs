using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    public bool isSymphony;
    // Start is called before the first frame update
    void Start()
    {
        if(isSymphony)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        
        Application.targetFrameRate = 60;
    }
}
