using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class KeyPressedManager : MonoBehaviour
{
    public Image keyBG;
    public Image keyFrame;
    public Image icon;
    public TextMeshProUGUI key;

    public bool specialChar;

    public Button keyButton;

    public string letter;

    KeyboardManager keyboardManager;

    Color defBGColor;
    Color defFrameColor;
    Color pressedBGColor;

    Sequence selSeq;

    // Start is called before the first frame update
    void Start()
    {
        keyboardManager = FindObjectOfType<KeyboardManager>();
        keyButton.onClick.AddListener(KeyPressed);

        if(!specialChar)
        {
            letter = key.text;
        }

        SetColors();
        BuildSelectionTween();
    }

    public void KeyPressed()
    {
        selSeq.Play().OnComplete(() => selSeq.Rewind());
        keyboardManager.UpdateString(letter);
    }

    void SetColors()
    {
        defBGColor = keyBG.color;
        defFrameColor = keyFrame.color;
        ColorUtility.TryParseHtmlString("#00FFED", out pressedBGColor);
    }

    void BuildSelectionTween()
    {
        selSeq = DOTween.Sequence();
        selSeq.Append(keyBG.DOColor(pressedBGColor, 0.15f));
        selSeq.Insert(0, keyFrame.DOColor(pressedBGColor, 0.15f));
        selSeq.Append(keyBG.DOColor(defBGColor, 0.15f));
        selSeq.Insert(0.15f, keyFrame.DOColor(defFrameColor, 0.15f));

        if (!specialChar || key != null)
        {
            selSeq.Insert(0, key.DOColor(defBGColor, 0.15f));
            selSeq.Insert(0.15f, key.DOColor(defFrameColor, 0.15f));
        }

        else if (specialChar && key == null)
        {
            selSeq.Insert(0, icon.DOColor(defBGColor, 0.15f));
            selSeq.Insert(0.15f, icon.DOColor(defFrameColor, 0.15f));
        }

        selSeq.Pause();
    }
}
