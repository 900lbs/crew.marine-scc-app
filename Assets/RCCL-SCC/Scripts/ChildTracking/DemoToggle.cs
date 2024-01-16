using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoToggle : MonoBehaviour
{
    public GameObject kidTrackerGO;
    
    public bool isVisible;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(ToggleVisibility);
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;

        kidTrackerGO.SetActive(isVisible);
    }
}
