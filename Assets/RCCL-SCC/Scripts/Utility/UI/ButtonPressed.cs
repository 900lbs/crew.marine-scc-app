using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ButtonPressed : MonoBehaviour
{
    Button thisButton;

    public Image background;
    public Image frame;

    public TextMeshProUGUI title;

    public Color selectedColor;
    public Color selectedColorText;
    public Color deselectedColorText;
    public Color deselectedColorBG;

	// Use this for initialization
	void Start ()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(Highlight);
	}
	
	public void Highlight()
    {
        if(background.color != selectedColor)
        {
            background.DOColor(selectedColor, 0.5f);
            if(frame != null)
            frame.DOColor(selectedColor, 0.5f);
            if(title != null)
            title.DOColor(selectedColorText, 0.5f);
        }

        else
        {
            background.DOColor(deselectedColorBG, 0.5f);
            if(frame != null)
            frame.DOColor(deselectedColorText, 0.5f);
            if(title != null)
            title.DOColor(deselectedColorText, 0.5f);
        }
    }
}
