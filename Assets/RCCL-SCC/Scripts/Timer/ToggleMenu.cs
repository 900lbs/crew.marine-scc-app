using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleMenu : MonoBehaviour
{
    public Button menuButtonDown;
    public Button menuButtonUp;
    public Button topSquare;
    public Button bottomSquare;

    private CanvasGroup mainMenuCanvasGroup;
    private CanvasGroup timerEditMenuCanvasGroup;

    public RectTransform mainMenuRectTransform;
    public RectTransform timerEditMenuRectTransform;

    public Image[] squares;
    public Image[] arrows;

    public Color squareSelected;
    public Color squareDeSelected;
    public Color arrowInactive;

    public float downValue = 300f;

    public bool mainMenuVisible = true;

	// Use this for initialization
	void Start ()
    {
        menuButtonDown.onClick.AddListener(ToggleMenus);
        menuButtonUp.onClick.AddListener(ToggleMenus);
        topSquare.onClick.AddListener(ToggleMenus);
        bottomSquare.onClick.AddListener(ToggleMenus);
        menuButtonUp.interactable = false;
        topSquare.interactable = false;

        squareSelected = squares[0].color;
        squareDeSelected = squares[1].color;
        arrowInactive = arrows[0].color;

        mainMenuCanvasGroup = mainMenuRectTransform.GetComponent<CanvasGroup>();
        timerEditMenuCanvasGroup = timerEditMenuRectTransform.GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	public void ToggleMenus ()
    {
		if(mainMenuVisible)
        {
            ToggleVisibility(mainMenuCanvasGroup, mainMenuRectTransform, 0, -300f);
            ToggleVisibility(timerEditMenuCanvasGroup, timerEditMenuRectTransform, 1, 0);
            mainMenuVisible = false;
            menuButtonDown.interactable = false;
            menuButtonUp.interactable = true;
            SwitchSquares(squares[1], squares[0], arrows[0], arrows[1]);
        }

        else
        {
            ToggleVisibility(mainMenuCanvasGroup, mainMenuRectTransform, 1, 0f);
            ToggleVisibility(timerEditMenuCanvasGroup, timerEditMenuRectTransform, 0, -300);
            mainMenuVisible = true;
            menuButtonDown.interactable = true;
            menuButtonUp.interactable = false;
            SwitchSquares(squares[0], squares[1], arrows[1], arrows[0]);
        }
	}

    void ToggleVisibility(CanvasGroup canvasGroup, RectTransform rectTransform, float opacity, float endYLocation)
    {
        rectTransform.DOAnchorPosY(endYLocation, 0.5f);
        canvasGroup.DOFade(opacity, 0.5f);
    }

    void SwitchSquares(Image selSquare, Image deSelSquare, Image activeArrow, Image inactiveArrow)
    {
        selSquare.DOColor(squareSelected, 0.5f);
        selSquare.fillCenter = true;
        selSquare.GetComponent<Button>().interactable = false;
        deSelSquare.DOColor(squareDeSelected, 0.5f);
        deSelSquare.fillCenter = false;
        deSelSquare.GetComponent<Button>().interactable = true;
        activeArrow.DOColor(squareDeSelected, 0.5f);
        inactiveArrow.DOColor(arrowInactive, 0.5f);
    }
}
