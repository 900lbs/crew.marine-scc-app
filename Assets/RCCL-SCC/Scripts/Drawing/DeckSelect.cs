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
using TMPro;

#if !SCC_2_5
public class DeckSelect : MonoBehaviour 
{
	public bool deckSelected;

	public DrawOnDeck drawOnDeck;

    public GameObject longView;

    public Button selectionButton;
    public Image selButtonImg;
    public Image selBrackets;
    public TextMeshProUGUI number;
    public TextMeshProUGUI dashes;

    public Image background;

    public Color32 selectedCol;
    public Color32 deselectedCol;

    public Color32 selectedButtonCol;
    public Color32 selectedTextCol;

    public Color32 deSelButtonCol;
    public Color32 deSelTextCol;
    public Color32 deSelDashCol;


    private void OnEnable()
    {
        if(selectionButton != null)
        {
            selectionButton.gameObject.SetActive(true);
        }

        if(selectionButton.transform.GetSiblingIndex() != selectionButton.transform.parent.childCount - 1)
        {
            selectionButton.transform.parent.GetChild(selectionButton.transform.GetSiblingIndex() + 1).gameObject.SetActive(true);
        }

    }

    private void OnDisable()
    {
        if (selectionButton != null)
        {
            selectionButton.gameObject.SetActive(false);
        }

        if (selectionButton != null && selectionButton.transform.GetSiblingIndex() != selectionButton.transform.parent.childCount - 1)
        {
            selectionButton.transform.parent.GetChild(selectionButton.transform.GetSiblingIndex() + 1).gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start () 
	{
        background = this.GetComponent<Image>();
        selButtonImg = selectionButton.GetComponent<Image>();
        number = selButtonImg.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dashes = selButtonImg.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        selBrackets = number.transform.GetChild(0).GetComponent<Image>();
        selectionButton.onClick.AddListener(selectDeck);
	}

    private void Update()
    {
        
    }

    public void selectDeck()
    {
        deckSelected = !deckSelected;
        selectionButton.interactable = false;

        if (deckSelected && background.color != selectedCol)
        {
            background.color = selectedCol;
            selButtonImg.color = selectedButtonCol;
            number.color = selectedTextCol;
            dashes.color = selectedTextCol;
            selBrackets.enabled = true;
        }

        if (!deckSelected && background.color != deselectedCol)
        {
            background.color = deselectedCol;
            selButtonImg.color = deSelButtonCol;
            number.color = deSelTextCol;
            dashes.color = deSelDashCol;
            selBrackets.enabled = false;
        }

        if (longView != null)
        {
            longView.SetActive(deckSelected);
        }

        if(DrawOnDeck.Instance.CheckTheDecks())
        {
            DrawOnDeck.Instance.ChangeButtonColor(DrawOnDeck.Instance.activeColor, true);
        }

        if(!DrawOnDeck.Instance.CheckTheDecks())
        {
            DrawOnDeck.Instance.ChangeButtonColor(DrawOnDeck.Instance.inactiveColor, false);
        }

        StartCoroutine(ReEnableButton());
    }

    public IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(0.25F);
        selectionButton.interactable = true;
    }
}
#endif