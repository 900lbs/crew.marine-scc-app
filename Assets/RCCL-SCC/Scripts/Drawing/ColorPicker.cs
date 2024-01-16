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

#if(SCC_2_5)
using Zenject;

public class ColorPicker : UI_Button 
{
    private Image img;

    public Image selBracket;

    public Color color;

    AnnotationManager annotationManager;
    
    [Inject]
    public void Construct(AnnotationManager anonMan)
    {
        annotationManager = anonMan;
    }

    protected override void OnStateChange()
    {

    }

/*     protected override void UIManager_OnInitialize(){}
 */
	// Use this for initialization
	void Start () 
	{
        annotationManager.selBrackets.Add(selBracket);

        img = this.GetComponent<Image>();

        color = img.color;

        this.gameObject.name = color.ToString();

        button.onClick.AddListener(changeColor);
	}

    public void changeColor()
    {
        annotationManager.curSelBracket = selBracket;
        annotationManager.SetCurrentLineColor(color);
    }
}

#elif(!SCC_2_5)

public class ColorPicker : MonoBehaviour 
{
    private Image img;

    private Button button;

    public Color color;

    public DrawOnDeck drawOnDeck;

	// Use this for initialization
	void Start () 
	{
        drawOnDeck = FindObjectOfType<DrawOnDeck>();

        img = this.GetComponent<Image>();

        button = this.GetComponent<Button>();

        color = img.color;

        this.gameObject.name = color.ToString();

        button.onClick.AddListener(changeColor);
	}

    public void changeColor()
    {
        drawOnDeck.lineCol = color;
    }
}

#endif