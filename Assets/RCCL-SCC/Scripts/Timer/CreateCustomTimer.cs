using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;
using DG.Tweening;

public class CreateCustomTimer : MonoBehaviour
{
    public string fileName;

    public string file;

    private List<string> lines;
    private string[] parts;

    public GameObject newCustomTimer;

    public Transform customTimerParent;

    private ContentSizeFitter contentSizeFitter;

    public GameObject addAlertPanel;

    [HideInInspector]
    public GameObject selectedTimer;

    public ScrollMenuButton scrollMenuButton;

    private NewTimer newTimer;

    private bool isStopWatch;

    [HideInInspector]
    public bool timersMaxed;

    public Button saveButton;
    public Button deleteButton;
    public Button editTimerButton;
    public Button editStopWatchButton;
    public Button leftScrollButton;

    public Image editTimerImage;
    public Image saveButtonFrame;

    public TextMeshProUGUI saveTitle;

    public TMP_InputField newTitle;

    public VerticalScrollSnap hoursScrollerTimer;
    public VerticalScrollSnap minsScrollerTimer;
    public VerticalScrollSnap secsScrollerTimer;

    public VerticalScrollSnap hoursScrollerAlert1;
    public VerticalScrollSnap minsScrollerAlert1;
    public VerticalScrollSnap secsScrollerAlert1;

    public VerticalScrollSnap hoursScrollerAlert2;
    public VerticalScrollSnap minsScrollerAlert2;
    public VerticalScrollSnap secsScrollerAlert2;

    public VerticalScrollSnap hoursScrollerAlert3;
    public VerticalScrollSnap minsScrollerAlert3;
    public VerticalScrollSnap secsScrollerAlert3;

    private int hoursTimer;
    private int minsTimer;
    private int secsTimer;

    private int hoursAlert1;
    private int minsAlert1;
    private int secsAlert1;

    private int hoursAlert2;
    private int minsAlert2;
    private int secsAlert2;

    private int hoursAlert3;
    private int minsAlert3;
    private int secsAlert3;

    public string title;

    private string boolean;

    public Color activeColor;
    public Color inactiveColor;

    public NewTimerToggle newTimerToggle;

    // Use this for initialization
    void Start ()
    {
        contentSizeFitter = this.GetComponent<ContentSizeFitter>();
        newTimerToggle = FindObjectOfType<NewTimerToggle>();
        LoadCustomTimers();

        editTimerButton.onClick.AddListener(EditTimer);
        editStopWatchButton.onClick.AddListener(EditStopWatch);
        saveButton.onClick.AddListener(SaveNewTimer);
        deleteButton.onClick.AddListener(DeleteTimer);
        leftScrollButton.onClick.AddListener(DeSelectTimer);
        leftScrollButton.onClick.AddListener(ResetValues);
        

        newTimer = new NewTimer();
	}
	
	public void LoadCustomTimers()
    {
        file = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/CustomTimers.csv"); //Resources.Load(fileName).ToString();
        
        lines = file.Split('\n').ToList();

        for(int i = 0; i < lines.Count; i++)
        {
            if(lines[i] == "")
            {
                lines.Remove(lines[i]);
            }

            else
            {
                parts = lines[i].Split(","[0]);

                BuildNewTimer();
            }
        }
    }

    public void PopulateNewTimer(string[] info, StopWatch stopWatch)
    {
        stopWatch.titleText.text = info[0];

        if (info[1] == "TRUE")
        {
            stopWatch.isStopWatch = true;
        }

        else
        {
            stopWatch.isStopWatch = false;
        }

        string[] startTime = parts[2].Split(":"[0]);

        stopWatch.hou = int.Parse(startTime[0]);
        stopWatch.min = int.Parse(startTime[1]);
        stopWatch.sec = float.Parse(startTime[2]);

        string[] alert1 = parts[3].Split(":"[0]);

        stopWatch.alert1Time.alertHours = int.Parse(alert1[0]);
        stopWatch.alert1Time.alertMinutes = int.Parse(alert1[1]);
        stopWatch.alert1Time.alertSeconds = float.Parse(alert1[2]);

        string[] alert2 = parts[4].Split(":"[0]);

        stopWatch.alert2Time.alertHours = int.Parse(alert2[0]);
        stopWatch.alert2Time.alertMinutes = int.Parse(alert2[1]);
        stopWatch.alert2Time.alertSeconds = float.Parse(alert2[2]);

        string[] alert3 = parts[5].Split(":"[0]);

        stopWatch.alert3Time.alertHours = int.Parse(alert3[0]);
        stopWatch.alert3Time.alertMinutes = int.Parse(alert3[1]);
        stopWatch.alert3Time.alertSeconds = float.Parse(alert3[2]);

        //stopWatch.addCustomTimer = this;

        StartCoroutine(RefreshList());
    }

