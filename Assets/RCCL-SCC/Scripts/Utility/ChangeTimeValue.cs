using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeTimeValue : MonoBehaviour
{
    public TextMeshProUGUI value;
	// Use this for initialization
	void Start ()
    {
        value = this.GetComponent<TextMeshProUGUI>();
        string tempValue = this.gameObject.name;
        string newValue = tempValue.Replace("(Clone)", "");
        value.text = newValue;

        if(this.transform.localScale.x > 1)
        {
            transform.localScale = Vector3.one;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
