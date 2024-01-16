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

public class DisableButton : MonoBehaviour
{
    public Button button;
    public Toggle toggle;
    public bool endOfFrame;

    private void OnEnable()
    {
        if (button != null)
        {
            button.interactable = true;
        }

    }

    // Use this for initialization
    void Start()
    {
        if(this.GetComponent<Button>() != null)
        {
            button = this.GetComponent<Button>();
            button.onClick.AddListener(disableButton);
        }
        
        else
        {
            toggle = this.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener( (bool value) => {disableToggle();});
        }
    }

    public void disableButton()
    {
        if (button.IsActive())
        {
            button.interactable = false;
            StartCoroutine(ReEnableButton());
        }
    }

    public IEnumerator ReEnableButton()
    {
        if(!endOfFrame)
        {
            yield return new WaitForSeconds(0.25f);
        }
        
        else
        {
            yield return new WaitForEndOfFrame();
        }

        button.interactable = true;
    }

    public void disableToggle()
    {
        Debug.Log("Worked");
        if (toggle.IsActive())
        {
            toggle.interactable = false;
            StartCoroutine(ReEnableToggle());
        }
    }

    public IEnumerator ReEnableToggle()
    {
        if(!endOfFrame)
        {
            yield return new WaitForSeconds(0.25f);
        }
        
        else
        {
            yield return new WaitForEndOfFrame();
        }

        toggle.interactable = true;
        Debug.Log("Reenabled");
    }
}
