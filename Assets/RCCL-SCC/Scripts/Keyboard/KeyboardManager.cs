using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class KeyboardManager : MonoBehaviour
{
    public TMP_InputField currentLine;
    public TextMeshProUGUI shown;
    public TextMeshProUGUI error;

    public bool isTitle;
    public bool isNumber;

    public string currentString;
    public string firstNum;

    public string isTitleString = "TimerTitle - InputField";

    public void ChangeInputField(TMP_InputField newField)
    {
        if(currentLine != null)
        {
            currentLine.GetComponent<InputFieldHighlight>().FrameSelected();
        }

        currentLine = newField;
        currentString = newField.text;
        currentLine.GetComponent<InputFieldHighlight>().FrameSelected();

        isTitle = currentLine.name == isTitleString ? true : false;

        if (!isTitle && !isNumber)
        {
            shown = newField.transform.Find("Text Area").transform.Find("True Text").GetComponent<TextMeshProUGUI>();
        }
    }

    public void ClearInputField()
    {
        if(currentLine != null)
        {
            currentLine.GetComponent<InputFieldHighlight>().FrameSelected();
        }

        currentLine = null;
    }

    private void Update()
    {
        if(currentLine!= null)
        {
            if(Input.anyKeyDown && !Input.GetKey(KeyCode.Backspace))
            {
                if(!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0))
                {
                    char[] charTest = Input.inputString.ToCharArray();

                    if (System.Char.IsLetterOrDigit(charTest[0]) || System.Char.IsSeparator(charTest[0]) && !System.Char.IsPunctuation(charTest[0])
                        && !System.Char.IsControl(charTest[0]) && !System.Char.IsHighSurrogate(charTest[0]))
                    {
                        ErrorTesting_Keyboard.ErrorCheck(ErrorTesting_Keyboard.errorState.NoErrors, error);
                        string val = Input.inputString.ToLower();
                        UpdateString(val);
                    }

                    else
                    {
                        ErrorTesting_Keyboard.ErrorCheck(ErrorTesting_Keyboard.errorState.InocrrectKey, error);
                        currentLine.text = currentLine.text.TrimEnd(charTest[0]);
                    }
                }
                
            }

            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if(currentLine.text != "")
                {
                    UpdateString("-");
                }

                else
                {
                    UpdateString("=");
                }
            }
        }
    }

    public void UpdateString(string letter)
    {
        if(currentLine != null)
        {
            if(isTitle)
            {
                UpdateTitle(letter);
            }

            else
            {
                UpdateTimer(letter);
            }
        }
    }

    void UpdateTitle(string letter)
    {
        if (letter == "-")
        {
            currentString = currentString.Length > 0 ? currentString.Remove(currentString.Length - 1) : "";
        }

        else if (letter == "=")
        {
            currentString = "";
        }

        else
        {
            if(currentString.Length < 16)
            {
                currentString += letter;
            }
            
        }

        currentLine.text = currentString;
    }

    void UpdateTimer(string letter)
    {
        if (!isNumber)
        {
            if (letter == "-" && currentString.Length > 0)
            {
                currentString = currentString.Remove(currentString.Length - 1);
            }

            else if (letter == "=")
            {
                currentString = "";
            }

            char[] charTest = letter.ToCharArray();

            if (System.Char.IsDigit(charTest[0]) && currentString.Length < 6)
            {
                currentString += letter;
            }

            else if(System.Char.IsDigit(charTest[0]) && currentString.Contains(":"))
            {
                currentString = "";
                currentString += letter;
            }
            
            currentLine.text = FormatTimer(currentString);
            shown.text = currentLine.text;
        }
        
        if(isNumber)
        {
            if (letter == "-" && currentString.Length > 0)
            {
                currentString = currentString.Remove(currentString.Length - 1);
            }

            else if (letter == "=")
            {
                currentString = "";
            }

            else
            {
                char[] charTest = letter.ToCharArray();

                if(System.Char.IsDigit(charTest[0]) && currentString.Length < 2)
                {
                    currentString += letter;
                    
                }
            }

            currentLine.text = currentString;
        }
    }

    string FormatTimer(string current)
    {
        if(current != "")
        {
            firstNum = current[0].ToString();
        }

        char[] numbers = current.ToCharArray();

        string timerString = "";

        if(numbers.Length <= 6 && numbers.Length > 0)
        {
            timerString = new string(numbers);

            for(int i =0; i < 6 - numbers.Length; i++)
            {
                timerString = timerString.Insert(0, "0");
            }
            
            timerString = timerString.Insert(2, ":");
            timerString = timerString.Insert(5, ":");

            timerString = timerString.Insert(timerString.IndexOf(firstNum), "<color=#00FFED>");
        }

        return timerString;
    }
}
