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

#if SCC_2_5
public class HideTimerWidget : MonoBehaviour 
{
    public CanvasGroup canvasGroup;

    public WidgetToDock widgetToDock;

    public Button toggleBut;

    public bool timerOn;

	// Use this for initialization
	void Start () 
	{
        toggleBut = this.GetComponent<Button>();

        toggleBut.onClick.AddListener(enableTheTimer);

        widgetToDock = canvasGroup.transform.GetComponent<WidgetToDock>();
    }
	
    public void enableTheTimer()
    {
        timerOn = !timerOn;

        if(timerOn)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            canvasGroup.transform.SetSiblingIndex(canvasGroup.transform.parent.childCount - 1);

        }

        if (!timerOn)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
#elif !SCC_2_5
public class HideTimerWidget : MonoBehaviour 
{
    public CanvasGroup canvasGroup;

    public WidgetToDock widgetToDock;

    public Button toggleBut;

    public bool timerOn;

	// Use this for initialization
	void Start () 
	{
        toggleBut = this.GetComponent<Button>();

        toggleBut.onClick.AddListener(enableTheTimer);

        widgetToDock = canvasGroup.transform.GetComponent<WidgetToDock>();
    }
	
    public void enableTheTimer()
    {
        timerOn = !timerOn;

        if(timerOn)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            canvasGroup.transform.SetSiblingIndex(canvasGroup.transform.parent.childCount - 1);

            widgetToDock.DockWidget();
            widgetToDock.visible = true;
        }

        if (!timerOn)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            widgetToDock.UnDockWidget();
            widgetToDock.visible = false;
        }
    }
}
#endif