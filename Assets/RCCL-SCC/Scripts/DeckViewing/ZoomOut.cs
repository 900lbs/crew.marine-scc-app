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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using Zenject;

#if SCC_2_5
public class ZoomOut : MonoBehaviour
{
    [Inject]
    PanDeckHolder DeckMapHolder;


    public static Action<bool> OnIsZoomedIn;

    ActiveState state;
    public ActiveState State
    {
        get
        {
            return state;
        }

        set
        {
            if (state != value)
            {
                OnIsZoomedIn?.Invoke((value == ActiveState.Enabled));
                state = value;
            }
        }
    }
    public CanvasScaler canvasScaler;
    public ZoomInToDeck zoomIn;

    public float minZoom;

    public Vector3 initialZoomPosition;

    RectTransform deckMapRect;

    // Use this for initialization
    void Start()
    {
        deckMapRect = DeckMapHolder.GetComponent<RectTransform>();

        canvasScaler = GameObject.FindWithTag("DeckScaler").GetComponent<CanvasScaler>();

        initialZoomPosition = zoomIn.deckHolderPivot.anchoredPosition3D;
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasScaler.scaleFactor == minZoom &&
            State != ActiveState.Disabled &&
            deckMapRect.anchoredPosition == Vector2.zero)
        {
            State = ActiveState.Disabled;
        }

        if ((canvasScaler.scaleFactor > minZoom ||
            deckMapRect.anchoredPosition != Vector2.zero) &&
            State != ActiveState.Enabled)
        {
            State = ActiveState.Enabled;
        }
    }

    public void zoomOut()
    {
        zoomIn.stopZoom = true;
        canvasScaler.scaleFactor = minZoom;
        zoomIn.deckHolderPivot.anchoredPosition3D = initialZoomPosition;
        deckMapRect.anchoredPosition3D = Vector3.zero;
        StartCoroutine(ReEnableButton());
    }

    public IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(0.25F);
    }

    public void EnableZoomOut()
    {
        zoomOut();
    }
}

#elif !SCC_2_5
public class ZoomOut : MonoBehaviour 
{
    public CanvasScaler canvasScaler;

    public GameObject zoomButtonGO;

    public Image zoomButIcon;

    public TextMeshProUGUI zoomButtonText;

    public Color inactiveColor;
    public Color activeColor;

    public RectTransform deckMapHolder;

    public Button zoomBut;

    public ZoomInToDeck zoomIn;

    public float minZoom;

    public Vector3 initialZoomPosition;

	// Use this for initialization
	void Start () 
	{
        canvasScaler = GameObject.FindWithTag("DeckScaler").GetComponent<CanvasScaler>();

        zoomBut = zoomButtonGO.GetComponent<Button>();

        inactiveColor = zoomButIcon.color;

        zoomBut.onClick.AddListener(zoomOut);

        initialZoomPosition = zoomIn.deckHolderPivot.anchoredPosition3D;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if(canvasScaler.scaleFactor == minZoom && zoomButtonGO.activeSelf || deckMapHolder.anchoredPosition == Vector2.zero && zoomButtonGO.activeSelf)
        {
            ChangeButtonColor(inactiveColor, false);            
        }

        if(canvasScaler.scaleFactor > minZoom && !zoomButtonGO.activeSelf || deckMapHolder.anchoredPosition != Vector2.zero && !zoomButtonGO.activeSelf)
        {
            ChangeButtonColor(activeColor, true);
        }
	}

    public void zoomOut()
    {
        zoomIn.stopZoom = true;
        canvasScaler.scaleFactor = minZoom;
        zoomIn.deckHolderPivot.anchoredPosition3D = initialZoomPosition;
        deckMapHolder.anchoredPosition3D = Vector3.zero;
        zoomBut.interactable = false;
        StartCoroutine(ReEnableButton());
    }

    public void ChangeButtonColor(Color newColor, bool state)
    {
        zoomButtonGO.SetActive(state);
        zoomButIcon.DOColor(newColor, 0.125f);
        zoomButtonText.DOColor(newColor, 0.125f);
    }

    public IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(0.25F);
        zoomBut.interactable = true;
    }
}
#endif