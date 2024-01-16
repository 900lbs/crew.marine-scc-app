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
using TMPro;
using System;

using Zenject;
#if(SCC_2_5)
public class SystemClock : MonoBehaviour
{
    //[Inject]
    //public EniramDataFeed networkCalls;
    public TextMeshProUGUI date;
    public TextMeshProUGUI time;

    private int minutes;
    private DateTime timeNow;

    public int month;
    public string day;
    public string curDay;
    public string year;
    public string timeString;

    public string[] months;

    // Use this for initialization
    void Start()
    {
        UpdateText();
        minutes = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        timeNow = DateTime.Now;

        if (minutes != timeNow.Minute)
        {
            UpdateText();
            minutes = timeNow.Minute;
        }

    }

    void UpdateText()
    {
        month = System.DateTime.Now.Month;

        day = System.DateTime.Now.Day.ToString("00");

        year = System.DateTime.Now.Year.ToString();

        date.text = day + " <SIZE=10><voffset=.7em>|</VOFFSET></SIZE> " + months[month] + " <SIZE=10><voffset=.7em>|</VOFFSET></SIZE> " + year;

        time.text = System.DateTime.Now.ToString("HH:mm");

        //if (NetworkClient.GetIsPlayerMasterClient())
            //networkCalls.PingData();


    }
}


#elif (!SCC_2_5)
public class SystemClock : MonoBehaviour 
{
    public TextMeshProUGUI date;
    public TextMeshProUGUI time;

    private int minutes;
    private DateTime timeNow;

    public int month;
    public string day;
    public string curDay;
    public string year;
    public string timeString;

    public Tester networkCalls;

    public string[] months;

	// Use this for initialization
	void Start () 
	{
        UpdateText();
        //networkCalls = FindObjectOfType<Tester>();
        minutes = -1;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        timeNow = DateTime.Now;

        if (minutes != timeNow.Minute)
        {
            UpdateText();
            minutes = timeNow.Minute;
        }
    }

    void UpdateText()
    {
        month = System.DateTime.Now.Month;

        day = System.DateTime.Now.Day.ToString("00");

        year = System.DateTime.Now.Year.ToString();

        date.text = day + " <SIZE=10><voffset=.7em>|</VOFFSET></SIZE> " + months[month] + " <SIZE=10><voffset=.7em>|</VOFFSET></SIZE> " + year;

        time.text = System.DateTime.Now.ToString("HH:mm");

        // if (networkCalls.initialize)
        // {
        //         networkCalls.pingData();
        // }
    }
}

#endif