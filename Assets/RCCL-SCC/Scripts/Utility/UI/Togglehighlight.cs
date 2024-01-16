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
using TMPro;

public class Togglehighlight : MonoBehaviour 
{
    Image img;

    TextMeshProUGUI text;

    public Color textSelectedColor;
    public Color textUnselectedColor;
    public Color imgSelectedColor;
    public Color unselectedColor;

    public bool highlighted;

    public bool mfzFilter;

    // Use this for initialization
    void Awake()
    {
        img = this.GetComponent<Image>();

        text = this.GetComponentInChildren<TextMeshProUGUI>();

        if(this.GetComponent<Button>() != null)
        {
            this.GetComponent<Button>().onClick.AddListener(Highlight);
            //Debug.Log("Added");
        }
    }

    public void Highlight()
    {
        if (!highlighted)
        {
            img.color = imgSelectedColor;
            if(!mfzFilter)
            {
                img.fillCenter = true;
            }
            text.color = textSelectedColor;
            highlighted = true;
        }

        else
        {
            img.color = unselectedColor;
            if(!mfzFilter)
            {
                img.fillCenter = false;
            }
            text.color = textUnselectedColor;
            highlighted = false;
        }
    }
}
