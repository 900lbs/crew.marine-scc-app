using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlideUIElement : MonoBehaviour
{
    public RectTransform elementToSlide;

    public Vector2 inPos;
    public Vector2 outPos;

    public Button buttonToSlide;

    public bool iconToChange;

    public Image icon;

    public Color inCol;
    public Color outCol;

    public enum State
    {
        isOut,
        isIn
    }

    public State state;

    private void Start()
    {
        buttonToSlide = this.GetComponent<Button>();

        buttonToSlide.onClick.AddListener(() => SlideElement());


        if (elementToSlide.anchoredPosition.x == inPos.x || elementToSlide.anchoredPosition.y == inPos.y)
        {
            state = State.isIn;
        }

        else if(elementToSlide.anchoredPosition == outPos)
        {
            state = State.isOut;
        }
    }

    public void SlideElement()
    {
        switch(state)
        {
            case State.isIn:
                elementToSlide.DOAnchorPos(outPos, 0.35f);
                if(icon != null)
                {
                    icon.DOColor(outCol, 0.35f);
                }
                state = State.isOut;
                break;

            case State.isOut:
                elementToSlide.DOAnchorPos(inPos, 0.35f);
                if (icon != null)
                {
                    icon.DOColor(inCol, 0.35f);
                }
                state = State.isIn;
                break;

            default:
                break;
        }
    }
}

