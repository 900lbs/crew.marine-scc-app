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
using DG.Tweening;
using Zenject;

#if SCC_2_5
public class ToggleSwitch : MonoBehaviour
{
    AnnotationManager annotationManager;

    [Inject]
    public void Construct(AnnotationManager annoManager)
    {
        annotationManager = annoManager;
    }

    [HideInInspector]
    public Button button;

    public Image icon;
    public Image toggleFrame;
    public Image toggleKnob;

    public RectTransform knobTrans;

    public TextMeshProUGUI text;

    public Vector3 startPos;
    public Vector3 endPos;

    public float dist;

    public Color selColor;
    public Color deSelColor;
    public Color textDeSelColor;

    public bool toggled;

    public bool ToggleOnStart = true;

    // Use this for initialization
    void Start()
    {
        button = this.GetComponent<Button>();
        if (!GetComponent<ToggleCurrentShipPropertyHoldersButton>())
            button.onClick.AddListener(Toggle);

        startPos = knobTrans.anchoredPosition;
        endPos = new Vector3(startPos.x + dist, startPos.y, startPos.z);

        if (text != null)
        {
            textDeSelColor = text.color;
        }

        if (ToggleOnStart)
        {
            if (toggled)
            {
                knobTrans.DOAnchorPosX(endPos.x, 0.125f);
                if (icon)
                    icon.enabled = true;

                toggled = true;

                if (text != null)
                {
                    text.DOColor(selColor, 0.125f);
                }

                toggleFrame.DOColor(selColor, 0.125f);
                toggleKnob.DOColor(selColor, 0.125f);
            }
        }
    }
    
    public void Toggle()
    {
        if (!toggled)
        {
            knobTrans.DOAnchorPosX(endPos.x, 0.125f);

            if (icon)
                icon.enabled = true;

            toggled = true;

            if (text != null)
            {
                text.DOColor(selColor, 0.125f);
            }

            toggleFrame.DOColor(selColor, 0.125f);
            toggleKnob.DOColor(selColor, 0.125f);
        }

        else if (toggled)
        {
            knobTrans.DOAnchorPosX(startPos.x, 0.125f);
            if (icon)
                icon.enabled = false;

            toggled = false;

            if (text != null)
            {
                text.DOColor(textDeSelColor, 0.125f);
            }

            toggleFrame.DOColor(deSelColor, 0.125f);
            toggleKnob.DOColor(deSelColor, 0.125f);
        }

    }

    public void Toggle(bool active)
    {
        if (active)
        {
            knobTrans.DOAnchorPosX(endPos.x, 0.125f);

            if (icon)
                icon.enabled = true;

            toggled = true;

            if (text != null)
            {
                text.DOColor(selColor, 0.125f);
            }

            toggleFrame.DOColor(selColor, 0.125f);
            toggleKnob.DOColor(selColor, 0.125f);
        }

        else
        {
            knobTrans.DOAnchorPosX(startPos.x, 0.125f);
            if (icon)
                icon.enabled = false;

            toggled = false;

            if (text != null)
            {
                text.DOColor(textDeSelColor, 0.125f);
            }

            toggleFrame.DOColor(deSelColor, 0.125f);
            toggleKnob.DOColor(deSelColor, 0.125f);
        }

    }
}

#elif !SCC_2_5
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

public class ToggleSwitch : MonoBehaviour 
{
    private Button button;

    public DrawOnDeck draw;

    public Image icon;
    public Image toggleFrame;
    public Image toggleKnob;

    public RectTransform knobTrans;

    public TextMeshProUGUI text;

    private Vector3 startPos;
    private Vector3 endPos;

    public float dist;

    public Color selColor;
    public Color deSelColor;
    public Color textDeSelColor;

    public bool toggled;
    public bool forArtwork;
    public bool forGAEdits;

	// Use this for initialization
	void Start () 
	{
        button = this.GetComponent<Button>();
        button.onClick.AddListener(Toggle);

        startPos = knobTrans.anchoredPosition;
        endPos = new Vector3(startPos.x + dist, startPos.y, startPos.z);

        if(text != null)
        {
            textDeSelColor = text.color;
        }

        if (forArtwork)
        {
            draw = FindObjectOfType<DrawOnDeck>();

            Toggle();
        }

        if(forGAEdits)
        {
            draw = FindObjectOfType<DrawOnDeck>();

            Toggle();
        }
	}

    public void Toggle()
    {
        if(!toggled)
        {
            knobTrans.DOAnchorPosX(endPos.x, 0.125f);
            icon.enabled = true;
            toggled = true;
           
            if(text != null)
            {
                text.DOColor(selColor, 0.125f);
            }

            toggleFrame.DOColor(selColor, 0.125f);
            toggleKnob.DOColor(selColor, 0.125f);
        }

        else if(toggled)
        {
            knobTrans.DOAnchorPosX(startPos.x, 0.125f); 
            icon.enabled = false;
            toggled = false;

            if (text != null)
            {
                text.DOColor(textDeSelColor, 0.125f);
            }

            toggleFrame.DOColor(deSelColor, 0.125f);
            toggleKnob.DOColor(deSelColor, 0.125f);
        }

        if(forArtwork)
        {
            draw.ToggleArtwork();
        }

        if(forGAEdits)
        {
            draw.ToggleGAEdits();
        }
    }
}

#endif