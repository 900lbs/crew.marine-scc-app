using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;
[RequireComponent(typeof(StopWatch))]
[System.Xml.Serialization.XmlType("request")]
[System.Serializable]
public class XMLTimers : xmlBase
{
    public enum TimerAction
    {
        Start,
        Stop,
        Reset
    }
    public XMLTimers() { }

    public string linkName;
    public string linkType = "o";
    public string eVar63;
    public string prop63;
    public string events;

    /// <summary>
    /// Timers custom save.
    /// </summary>
    /// <param name="parameters">[1]: StopWatch reference | [2] TimerAction enum type</param>
    public async override Task CustomSave(params object[] parameters)
    {
        try
        {
            StopWatch stopwatch = parameters[0] as StopWatch;
            TimerAction timerAction = (TimerAction)parameters[1];
            if (stopwatch != null && timerAction.GetType() != null)
            {
                linkName = stopwatch.titleText.text + "-" + timerAction.ToString();
                eVar63 = stopwatch.titleText.text + "-" + timerAction.ToString();
                prop63 = stopwatch.titleText.text + "-" + timerAction.ToString();
                events = GetCorrectEvent(timerAction);
                await new WaitForEndOfFrame();
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    string GetCorrectEvent(TimerAction timerAction)
    {
        switch (timerAction)
        {
            case TimerAction.Start:
                return "event505";

            case TimerAction.Stop:
                return "event506";

            case TimerAction.Reset:
                return "event507";

            default:
                Debug.LogError("Incorrect timer action for requested event: " + timerAction.ToString());
                return null;
        }
    }
    public new class Factory : PlaceholderFactory<XMLTimers> { }
}


