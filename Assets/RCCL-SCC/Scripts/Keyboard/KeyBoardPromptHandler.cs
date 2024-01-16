using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardPromptHandler : MonoBehaviour
{
    public GameObject[] objsToToggle;
    public RectTransform keyboardPanel;

    public Vector2 primarySize;
    public Vector2 secondarySize;

    public enum KeyboardState
    {
        timer,
        stopwatch,
        name,
        number
    };

    public KeyboardState curState;

    public void ChangeKeyboardState(KeyboardState state)
    {
        switch(state)
        {
            case KeyboardState.timer:
                {
                    foreach(GameObject obj in objsToToggle)
                    {
                        obj.SetActive(true);
                    }

                    keyboardPanel.sizeDelta = primarySize;

                    break;
                }

            case KeyboardState.stopwatch:
                {
                    foreach (GameObject obj in objsToToggle)
                    {
                        obj.SetActive(false);
                    }

                    keyboardPanel.sizeDelta = secondarySize;

                    break;
                }

            case KeyboardState.name:
                {
                    objsToToggle[0].SetActive(true);
                    objsToToggle[1].SetActive(true);
                    objsToToggle[2].SetActive(false);
                    objsToToggle[3].SetActive(false);
                    objsToToggle[4].SetActive(true);

                    keyboardPanel.sizeDelta = primarySize;

                    break;
                }

            case KeyboardState.number:
                {
                    objsToToggle[0].SetActive(false);
                    objsToToggle[1].SetActive(false);
                    objsToToggle[2].SetActive(true);
                    objsToToggle[3].SetActive(true);
                    objsToToggle[4].SetActive(false);

                    keyboardPanel.sizeDelta = secondarySize;

                    break;
                }
        }
    }
}
