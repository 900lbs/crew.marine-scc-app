using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorTesting_Keyboard : MonoBehaviour
{
    public enum errorState
        {
            NoTitle,
            BadTimerValue,
            AlertOutOfSequence,
            InocrrectKey,
            NoErrors
        };

    public static bool ErrorCheck(errorState error, TextMeshProUGUI errorMessage)
    {
        if(errorMessage != null)
        {
            switch(error)
            {
                case errorState.NoTitle:
                    {
                        errorMessage.text = "<color=#FF0000>Error: <color=#009bc4> No timer name detected";
                        return false;
                    }

                case errorState.BadTimerValue:
                    {
                        errorMessage.text = "<color=#FF0000>Error: <color=#009bc4> Timer value is invalid";
                        return false;
                    }

                case errorState.AlertOutOfSequence:
                    {
                        errorMessage.text = "<color=#FF0000>Error: <color=#009bc4> Alert is out of sequence";
                        return false;
                    }

                case errorState.InocrrectKey:
                    {
                        errorMessage.text = "<color=#FF0000>Error: <color=#009bc4> Timers must use digits";
                        return false;
                    }

                case errorState.NoErrors:
                    {
                        errorMessage.text = "";
                        return true;
                    }

                default:
                    return false;
            }
        }

        else
        {
            return false;
        }
    }
}
