using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if SCC_2_5
public class NewTimerToggle : MonoBehaviour
{
    public TextMeshProUGUI titleButtonText;
    public TMP_InputField defaultField;

    public HideTimerWidget customTimerWidget;
    public ToggleSwitch timerWidgetToggle;
    AddCustomTimer addCustomTimer;
    public KeyBoardPromptHandler keyboardPrompt;
    KeyboardManager keyboardManager;

    public Button createNewTimerButton;


    public bool isStopWatch;


	// Use this for initialization
	void Awake ()
    {
        addCustomTimer = FindObjectOfType<AddCustomTimer>();
        keyboardManager = keyboardPrompt.GetComponentInChildren<KeyboardManager>();

        createNewTimerButton = this.GetComponent<Button>();

        createNewTimerButton.onClick.AddListener(ToggleTimerInput);

	}

    public void ToggleTimerInput()
    {
        KeyBoardPromptHandler.KeyboardState state = isStopWatch ? KeyBoardPromptHandler.KeyboardState.stopwatch :
            KeyBoardPromptHandler.KeyboardState.timer;
    

        addCustomTimer.MakeNewTimer(isStopWatch);
        addCustomTimer.keyboardPrompt.SetActive(true);
        keyboardPrompt.ChangeKeyboardState(state);
        StartCoroutine(ToggleInputField());
    }

    IEnumerator ToggleInputField()
    {
        yield return new WaitForEndOfFrame();
        keyboardManager.ChangeInputField(defaultField);
    }
}
#elif !SCC_2_5

public class NewTimerToggle : MonoBehaviour
{
    public TextMeshProUGUI titleButtonText;
    public TMP_InputField defaultField;

    public HideTimerWidget customTimerWidget;
    public ToggleSwitch timerWidgetToggle;
    AddCustomTimer addCustomTimer;
    public KeyBoardPromptHandler keyboardPrompt;
    KeyboardManager keyboardManager;

    public Button createNewTimerButton;


    public bool isStopWatch;


	// Use this for initialization
	void Awake ()
    {
        addCustomTimer = FindObjectOfType<AddCustomTimer>();
        keyboardManager = keyboardPrompt.GetComponentInChildren<KeyboardManager>();

        createNewTimerButton = this.GetComponent<Button>();

        createNewTimerButton.onClick.AddListener(ToggleTimerInput);

	}

    public void ToggleTimerInput()
    {
        KeyBoardPromptHandler.KeyboardState state = isStopWatch ? KeyBoardPromptHandler.KeyboardState.stopwatch :
            KeyBoardPromptHandler.KeyboardState.timer;
        
        if(!timerWidgetToggle.toggled)
        {
            customTimerWidget.enableTheTimer();
            timerWidgetToggle.Toggle();
        }

        addCustomTimer.MakeNewTimer(isStopWatch);
        addCustomTimer.keyboardPrompt.SetActive(true);
        keyboardPrompt.ChangeKeyboardState(state);
        StartCoroutine(ToggleInputField());
    }

    IEnumerator ToggleInputField()
    {
        yield return new WaitForEndOfFrame();
        keyboardManager.ChangeInputField(defaultField);
    }
}

#endif