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

public class TextScaling : MonoBehaviour 
{
    public CanvasScaler canvasScaler;

    public RectTransform textHolder;

    Vector3 newScale;

    float startScaleX;

	// Use this for initialization
	void Start () 
	{
        canvasScaler = GameObject.FindWithTag("DeckScaler").GetComponent<CanvasScaler>();

        textHolder = this.GetComponent<RectTransform>();

        newScale = textHolder.localScale;

        startScaleX = newScale.x;
	}
	
	// Update is called once per frame
	void Update () 
	{
        //if (canvasScaler.scaleFactor >= 1 && canvasScaler.scaleFactor <= 25)
        //{
        //    newScale.x = startScaleX / canvasScaler.scaleFactor;

        //    textHolder.localScale = newScale;
        //}

        //if (canvasScaler.scaleFactor < 1)
        //{
        //    textHolder.localScale = Vector3.one;
        //}

        //if (canvasScaler.scaleFactor > 25)
        //{
        //    newScale.x = startScaleX / canvasScaler.scaleFactor;

        //    textHolder.localScale = newScale;
        //}
	}
}
