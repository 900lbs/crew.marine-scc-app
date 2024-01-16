using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Zenject;
#if SCC_2_5
public class AddCustomTimer : MonoBehaviour
{
    StopWatch.Factory stopwatchFactory;

    [Inject]
    void Construct(StopWatch.Factory stopwatchFact)
    {
        stopwatchFactory = stopwatchFact;
    }

    //Variables for loading timers from CSV
    public string fileName;
    public string file;
    private List<string> lines;
    private string[] parts;

    //Prefab for new timer
    public GameObject newCustomTimer;

    //Gameobject that holds the keyboard
    public GameObject keyboardPrompt;

    //KeyboardManager
    public KeyboardManager keyboardManager;

    //Transform to assign the new timer to
    public Transform customTimerParent;

    //Used for adding new timers, make sure the add the content size fitter form the custom timer widget
    public ContentSizeFitter contentSizeFitter;

    //Variable for timer we are editing
//    [HideInInspector]
    public GameObject selectedTimer;

    //Current NewTimer class we are using to edit/create
    public NewTimer curTimer;

    //Fields we are gathering info from
    public TMP_InputField title;
    public TMP_InputField timerValue;
    public TMP_InputField alert1;
    public TMP_InputField alert2;
    public TMP_InputField alert3;

    //Error Message Text Holder
    public TextMeshProUGUI error;

    //Is this timer a stopwatch?
    private bool isStopWatch;

    //Is this component used for the Tablet version?
    public bool isTablet;

    //Delete button
    public Button deleteButton;

    public NewTimerToggle newTimerToggle;
    public NewTimerToggle newStopwatchToggle;

    // Use this for initialization
    void Start()
    {
        LoadCustomTimers();
    }

    public void LoadCustomTimers()
    {
        file = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/CustomTimers.csv"); //Resources.Load(fileName).ToString();

        lines = file.Split('\n').ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == "")
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

        if(!isTablet)
        {
            StartCoroutine(RefreshList());
        }
        
    }

    public void MakeNewTimer(bool stopWatch)
    {
        curTimer = new NewTimer();

        curTimer.boolean = stopWatch ? "TRUE" : "FALSE";
    }

    public void UpdateTitle()
    {
        if(curTimer != null)
        {
            curTimer.title = title.text;
        }
        
    }

    public void UpdateNewTimer()
    {
        if (curTimer != null)
        {
            curTimer.initialTime = timerValue.text.Replace("<color=#00FFED>", "");
        }
    }

    public void UpdateNewAlert1()
    {
        if (curTimer != null)
        {
            curTimer.alert1 = alert1.text.Replace("<color=#00FFED>", "");
        }
    }

    public void UpdateNewAlert2()
    {
        if (curTimer != null)
        {
            curTimer.alert2 = alert2.text.Replace("<color=#00FFED>", "");
        }
    }

    public void UpdateNewAlert3()
    {
        if (curTimer != null)
        {
            curTimer.alert3 = alert3.text.Replace("<color=#00FFED>", "");
        }
    }

    public void ResetValues()
    {
        selectedTimer = null;
        curTimer = new NewTimer();

        title.text = "";
        timerValue.text = "";
        alert1.text = "";
        alert2.text = "";
        alert3.text = "";

        HighlightTime(timerValue);
        HighlightTime(alert1);
        HighlightTime(alert2);
        HighlightTime(alert3);
    }

    public void SaveNewTimer(NewTimer newTimer)
    {
        if(selectedTimer != null)
        {
            DeleteTimer();
        }

        foreach (string line in lines)
        {
            if (line.Contains(newTimer.title))
            {
                newTimer.title = newTimer.title + " 1";
            }
        }

        lines.Add(newTimer.title + "," + newTimer.boolean + "," + newTimer.initialTime + "," + newTimer.alert1 + "," + newTimer.alert2
            + "," + newTimer.alert3);

        parts = lines[lines.Count - 1].Split(","[0]);

        BuildNewTimer();
    
        WriteTimerToCSV();
    }

    public void BuildNewTimer()
    {
        GameObject customTimer = stopwatchFactory.Create(newCustomTimer).gameObject;
        customTimer.transform.SetParent(customTimerParent);
        customTimer.transform.localScale = Vector3.one;
        if (customTimerParent.childCount <= 7 && !isTablet)
        {
            customTimerParent.GetComponent<GridLayoutGroup>().constraintCount = customTimerParent.childCount;
        }

        PopulateNewTimer(parts, customTimer.GetComponent<StopWatch>());
    }

    public void DeleteTimer()
    {
        if (selectedTimer != null)
        {
            string lineToFind = selectedTimer.name;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(lineToFind))
                {
                    Destroy(selectedTimer);
                    selectedTimer = null;
                    lines.Remove(lines[i]);
                    StartCoroutine(RefreshList());
                    break;
                }
            }
        }

        else
        {
            ResetValues();
        }

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
            newStopwatchToggle.createNewTimerButton.interactable = false;
        }

        else
        {
            newTimerToggle.createNewTimerButton.interactable = true;
            newStopwatchToggle.createNewTimerButton.interactable = true;
        }
    }

    public void EditTimer()
    {
        if(selectedTimer != null)
        {
            keyboardPrompt.SetActive(true);

            newTimerToggle.keyboardPrompt.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.timer);

            StartCoroutine(ToggleInputField());

            deleteButton.gameObject.SetActive(true);

            title.text = curTimer.title;
            timerValue.text = curTimer.initialTime;
            alert1.text = curTimer.alert1;
            alert2.text = curTimer.alert2;
            alert3.text = curTimer.alert3;
            HighlightTime(timerValue);
            HighlightTime(alert1);
            HighlightTime(alert2);
            HighlightTime(alert3);
        }
    }

    public void EditStopWatch()
    {
        if (selectedTimer != null)
        {
            keyboardPrompt.SetActive(true);

            newTimerToggle.keyboardPrompt.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.stopwatch);

            StartCoroutine(ToggleInputField());

            deleteButton.gameObject.SetActive(true);

            title.text = curTimer.title;
        }
    }

    IEnumerator ToggleInputField()
    {
        yield return new WaitForEndOfFrame();
        keyboardManager.ChangeInputField(title);
    }

    void HighlightTime(TMP_InputField field)
    {
        string formatStr = field.text.ToString();

        char[] charTest = formatStr.ToCharArray();

        int breakpoint = 0;

        foreach(char character in charTest)
        {
            if(character.ToString() == "0" || character.ToString() == ":")
            {
                breakpoint++;
            }

            else
            {
                break;
            }
        }

        formatStr = formatStr.Insert(breakpoint, "<color=#00FFED>");

        field.transform.Find("Text Area").transform.Find("True Text").GetComponent<TextMeshProUGUI>().text = formatStr;
    }

    public void DeSelectTimer()
    {
        if (selectedTimer != null)
        {
            selectedTimer.GetComponent<StopWatch>().SelectTimer();
            deleteButton.gameObject.SetActive(false);
        }

        ResetValues();
    }

    void WriteTimerToCSV()
    {
        string filePath = GetPath();

        StreamWriter writer = new StreamWriter(filePath);

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] != "")
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

