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

public class ShowOverlay : MonoBehaviour 
{
    public DeckOverlay[] decks;

    public GameObject[] safetyOverlays;
    public GameObject[] doorOverlays;
    public GameObject[] assemblyOverlays;
    public GameObject[] damageControlPlanOverlays;
    public GameObject[] escapeRoutesOverlays;
    public GameObject[] hatchesOverlays;
    public GameObject[] structuralFireProtectionOverlays;
    public GameObject[] waterTightOverlays;
    public GameObject[] fireControlOverlays;

    public bool[] toggled;

    public Button safetyButton;
    public Button doorButton;
    public Button assemblyButton;
    public Button damageButton;
    public Button escapeButton;
    public Button hatchesButton;
    public Button structureButton;
    public Button waterButton;
    public Button fireButton;


	// Use this for initialization
	void Start () 
	{
        safetyButton.onClick.AddListener(delegate
        {
            ToggleOverlays(safetyOverlays, toggled[0], "safetyOverlays");
            toggled[0] = !toggled[0];
        });

        doorButton.onClick.AddListener(delegate
        {
            ToggleOverlays(doorOverlays, toggled[1], "doorOverlays");
            toggled[1] = !toggled[1];
        });

        assemblyButton.onClick.AddListener(delegate
        {
            
            ToggleOverlays(assemblyOverlays, toggled[2], "assemblyOverlays");
            toggled[2] = !toggled[2];
        });

        damageButton.onClick.AddListener(delegate
        {
            ToggleOverlays(damageControlPlanOverlays, toggled[3], "damageControlPlanOverlays");
            toggled[3] = !toggled[3];
        });

        escapeButton.onClick.AddListener(delegate
        {
            ToggleOverlays(escapeRoutesOverlays, toggled[4], "escapeRoutesOverlays");
            toggled[4] = !toggled[4];
        });

        hatchesButton.onClick.AddListener(delegate
        {
            ToggleOverlays(hatchesOverlays, toggled[5], "hatchesOverlays");
            toggled[5] = !toggled[5];
        });

        structureButton.onClick.AddListener(delegate
        {
            ToggleOverlays(structuralFireProtectionOverlays, toggled[6], "structuralFireProtectionOverlays");
            toggled[6] = !toggled[6];
        });

        waterButton.onClick.AddListener(delegate
        {
            ToggleOverlays(waterTightOverlays, toggled[7], "waterTightOverlays");
            toggled[7] = !toggled[7];
        });

        fireButton.onClick.AddListener(delegate
        {
            ToggleOverlays(fireControlOverlays, toggled[8], "fireControlOverlays");
            toggled[8] = !toggled[8];
        });

        System.Array.Reverse(safetyOverlays);
        System.Array.Reverse(doorOverlays);
        System.Array.Reverse(assemblyOverlays);
        System.Array.Reverse(damageControlPlanOverlays);
        System.Array.Reverse(escapeRoutesOverlays);
        System.Array.Reverse(hatchesOverlays);
        System.Array.Reverse(structuralFireProtectionOverlays);
        System.Array.Reverse(waterTightOverlays);
        System.Array.Reverse(fireControlOverlays);
	}

    public void ToggleOverlays(GameObject[] overlays, bool toggle, string fieldName)
    {
        if(!toggle)
        {
            int overlayNum = 0;

            for (int i = 0; i < decks.Length; i++)
            {
                bool overlayBool = (bool)decks[i].GetType().GetField(fieldName).GetValue(decks[i]);

                if (overlayBool)
                {
                    GameObject newOverlay = Instantiate(overlays[overlayNum], decks[i].transform);

                    newOverlay.name = fieldName;

                    int newIndex = decks[i].transform.Find("LineHolder").GetSiblingIndex();

                    newOverlay.transform.SetSiblingIndex(newIndex);

                    overlayNum++;
                }
            }
        }

        else
        {
            for (int i = 0; i < decks.Length; i++)
            {
                bool overlayBool = (bool)decks[i].GetType().GetField(fieldName).GetValue(decks[i]);

                if (overlayBool)
                {
                    Destroy(decks[i].transform.Find(fieldName).gameObject);
                }
            }

            Resources.UnloadUnusedAssets();
        }

    }
}
