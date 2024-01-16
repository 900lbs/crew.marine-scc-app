using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Button_ColorSwitch : MonoBehaviour
{
    public Image addBGImage;
    public Image addIconImage;

    public Button colorSwitchButton;

    Color activeColor;
    Color iconActiveColor;
    Color inactiveColor;
    Color iconInactiveColor;

    Sequence selectSeq;
    
    // Start is called before the first frame update
    void Start()
    {
        SetColors();

        colorSwitchButton = this.GetComponent<Button>();

        colorSwitchButton.onClick.AddListener(ChangeColors);

        BuildSelectionTween();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetColors()
    {
        ColorUtility.TryParseHtmlString("#021D3A", out inactiveColor);
        ColorUtility.TryParseHtmlString("#021D3A", out iconActiveColor);
        ColorUtility.TryParseHtmlString("#00FFED", out activeColor);
        ColorUtility.TryParseHtmlString("#00FFED", out iconInactiveColor);
    }

    void ChangeColors()
    {
        selectSeq.Play().OnComplete(()=>selectSeq.Rewind());
    }

    void BuildSelectionTween()
    {
        selectSeq = DOTween.Sequence();
        selectSeq.Append(addBGImage.DOColor(activeColor, 0.15f));
        selectSeq.Insert(0, addIconImage.DOColor(iconActiveColor, 0.15f));
        selectSeq.Append(addBGImage.DOColor(inactiveColor, 0.15f));
        selectSeq.Insert(0.15f, addIconImage.DOColor(iconInactiveColor, 0.15f));
        selectSeq.Pause();
    }
}
