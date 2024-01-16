using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

#if !SCC_2_5
public class ButtonHighlight : MonoBehaviour
{
    Image img;
    public Image icon;

    TextMeshProUGUI text;

    public Color textSelectedColor;
    public Color imgSelectedColor;
    public Color unselectedColor;

    public bool highlighted;
    public bool eraseAll;

    // Use this for initialization
    void Start()
    {
        img = this.GetComponent<Image>();

        icon = transform.GetChild(1).GetComponent<Image>();

        text = this.GetComponentInChildren<TextMeshProUGUI>();

        if (eraseAll)
        {
            this.GetComponent<Button>().onClick.AddListener(delegate
            {
                Highlight(true);
            });
        }
    }

    public void Highlight(bool toggled)
    {
        if (toggled)
        {
            img.color = imgSelectedColor;

            if (text != null)
            {
                text.color = textSelectedColor;
            }

            if (icon != null)
            {
                icon.color = imgSelectedColor;
            }
        }

        else
        {
            img.color = unselectedColor;

            if (text != null)
            {
                text.color = unselectedColor;
            }

            if (icon != null)
            {
                icon.color = unselectedColor;
            }
        }
    }
}

#endif