[System.Serializable]
public class NewTimer
{
    public string title;
    public string boolean;
    public string initialTime = "00:00:00";
    public string alert1 = "00:00:00";
    public string alert2 = "00:00:00";
    public string alert3 = "00:00:00";
}

#elif !SCC_2_5
public class AddCustomTimer : MonoBehaviour
{
    //Variables for loading timers from CSV
    public string fileName;
    public string file;
    private List<string> lines;
    private string[] parts;

    //Prefab for new timer
    public GameObject newCustomTimer;

    //Gameobject that holds the keyboard
    public GameObject keyboardPrompt;

    //KeyboardManager
    public KeyboardManager keyboardManager;

    //Transform to assign the new timer to
    public Transform customTimerParent;

    //Used for adding new timers, make sure the add the content size fitter form the custom timer widget
    public ContentSizeFitter contentSizeFitter;

    //Variable for timer we are editing
//    [HideInInspector]
    public GameObject selectedTimer;

    //Current NewTimer class we are using to edit/create
    public NewTimer curTimer;

    //Fields we are gathering info from
    public TMP_InputField title;
    public TMP_InputField timerValue;
    public TMP_InputField alert1;
    public TMP_InputField alert2;
    public TMP_InputField alert3;

    //Error Message Text Holder
    public TextMeshProUGUI error;

    //Is this timer a stopwatch?
    private bool isStopWatch;

    //Is this component used for the Tablet version?
    public bool isTablet;

    //Delete button
    public Button deleteButton;

    public NewTimerToggle newTimerToggle;
    public NewTimerToggle newStopwatchToggle;

    // Use this for initialization
    void Start()
    {
        LoadCustomTimers();
    }

    public void LoadCustomTimers()
    {
        file = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/CustomTimers.csv"); //Resources.Load(fileName).ToString();

        lines = file.Split('\n').ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == "")
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

