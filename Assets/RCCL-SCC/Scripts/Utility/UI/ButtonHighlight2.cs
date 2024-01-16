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

public class ButtonHighlight2 : MonoBehaviour
{
    Image img;

    TextMeshProUGUI text;

    public Color textSelectedColor;
    public Color imgSelectedColor;
    public Color unselectedColor;

    public ButtonHighlight2 buttonHighlight2;
    public ButtonHighlight2 buttonHighlight3;

    public bool highlighted;
    public bool eraseAll;

    // Use this for initialization
    void Start()
    {
        img = this.GetComponent<Image>();

        if(GetComponentInChildren<TextMeshProUGUI>())
            text = this.GetComponentInChildren<TextMeshProUGUI>();

        if (eraseAll)
        {
            this.GetComponent<Button>().onClick.AddListener(delegate
            {
                StartCoroutine(resetIt());
            });
        }

        else
        {
            this.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (buttonHighlight2.highlighted)
                {
                    buttonHighlight2.highlighted = false;
                    buttonHighlight2.Highlight(buttonHighlight2.highlighted);
                }
                highlighted = !highlighted;
                Highlight(highlighted);
            });
        }
    }

    public void Highlight(bool toggled)
    {
        if (toggled)
        {
            img.fillCenter = true;
            img.color = imgSelectedColor;
            if(text)
                text.color = textSelectedColor;
            this.GetComponent<Button>().interactable = false;
        }

        else
        {
            img.fillCenter = false;
            img.color = unselectedColor;
            if(text)
                text.color = unselectedColor;
            this.GetComponent<Button>().interactable = true;
        }
    }

    public IEnumerator resetIt()
    {
        Highlight(true);
        yield return new WaitForSeconds(0.15f);
        Highlight(false);
        buttonHighlight2.highlighted = false;
        buttonHighlight2.Highlight(buttonHighlight2.highlighted);
        buttonHighlight3.highlighted = false;
        buttonHighlight3.Highlight(buttonHighlight3.highlighted);
    }
}
