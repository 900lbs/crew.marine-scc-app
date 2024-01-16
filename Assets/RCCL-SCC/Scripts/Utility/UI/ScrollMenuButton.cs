using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollMenuButton : MonoBehaviour
{
    public RectTransform contentHolder;

    public Button leftButton;
    public Button rightButton;

    public Image leftArrowIcon;
    public Image rightArrowIcon;

    public float newPosition;
    public float distanceBetweenPanels;

    public int numberOfPanels;
    public int currentPanel;

    public Color arrowInactive;
    public Color arrowActive;

    // Use this for initialization
    void Start ()
    {
        leftButton.onClick.AddListener(LeftScroll);
        rightButton.onClick.AddListener(RightScroll);

        if(leftArrowIcon == null)
            leftArrowIcon = leftButton.transform.GetChild(0).GetComponent<Image>();
        
        if(rightArrowIcon == null)
            rightArrowIcon = rightButton.transform.GetChild(0).GetComponent<Image>();

        leftButton.enabled = false;

        arrowInactive = leftArrowIcon.color;

        if(rightArrowIcon != null)
        {
            arrowActive = rightArrowIcon.color;
        }
        
    }

    public void LeftScroll()
    {
        if(currentPanel > 1)
        {
            newPosition += distanceBetweenPanels;
            currentPanel--;

            rightButton.enabled = true;
            rightArrowIcon.DOColor(arrowActive, 0.5f);

            if(currentPanel == 1)
            {
                leftButton.enabled = false;
                leftArrowIcon.DOColor(arrowInactive, 0.5f);
            }
        }

        ScrollMenu(newPosition);
    }

    public void RightScroll()
    {
        if(currentPanel < numberOfPanels)
        {
            newPosition -= distanceBetweenPanels;
            currentPanel++;

            leftButton.enabled = true;
            leftArrowIcon.DOColor(arrowActive, 0.5f);

            if (currentPanel == numberOfPanels)
            {
                rightButton.enabled = false;
                rightArrowIcon.DOColor(arrowInactive, 0.5f);
            }
        }

        ScrollMenu(newPosition);
    }
	
	public void ScrollMenu(float destination)
    {
        contentHolder.DOAnchorPosX(destination, 0.5f, true);
    }
}
