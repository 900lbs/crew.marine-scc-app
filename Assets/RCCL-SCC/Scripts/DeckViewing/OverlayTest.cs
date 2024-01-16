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

public class OverlayTest : MonoBehaviour 
{
    public GameObject[] safetyOverlays;
    public GameObject[] doorOverlays;
    public GameObject[] assemblyOverlays;
    public GameObject[] damageControlPlanOverlays;
    public GameObject[] escapeRoutesOverlays;
    public GameObject[] hatchesOverlays;
    public GameObject[] structuralFireProtectionOverlays;
    public GameObject[] waterTightOverlays;
    public GameObject[] fireControlOverlays;

    public Button safetyButton;
    public Button doorButton;
    public Button assemblyButton;
    public Button damageButton;
    public Button escapeButton;
    public Button hatchesButton;
    public Button structureButton;
    public Button waterButton;
    public Button fireButton;

    private void Start()
    {
        ToggleOverlays(safetyOverlays);
        ToggleOverlays(doorOverlays);
        ToggleOverlays(assemblyOverlays);
        ToggleOverlays(damageControlPlanOverlays);
        ToggleOverlays(escapeRoutesOverlays);
        ToggleOverlays(hatchesOverlays);
        ToggleOverlays(structuralFireProtectionOverlays);
        ToggleOverlays(waterTightOverlays);
        ToggleOverlays(fireControlOverlays);

        safetyButton.onClick.AddListener(delegate
        {
            ToggleOverlays(safetyOverlays);
        });

        doorButton.onClick.AddListener(delegate
        {
            ToggleOverlays(doorOverlays);
        });

        assemblyButton.onClick.AddListener(delegate
        {
            ToggleOverlays(assemblyOverlays);
        });

        damageButton.onClick.AddListener(delegate
        {
            ToggleOverlays(damageControlPlanOverlays);
        });

        escapeButton.onClick.AddListener(delegate
        {
            ToggleOverlays(escapeRoutesOverlays);
        });

        hatchesButton.onClick.AddListener(delegate
        {
            ToggleOverlays(hatchesOverlays);
        });

        structureButton.onClick.AddListener(delegate
        {
            ToggleOverlays(structuralFireProtectionOverlays);
        });

        waterButton.onClick.AddListener(delegate
        {
            ToggleOverlays(waterTightOverlays);
        });

        fireButton.onClick.AddListener(delegate
        {
            ToggleOverlays(fireControlOverlays);
        });
    }

    public void ToggleOverlays(GameObject[] overlays)
    {
        foreach(GameObject go in overlays)
        {
            go.SetActive(!go.activeSelf);
        }
    }
}
