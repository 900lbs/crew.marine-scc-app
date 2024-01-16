using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewTimerColor : MonoBehaviour
{
    public TextMeshProUGUI[] hoursChildren;
    public TextMeshProUGUI[] minsChildren;
    public TextMeshProUGUI[] secsChildren;

    public Transform hoursHolder;
    public Transform minsHolder;
    public Transform secsHolder;

    public Color colorToBe;
    // Use this for initialization
    void Start ()
    {
        hoursChildren = hoursHolder.GetComponentsInChildren<TextMeshProUGUI>();
        minsChildren = minsHolder.GetComponentsInChildren<TextMeshProUGUI>();
        secsChildren = secsHolder.GetComponentsInChildren<TextMeshProUGUI>();

        ChangeColors(colorToBe);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeColors(Color newColor)
    {
        foreach (TextMeshProUGUI tmp in hoursChildren)
        {
            tmp.color = newColor;
        }

        foreach (TextMeshProUGUI tmp in minsChildren)
        {
            tmp.color = newColor;
        }

        foreach (TextMeshProUGUI tmp in secsChildren)
        {
            tmp.color = newColor;
        }
    }
    }
