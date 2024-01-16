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

using Zenject;

#if SCC_2_5

public class DeckDrawingSelection : MonoBehaviour 
{
    [Inject]
    AnnotationManager annotationManager;

    public Button drawButton;

    // Use this for initialization
    void Start () 
	{
        drawButton.onClick.AddListener(selectImage);
	}
	
	public void selectImage()
    {
       /*  annotationManager.parentObject = this.transform.GetChild(0).transform;
        annotationManager.parentRect = this.transform.GetChild(0).GetComponent<RectTransform>(); */
    }
}

#elif !SCC_2_5

public class DeckDrawingSelection : MonoBehaviour 
{
    public DrawOnDeck drawLine;

    public Button drawButton;

    // Use this for initialization
    void Start () 
	{
        drawLine = FindObjectOfType<DrawOnDeck>();

        drawButton.onClick.AddListener(selectImage);
	}
	
	public void selectImage()
    {
        drawLine.parentObject = this.transform.GetChild(0).transform;
        drawLine.parentRect = this.transform.GetChild(0).GetComponent<RectTransform>();
    }
}
#endif