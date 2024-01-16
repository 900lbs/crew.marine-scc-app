using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    Color defBGColor;
    Color defFrameColor;
    Color pressedBGColor;

    public Sequence selSeq;

    public AddCustomTimer addCustomTimer;

    public KeyboardManager timerKeyboardManager;

    public Image keyBG;
    public Image keyFrame;

    public Image[] fieldFrames;

    public TextMeshProUGUI key;

    public bool isSave;

    public Button saveButton;

    public Color errorFrameCol;
    public Color defaultFrameCol;

    void Start()
    {
        addCustomTimer = FindObjectOfType<AddCustomTimer>();
        //SetColors();
        //BuildSelectionTween();

        if(isSave)
        {
            saveButton.onClick.AddListener(Save);
        }

        else
        {
            saveButton.onClick.AddListener(Delete);
        }
    }

    void Save()
    {
        ResetFrames();
        CheckForErrors();
    }

    void Delete()
    {
        ResetFrames();
        StartCoroutine(DeleteTimer());
    }

    IEnumerator SaveTimer()
    {
        //selSeq.Play().OnComplete(() => selSeq.Rewind());
        addCustomTimer.SaveNewTimer(addCustomTimer.curTimer);
        addCustomTimer.DeSelectTimer();
        yield return new WaitForSeconds(.3f);
        addCustomTimer.ResetValues();
        addCustomTimer.keyboardPrompt.SetActive(false);
        
    }

    IEnumerator DeleteTimer()
    {
        //selSeq.Play().OnComplete(() => selSeq.Rewind());
        addCustomTimer.DeleteTimer();
        addCustomTimer.DeSelectTimer();
        yield return new WaitForSeconds(.3f);
        addCustomTimer.ResetValues();
        timerKeyboardManager.currentLine = null;
        timerKeyboardManager.currentString = "";
        this.gameObject.SetActive(false);
        addCustomTimer.keyboardPrompt.SetActive(false);
    }

    public void CheckForErrors()
    {
        Debug.Log(addCustomTimer.curTimer.title);

        if (addCustomTimer.curTimer.title == null || addCustomTimer.curTimer.title == "")
        {
            fieldFrames[0].DOColor(errorFrameCol, 0.15f);
            ErrorTesting_Keyboard.ErrorCheck(ErrorTesting_Keyboard.errorState.NoTitle, addCustomTimer.error);
            return;
        }

        else if (CheckTimerValues(addCustomTimer.curTimer.initialTime, addCustomTimer.curTimer.alert1, addCustomTimer.curTimer.alert2,
            addCustomTimer.curTimer.alert3, addCustomTimer.curTimer.boolean))
        {
            ErrorTesting_Keyboard.ErrorCheck(ErrorTesting_Keyboard.errorState.BadTimerValue, addCustomTimer.error);
            return;
        }

        else if (CheckAlertOrder(addCustomTimer.curTimer.initialTime, addCustomTimer.curTimer.alert1, addCustomTimer.curTimer.alert2,
           addCustomTimer.curTimer.alert3))
        {
            ErrorTesting_Keyboard.ErrorCheck(ErrorTesting_Keyboard.errorState.AlertOutOfSequence, addCustomTimer.error);
            return;
        }

        else
        {
            ErrorTesting_Keyboard.ErrorCheck(ErrorTesting_Keyboard.errorState.NoErrors, addCustomTimer.error);
            StartCoroutine(SaveTimer());
        }
    }

    bool CheckTimerValues(string initTimer, string alert1, string alert2, string alert3, string boolean)
    {
        string[] initialTimerVals = initTimer.Split(":"[0]);
        string[] alert1TimerVals = alert1.Split(":"[0]);
        string[] alert2TimerVals = alert2.Split(":"[0]);
        string[] alert3TimerVals = alert3.Split(":"[0]);

        if (initTimer == "00:00:00" && boolean == "FALSE" || int.Parse(initialTimerVals[0]) > 99 || int.Parse(initialTimerVals[1]) > 59 || 
            float.Parse(initialTimerVals[2]) > 59)
        {
            fieldFrames[1].DOColor(errorFrameCol, 0.15f);
            return true;
        }

        if(int.Parse(alert1TimerVals[0]) > 99 || int.Parse(alert1TimerVals[1]) > 59 || float.Parse(alert1TimerVals[2]) > 59)
        {
            fieldFrames[2].DOColor(errorFrameCol, 0.15f);
            return true;
        }

        if (int.Parse(alert2TimerVals[0]) > 99 || int.Parse(alert2TimerVals[1]) > 59 || float.Parse(alert2TimerVals[2]) > 59)
        {
            fieldFrames[3].DOColor(errorFrameCol, 0.15f);
            return true;
        }

        if (int.Parse(alert3TimerVals[0]) > 99 || int.Parse(alert3TimerVals[1]) > 59 || float.Parse(alert3TimerVals[2]) > 59)
        {
            fieldFrames[4].DOColor(errorFrameCol, 0.15f);
            return true;
        }

        return false;
    }

    bool CheckAlertOrder(string initTimer, string alert1, string alert2, string alert3)
    {
        string timerVal = initTimer.Replace(":", "");
        string alert1Val = alert1.Replace(":", "");
        string alert2Val = alert2.Replace(":", "");
        string alert3Val = alert3.Replace(":", "");

        int timerInt = int.Parse(timerVal);
        int alert1Int = int.Parse(alert1Val);
        int alert2Int = int.Parse(alert2Val);
        int alert3Int = int.Parse(alert3Val);

        if(alert1Int > timerInt)
        {
            fieldFrames[2].DOColor(errorFrameCol, 0.15f);
            return true;
        }

        if(alert1Int != 0)
        {
            if (alert2Int > alert1Int || alert2Int > timerInt)
            {
                fieldFrames[3].DOColor(errorFrameCol, 0.15f);
                return true;
            }
        }

        if (alert1Int != 0 && alert2Int != 0)
        {
            if (alert3Int > alert2Int || alert3Int > alert1Int || alert3Int > timerInt)
            {
                fieldFrames[4].DOColor(errorFrameCol, 0.15f);
                return true;
            }
        }

        return false;
    }

    public void ResetFrames()
    {
        foreach(Image frame in fieldFrames)
        {
            if(frame.GetComponentInParent<InputFieldHighlight>() != null && frame.GetComponentInParent<InputFieldHighlight>().selected)
            {
                frame.GetComponentInParent<InputFieldHighlight>().FrameSelected();
            }
        }

        timerKeyboardManager.currentLine = null;
    }

    //void SetColors()
    //{
    //    defBGColor = keyBG.color;
    //    defFrameColor = keyFrame.color;
    //    ColorUtility.TryParseHtmlString("#00FFED", out pressedBGColor);
    //    ColorUtility.TryParseHtmlString("#FF0000", out errorFrameCol);
    //    ColorUtility.TryParseHtmlString("#025589", out defaultFrameCol);
    //}

    //void BuildSelectionTween()
    //{
    //    selSeq = DOTween.Sequence();
    //    selSeq.Append(keyBG.DOColor(pressedBGColor, 0.15f));
    //    selSeq.Insert(0, keyFrame.DOColor(pressedBGColor, 0.15f));
    //    selSeq.Append(keyBG.DOColor(defBGColor, 0.15f));
    //    selSeq.Insert(0.15f, keyFrame.DOColor(defFrameColor, 0.15f));
    //    selSeq.Insert(0, key.DOColor(defBGColor, 0.15f));
    //    selSeq.Insert(0.15f, key.DOColor(defFrameColor, 0.15f));

    //    selSeq.Pause();
    //}
}
