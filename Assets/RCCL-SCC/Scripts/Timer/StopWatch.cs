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

using Zenject;

public class StopWatch : MonoBehaviour
{
    #region Injection Construction
    [Inject]
    XMLWriterDynamic.Factory xmlFactory;

    #endregion

    private string timedis;

    private Button startButton;
    private Button stopButton;
    private Button resetButton;
    private Button selectButton;

    private ButtonHighlight2 resetButtonHighlight;

    private float initialSeconds;
    private int initalMinutes;
    private int initalHours;

    private bool alert1;
    private bool alert2;
    private bool alert3;

    public RectTransform textBG;

    public Image textBGImage;

    public AddCustomTimer addCustomTimer;

    public AlertTime alert1Time;
    public AlertTime alert2Time;
    public AlertTime alert3Time;

    public float sec;
    public int min;
    public int hou;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI titleText;

    public Color initialColor;
    public Color alert1Color;
    public Color alert2Color;
    public Color alert3Color;
    public Color selectedColor;
    public Color deselectedColor;

    public bool startime = false;
    public bool isStopWatch;
    public bool selected;
    public bool tablet;

    public XMLWriterDynamic xmlWriter;

    void Start()
    {
        addCustomTimer = FindObjectOfType<AddCustomTimer>();
        SetUpButtons();

        if (isStopWatch)
        {
            sec = 0;
            min = 0;
            hou = 0;
        }

        initialSeconds = sec;
        initalMinutes = min;
        initalHours = hou;

        initialColor = timeText.color;

        if (titleText != null)
        {
            this.gameObject.name = titleText.text;
            deselectedColor = titleText.color;
        }

        StartCoroutine(SetTextBG());

        PopulateTime();
        try
        {
            xmlWriter = xmlFactory.Create(gameObject, XMLType.Timers);
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    void Update()
    {
        if (startime == true)
        {
            if (isStopWatch)
            {
                StopWatchTimer();
            }

            else
            {
                CountDownTimer();
            }

            PopulateTime();
        }
    }

    public void StopWatchTimer()
    {
        sec += Time.deltaTime;

        if (Mathf.Floor(sec) >= 60)
        {
            sec = 0; min = min + 1;
        }

        if (min >= 60)
        {
            min = 0; hou = hou + 1;
        }
    }

    public void CountDownTimer()
    {
        sec -= Time.deltaTime;

        if (Mathf.Floor(sec) < 0)
        {
            if (min > 0 || hou > 0)
            {
                sec = 60;
                min = min - 1;
            }

            else
            {
                sec = 0;
                StopTimer();
            }
        }

        if (min < 0)
        {
            if (hou > 0)
            {
                min = 59;
                hou = hou - 1;
            }

            else
            {
                min = 0;
            }
        }

        CheckAlert();
    }

    public void PopulateTime()
    {
        timedis = "[" + (hou.ToString("00") + ":" + min.ToString("00") + ":" + Mathf.Floor(sec).ToString("00")) + "]";

        timeText.text = timedis;
    }

    public void CheckAlert()
    {
        //Make this easier to read
        if (!alert1)
        {
            if (hou <= alert1Time.alertHours)
            {
                if (min <= alert1Time.alertMinutes)
                {
                    if (sec <= alert1Time.alertSeconds + 1)
                    {
                        timeText.color = alert1Color;
                        alert1 = true;

                        if (tablet)
                        {
                            titleText.color = selectedColor;
                            textBGImage.enabled = true;
                            textBGImage.color = alert1Color;
                        }
                    }
                }
            }
        }

        if (!alert2)
        {
            if (hou <= alert2Time.alertHours)
            {
                if (min <= alert2Time.alertMinutes)
                {
                    if (sec <= alert2Time.alertSeconds + 1)
                    {
                        timeText.color = alert2Color;
                        alert2 = true;

                        if (tablet)
                        {
                            titleText.color = selectedColor;
                            textBGImage.enabled = true;
                            textBGImage.color = alert2Color;
                        }
                    }
                }
            }
        }

        if (!alert3)
        {
            if (hou <= alert3Time.alertHours)
            {
                if (min <= alert3Time.alertMinutes)
                {
                    if (sec <= alert3Time.alertSeconds + 1)
                    {
                        timeText.color = alert3Color;
                        alert3 = true;

                        if (tablet)
                        {
                            titleText.color = selectedColor;
                            textBGImage.enabled = true;
                            textBGImage.color = alert3Color;
                        }
                    }
                }
            }
        }
    }

    public async void ResetTime()
    {
        startime = false;
        sec = initialSeconds;
        min = initalMinutes;
        hou = initalHours;
        timeText.color = initialColor;
        alert1 = false;
        alert2 = false;
        alert3 = false;
        PopulateTime();

        if (tablet)
        {
            startButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);

            if (titleText != null)
            {
                titleText.color = initialColor;
                textBGImage.enabled = false;
            }
        }

        else
        {
            startButton.interactable = true;
            stopButton.interactable = false;
        }

        await xmlWriter.AttemptCustomSave(this, XMLTimers.TimerAction.Reset);
        await xmlWriter.Save();
    }

    public async void StartTimer()
    {
        startime = true;

        if (tablet)
        {
            startButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);
        }

        else
        {
            startButton.interactable = false;
            stopButton.interactable = true;
        }

        await xmlWriter.AttemptCustomSave(this, XMLTimers.TimerAction.Start);
        await xmlWriter.Save();
    }