    public void UpdateTitle()
    {
        title = newTitle.text;

        if(newTitle.text != "")
        {
            saveButtonFrame.color = activeColor;
            saveTitle.color = activeColor;
            saveButton.interactable = true;
        }

        else
        {
            saveButtonFrame.color = inactiveColor;
            saveTitle.color = inactiveColor;
            saveButton.interactable = false;
        }
    }

    public void UpdateNewTimer()
    {
        hoursTimer = hoursScrollerTimer.CurrentPage;
        minsTimer = minsScrollerTimer.CurrentPage;
        secsTimer = secsScrollerTimer.CurrentPage;

        if(secsTimer > 0 || minsTimer > 0 || hoursTimer > 0)
        {
            editTimerButton.interactable = true;
            editTimerImage.DOColor(activeColor, 0.5f);
        }

        else
        {
            editTimerButton.interactable = false;
            editTimerImage.DOColor(inactiveColor, 0.5f);
        }
    }

    public void UpdateNewAlert1()
    {
        hoursAlert1 = hoursScrollerAlert1.CurrentPage;
        minsAlert1 = minsScrollerAlert1.CurrentPage;
        secsAlert1 = secsScrollerAlert1.CurrentPage;
    }

    public void UpdateNewAlert2()
    {
        hoursAlert2 = hoursScrollerAlert2.CurrentPage;
        minsAlert2 = minsScrollerAlert2.CurrentPage;
        secsAlert2 = secsScrollerAlert2.CurrentPage;
    }

    public void UpdateNewAlert3()
    {
        hoursAlert3 = hoursScrollerAlert3.CurrentPage;
        minsAlert3 = minsScrollerAlert3.CurrentPage;
        secsAlert3 = secsScrollerAlert3.CurrentPage;
    }

    public void ResetValues()
    {
        title = "";
        newTitle.text = "";
        hoursTimer = 0;
        hoursScrollerTimer.ChangePage(0);
        minsTimer = 0;
        minsScrollerTimer.ChangePage(0);
        secsTimer = 0;
        secsScrollerTimer.ChangePage(0);
        hoursAlert1 = 0;
        hoursScrollerAlert1.ChangePage(0);
        minsAlert1 = 0;
        minsScrollerAlert1.ChangePage(0);
        secsAlert1 = 0;
        secsScrollerAlert1.ChangePage(0);
        hoursAlert2 = 0;
        hoursScrollerAlert2.ChangePage(0);
        minsAlert2 = 0;
        minsScrollerAlert2.ChangePage(0);
        secsAlert2 = 0;
        secsScrollerAlert2.ChangePage(0);
        hoursAlert3 = 0;
        hoursScrollerAlert3.ChangePage(0);
        minsAlert3 = 0;
        minsScrollerAlert3.ChangePage(0);
        secsAlert3 = 0;
        secsScrollerAlert3.ChangePage(0);

        selectedTimer = null;
    }