        if(!isTablet)
        {
            StartCoroutine(RefreshList());
        }
        
    }

    public void MakeNewTimer(bool stopWatch)
    {
        curTimer = new NewTimer();

        curTimer.boolean = stopWatch ? "TRUE" : "FALSE";
    }

    public void UpdateTitle()
    {
        if(curTimer != null)
        {
            curTimer.title = title.text;
        }
        
    }

    public void UpdateNewTimer()
    {
        if (curTimer != null)
        {
            curTimer.initialTime = timerValue.text.Replace("<color=#00FFED>", "");
        }
    }

    public void UpdateNewAlert1()
    {
        if (curTimer != null)
        {
            curTimer.alert1 = alert1.text.Replace("<color=#00FFED>", "");
        }
    }

    public void UpdateNewAlert2()
    {
        if (curTimer != null)
        {
            curTimer.alert2 = alert2.text.Replace("<color=#00FFED>", "");
        }
    }

    public void UpdateNewAlert3()
    {
        if (curTimer != null)
        {
            curTimer.alert3 = alert3.text.Replace("<color=#00FFED>", "");
        }
    }

    public void ResetValues()
    {
        selectedTimer = null;
        curTimer = new NewTimer();

        title.text = "";
        timerValue.text = "";
        alert1.text = "";
        alert2.text = "";
        alert3.text = "";

        HighlightTime(timerValue);
        HighlightTime(alert1);
        HighlightTime(alert2);
        HighlightTime(alert3);
    }

    public void SaveNewTimer(NewTimer newTimer)
    {
        if(selectedTimer != null)
        {
            DeleteTimer();
        }

        foreach (string line in lines)
        {
            if (line.Contains(newTimer.title))
            {
                newTimer.title = newTimer.title + " 1";
            }
        }

        lines.Add(newTimer.title + "," + newTimer.boolean + "," + newTimer.initialTime + "," + newTimer.alert1 + "," + newTimer.alert2
            + "," + newTimer.alert3);

        parts = lines[lines.Count - 1].Split(","[0]);

        BuildNewTimer();
    
        WriteTimerToCSV();
    }

    public void BuildNewTimer()
    {
        GameObject customTimer = Instantiate(newCustomTimer, customTimerParent);

        if (customTimerParent.childCount <= 7 && !isTablet)
        {
            customTimerParent.GetComponent<GridLayoutGroup>().constraintCount = customTimerParent.childCount;
        }

        PopulateNewTimer(parts, customTimer.GetComponent<StopWatch>());
    }

    public void DeleteTimer()
    {
        if (selectedTimer != null)
        {
            string lineToFind = selectedTimer.name;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(lineToFind))
                {
                    Destroy(selectedTimer);
                    selectedTimer = null;
                    lines.Remove(lines[i]);
                    StartCoroutine(RefreshList());
                    break;
                }
            }
        }

        else
        {
            ResetValues();
        }

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
            newStopwatchToggle.createNewTimerButton.interactable = false;
        }

        else
        {
            newTimerToggle.createNewTimerButton.interactable = true;
            newStopwatchToggle.createNewTimerButton.interactable = true;
        }
    }

    public void EditTimer()
    {
        if(selectedTimer != null)
        {
            keyboardPrompt.SetActive(true);

            newTimerToggle.keyboardPrompt.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.timer);

            StartCoroutine(ToggleInputField());

            deleteButton.gameObject.SetActive(true);

            title.text = curTimer.title;
            timerValue.text = curTimer.initialTime;
            alert1.text = curTimer.alert1;
            alert2.text = curTimer.alert2;
            alert3.text = curTimer.alert3;
            HighlightTime(timerValue);
            HighlightTime(alert1);
            HighlightTime(alert2);
            HighlightTime(alert3);
        }
    }

    public void EditStopWatch()
    {
        if (selectedTimer != null)
        {
            keyboardPrompt.SetActive(true);

            newTimerToggle.keyboardPrompt.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.stopwatch);

            StartCoroutine(ToggleInputField());

            deleteButton.gameObject.SetActive(true);

            title.text = curTimer.title;
        }
    }

    IEnumerator ToggleInputField()
    {
        yield return new WaitForEndOfFrame();
        keyboardManager.ChangeInputField(title);
    }

    void HighlightTime(TMP_InputField field)
    {
        string formatStr = field.text.ToString();

        char[] charTest = formatStr.ToCharArray();

        int breakpoint = 0;

        foreach(char character in charTest)
        {
            if(character.ToString() == "0" || character.ToString() == ":")
            {
                breakpoint++;
            }

            else
            {
                break;
            }
        }

        formatStr = formatStr.Insert(breakpoint, "<color=#00FFED>");

        field.transform.Find("Text Area").transform.Find("True Text").GetComponent<TextMeshProUGUI>().text = formatStr;
    }

    public void DeSelectTimer()
    {
        if (selectedTimer != null)
        {
            selectedTimer.GetComponent<StopWatch>().SelectTimer();
            deleteButton.gameObject.SetActive(false);
        }

        ResetValues();
    }

    void WriteTimerToCSV()
    {
        string filePath = GetPath();

        StreamWriter writer = new StreamWriter(filePath);

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] != "")
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

[System.Serializable]
public class NewTimer
{
    public string title;
    public string boolean;
    public string initialTime = "00:00:00";
    public string alert1 = "00:00:00";
    public string alert2 = "00:00:00";
    public string alert3 = "00:00:00";
}

#endif