    public async void StopTimer()
    {
        startime = false;

        if (tablet)
        {
            startButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
        }

        else
        {
            startButton.interactable = true;
            stopButton.interactable = false;
        }

        await xmlWriter.AttemptCustomSave(this, XMLTimers.TimerAction.Stop);
        await xmlWriter.Save();
    }

    public void SelectTimer()
    {
        if (!selected)
        {
            addCustomTimer.selectedTimer = this.gameObject;
            addCustomTimer.curTimer = new NewTimer();
            addCustomTimer.curTimer.title = titleText.text;
            addCustomTimer.curTimer.boolean = isStopWatch ? "TRUE" : "FALSE";
            addCustomTimer.curTimer.initialTime = (initalHours.ToString("00") + ":" + initalMinutes.ToString("00") + ":" +
                Mathf.Floor(initialSeconds).ToString("00"));
            addCustomTimer.curTimer.alert1 = (alert1Time.alertHours.ToString("00") + ":" + alert1Time.alertMinutes.ToString("00") + ":" +
                Mathf.Floor(alert1Time.alertSeconds).ToString("00"));
            addCustomTimer.curTimer.alert2 = (alert2Time.alertHours.ToString("00") + ":" + alert2Time.alertMinutes.ToString("00") + ":" +
                Mathf.Floor(alert2Time.alertSeconds).ToString("00"));
            addCustomTimer.curTimer.alert3 = (alert3Time.alertHours.ToString("00") + ":" + alert3Time.alertMinutes.ToString("00") + ":" +
                Mathf.Floor(alert3Time.alertSeconds).ToString("00"));
            titleText.color = selectedColor;

            if (isStopWatch)
            {
                addCustomTimer.EditStopWatch();
            }

            else
            {
                addCustomTimer.EditTimer();
            }

            textBGImage.enabled = true;
            selected = true;
        }

        else
        {
            addCustomTimer.selectedTimer = null;
            addCustomTimer.curTimer = new NewTimer();
            titleText.color = deselectedColor;
            textBGImage.enabled = false;
            selected = false;
        }
    }

    private void SetUpButtons()
    {
        startButton = transform.Find("Buttons").Find("StartButton").GetComponent<Button>();
        stopButton = transform.Find("Buttons").Find("StopButton").GetComponent<Button>();
        resetButton = transform.Find("Buttons").Find("ResetButton").GetComponent<Button>();

        if (titleText != null)
        {
            if (titleText.gameObject.GetComponent<Button>() != null)
            {
                selectButton = titleText.gameObject.GetComponent<Button>();
                selectButton.onClick.AddListener(SelectTimer);
            }
        }

        resetButtonHighlight = resetButton.GetComponent<ButtonHighlight2>();

        startButton.onClick.AddListener(StartTimer);
        stopButton.onClick.AddListener(StopTimer);
        resetButton.onClick.AddListener(ResetTime);

        if (tablet)
        {
            stopButton.gameObject.SetActive(false);
        }
    }

    public IEnumerator SetTextBG()
    {
        yield return new WaitForSeconds(0.5f);
        if (textBG != null)
        {
            textBG.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, titleText.rectTransform.rect.width + 58);
        }
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, StopWatch> { }
}

[System.Serializable]
public class AlertTime
{
    public float alertSeconds;
    public int alertMinutes;
    public int alertHours;
}