    public void SaveNewTimer()
    {
        foreach(string line in lines)
        {
            if(line.Contains(title))
            {
                title = title + " 1";
            }
        }

        if(isStopWatch)
        {
            newTimer.title = title;
            newTimer.boolean = "TRUE";
            newTimer.initialTime = "0:00:00";
            newTimer.alert1 = "0:00:00";
            newTimer.alert2 = "0:00:00";
            newTimer.alert3 = "0:00:00";
        }

        else
        {
            newTimer.title = title;
            newTimer.boolean = "FALSE";
            newTimer.initialTime = hoursTimer.ToString("00") + ":" + minsTimer.ToString("00") + ":" + secsTimer.ToString("00");
            newTimer.alert1 = hoursAlert1.ToString("00") + ":" + minsAlert1.ToString("00") + ":" + secsAlert1.ToString("00");
            newTimer.alert2 = hoursAlert2.ToString("00") + ":" + minsAlert2.ToString("00") + ":" + secsAlert2.ToString("00");
            newTimer.alert3 = hoursAlert3.ToString("00") + ":" + minsAlert3.ToString("00") + ":" + secsAlert3.ToString("00");
        }

        lines.Add(newTimer.title + "," + newTimer.boolean + "," + newTimer.initialTime + "," + newTimer.alert1 + "," + newTimer.alert2
            + "," + newTimer.alert3);

        parts = lines[lines.Count - 1].Split(","[0]);

        BuildNewTimer();

        scrollMenuButton.LeftScroll();

        WriteTimerToCSV();
    }

    public void BuildNewTimer()
    {
        GameObject customTimer = Instantiate(newCustomTimer, customTimerParent);

        if (customTimerParent.childCount <= 7)
        {
            customTimerParent.GetComponent<GridLayoutGroup>().constraintCount = customTimerParent.childCount;
        }

        PopulateNewTimer(parts, customTimer.GetComponent<StopWatch>());
    }

    public void DeleteTimer()
    {
        if(selectedTimer != null)
        {
            string lineToFind = selectedTimer.name;

            for(int i = 0; i < lines.Count; i++)
            {
                if(lines[i].Contains(lineToFind))
                {
                    Destroy(selectedTimer);
                    selectedTimer = null;
                    lines.Remove(lines[i]);
                    ResetValues();
                    StartCoroutine(RefreshList());
                    break;
                }
            }
        }

        else
        {
            ResetValues();
        }

        scrollMenuButton.LeftScroll();
        WriteTimerToCSV();
    }

    IEnumerator RefreshList()
    {
        
        customTimerParent.gameObject.SetActive(false);
        contentSizeFitter.enabled = false;
        yield return new WaitForEndOfFrame();
        customTimerParent.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        if (customTimerParent.childCount <= 7)
        {
            customTimerParent.GetComponent<GridLayoutGroup>().constraintCount = customTimerParent.childCount;
        }

        yield return new WaitForEndOfFrame();
        contentSizeFitter.enabled = true;

        if (customTimerParent.childCount == 14)
        {
            newTimerToggle.createNewTimerButton.interactable = false;
            editStopWatchButton.interactable = false;
        }

        else
        {
            newTimerToggle.createNewTimerButton.interactable = true;
            editStopWatchButton.interactable = true;
        }
    }

    public void EditTimer()
    {
        isStopWatch = false;
        scrollMenuButton.RightScroll();
        addAlertPanel.SetActive(true);
        newTitle.enabled = true;
        newTitle.text = "";

        if (!saveButton.gameObject.activeSelf)
        {
            saveButton.gameObject.SetActive(true);
        }
    }

    public void EditStopWatch()
    {
        isStopWatch = true;
        scrollMenuButton.RightScroll();
        addAlertPanel.SetActive(false);
        newTitle.enabled = true;
        newTitle.text = "";

        if (!saveButton.gameObject.activeSelf)
        {
            saveButton.gameObject.SetActive(true);
        }
    }

    public void DeSelectTimer()
    {
        if(selectedTimer != null)
        {
            selectedTimer.GetComponent<StopWatch>().SelectTimer();
        }
    }

    void WriteTimerToCSV()
    {
        string filePath = GetPath();

        StreamWriter writer = new StreamWriter(filePath);

        for(int i = 0; i < lines.Count; i++)
        {
            if(lines[i] != "")
            {
                writer.WriteLine(lines[i]);
            }
        }

        writer.Flush();

        writer.Close();

        ResetValues();
    }

    private string GetPath()
    {
        return Application.streamingAssetsPath + "/CustomTimers.csv";
    }
}

//[System.Serializable]
//public class NewTimer
//{
//    public string title;
//    public string boolean;
//    public string initialTime;
//    public string alert1;
//    public string alert2;
//    public string alert3;
//}
