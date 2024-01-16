using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InputFieldHighlight : MonoBehaviour
{
    public Image frame;

    public bool selected;

    Color selectedCol;
    Color defaultCol;

    private void Start()
    {
        if(this.transform.Find("Frame") != null)
        {
            frame = this.transform.Find("Frame").GetComponent<Image>();
        }
        
        defaultCol = frame.color;
        ColorUtility.TryParseHtmlString("#00FFED", out selectedCol);
    }

    public void FrameSelected()
    {
        selected = !selected;

        if(selected)
        {
            frame.DOColor(selectedCol, 0.15f);
        }

        if (!selected)
        {
            frame.DOColor(defaultCol, 0.15f);
        }
    }
}
