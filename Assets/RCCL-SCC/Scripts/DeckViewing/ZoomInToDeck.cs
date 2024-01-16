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
using TouchScript.Gestures.TransformGestures;
using TouchScript.Behaviors;
using TMPro;
using DG.Tweening;

#if SCC_2_5
using Zenject;
public class ZoomInToDeck : MonoBehaviour, IZoomInDeck
{
    [Inject]
    public AnnotationManager annotationManager;

    private Vector3 horizontalRot = new Vector3(0, 0, -90f);
    private Vector3 verticalRot;

    private Vector2 pos;
    private Vector2 newCenter;
    Vector2 Center = new Vector2(0, 0);

    List<Touch> touches;

    private bool centered;
    private bool beginZoom;
    private bool leftSide;
    private bool bottomSide;

    public ZoomOut zoomOut;

    public CanvasScaler canvasScaler;
    public Canvas canvas;

    public RectTransform deckHolderPivot;
    public RectTransform deckholder;

    public Color inactiveColor;
    public Color activeColor;

    public GameObject forwardVert;
    public GameObject forwardHori;

    private float minZoom = 1f;
    public float maxZoom = 15f;
    private float tapcount = 0f;
    private float doubleTapTimer = 0f;
    private float endNum = 5f;
    public float newCamScale;
    public bool stopZoom;

    public LerpSmoothState.Smoothing smooth;

    public float pivotSpeed;
    public float percentageModifier = 0.2f;

    public Vector2 testPos; 

    public ScreenTransformGesture screenTransformGesture;
    public Transformer transformer;

    private void Start()
    {
        zoomOut = FindObjectOfType<ZoomOut>();
        verticalRot = deckHolderPivot.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        touches = InputHelper.GetTouches();

        if (Input.touches.Length == 1)
        {
            if (ExtendedStandaloneInputModule.GetPointerEventData(Input.touches[0].fingerId).pointerEnter != null 
            && annotationManager.GetCurrentAnnotationState() == AnnotationState.Move)
            {
                if (ExtendedStandaloneInputModule.GetPointerEventData(Input.touches[0].fingerId).pointerEnter.name.StartsWith("DeckMap_"))
                {
                    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        tapcount++;
                    }

                    if (tapcount >= 2)
                    {
                        stopZoom = false;
                        StartCoroutine(DoubleTapToZoom(Input.touches[0].position, newCamScale));

                        doubleTapTimer = 0f;
                        tapcount = 0f;
                    }
                }
            }
        }

        if (tapcount > 0)
        {
            doubleTapTimer += Time.deltaTime;
        }


        if (doubleTapTimer > 0.5f)
        {
            doubleTapTimer = 0f;
            tapcount = 0f;
        }

        if (Input.touchCount == 2 && ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter != null)
        {
            if(ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.name.StartsWith("DeckMap_"))
            {
                DetectTouchMovement.Calculate();

                stopZoom = true;

                CenterZoom();

                ZoomMap(true);
            }
            
        }

        else
        {
            centered = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //ZoomMap(false);
        }
    }

    public IEnumerator DoubleTapToZoom(Vector3 doubleTapCenter, float newScaleFactor)
    {
        Debug.Log("Firing");
        float curNum = 0f;
        float posX = 0;
        float posY = 0;
        endNum = newScaleFactor / 1.66f;
        pivotSpeed = (endNum * 2);

        leftSide = doubleTapCenter.x < Screen.width / 2 ? true : false;
        bottomSide = doubleTapCenter.y < Screen.height / 2 ? true : false;

       

        while (canvasScaler.scaleFactor < newScaleFactor - 0.01f && !stopZoom)
        {
            screenTransformGesture.enabled = false;
            transformer.enabled = false;

            if (!beginZoom)
            {
                deckholder.parent = canvas.transform;

                UpdatePivotPosition(doubleTapCenter, true);

                deckholder.parent = deckHolderPivot;

                posX = deckHolderPivot.anchoredPosition.x;
                posY = deckHolderPivot.anchoredPosition.y;

                beginZoom = true;
            }

            if (beginZoom)
            {
                float t = curNum / endNum;
                t = LerpSmoothState.SmoothLerp(t, smooth);
                t = t + (t * percentageModifier);

                canvasScaler.scaleFactor = Mathf.Lerp(canvasScaler.scaleFactor, newScaleFactor, t);

                posX = PivotNearCenter(posX, Center.x, leftSide) ? posX = 0 : Mathf.Lerp(deckHolderPivot.anchoredPosition.x, 0, t * pivotSpeed);
                posY = PivotNearCenter(posY, Center.y, bottomSide) ? posY = 0 : Mathf.Lerp(deckHolderPivot.anchoredPosition.y, 0, t * pivotSpeed);

                deckHolderPivot.anchoredPosition = new Vector2(posX, posY);
            }

            curNum += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        stopZoom = false;
        beginZoom = false;
        screenTransformGesture.enabled = true;
        transformer.enabled = true;
    }

    public void rotateThemDecks()
    {
        if (deckHolderPivot.rotation.z == 0)
        {
            DeckRotation(horizontalRot, true);
        }

        else
        {
            DeckRotation(verticalRot, false);
        }

        zoomOut.zoomOut();
    }

    public void DeckRotation(Vector3 rot, bool rotated)
    {
        deckHolderPivot.eulerAngles = rot;
        annotationManager.Rotated = rotated;
        forwardVert.SetActive(!rotated);
        forwardHori.SetActive(rotated);
    }

    public void ZoomMap(bool touchBased)
    {
        if (canvasScaler.scaleFactor >= minZoom && canvasScaler.scaleFactor <= maxZoom)
        {
            if (touchBased)
            {
                canvasScaler.scaleFactor += DetectTouchMovement.pinchDistanceDelta / 32;
            }

            else
            {
                canvasScaler.scaleFactor += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 32;
            }

        }

        if (canvasScaler.scaleFactor < minZoom)
        {
            canvasScaler.scaleFactor = minZoom;
        }

        if (canvasScaler.scaleFactor > maxZoom)
        {
            canvasScaler.scaleFactor = maxZoom;
        }
    }

    public void CenterZoom()
    {
        DetectTouchMovement.Calculate();

        newCenter = DetectTouchMovement.center;

        if (!centered)
        {
            deckholder.parent = canvas.transform;

            UpdatePivotPosition(newCenter, true);

            deckholder.parent = deckHolderPivot;

            centered = true;
        }

        else
        {
            UpdatePivotPosition(newCenter, true);
        }
    }

    public void UpdatePivotPosition(Vector2 newPosition, bool camSpace)
    {
        if (camSpace)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, newPosition, canvas.worldCamera, out pos);
            deckHolderPivot.transform.position = canvas.transform.TransformPoint(pos);
        }

        else
        {
            return;
        }
    }

    public bool PivotNearCenter(float point, float centerPoint, bool leftBottomSide)
    {
        if (leftBottomSide && point >= 0)
        {
            return true;
        }

        if (!leftBottomSide && point <= 0)
        {
            return true;
        }

        return false;
    }
}

/*-------------------------------------------------------------------------------------------------------------*/

#elif !SCC_2_5
public class ZoomInToDeck : MonoBehaviour
{
    private Vector3 horizontalRot = new Vector3(0, 0, -90f);
    private Vector3 verticalRot;

    private Vector2 pos;
    private Vector2 newCenter;
    Vector2 Center = new Vector2(0, 0);

    List<Touch> touches;

    private bool centered;
    private bool beginZoom;
    private bool leftSide;
    private bool bottomSide;

    public ZoomOut zoomOut;

    public CanvasScaler canvasScaler;
    public Canvas canvas;

    public DrawOnDeck drawOnDeck;

    public RectTransform deckHolderPivot;
    public RectTransform deckholder;

    public Button rotateDecks;

    public Image rotateButtonIcon;
    public TextMeshProUGUI rotateButtonText;

    public Color inactiveColor;
    public Color activeColor;

    public GameObject forwardVert;
    public GameObject forwardHori;

    private float minZoom = 1f;
    private float tapcount = 0f;
    private float doubleTapTimer = 0f;
    private float endNum = 5f;
    public float newCamScale;
    public bool stopZoom;

    public LerpSmoothState.Smoothing smooth;

    public float pivotSpeed;
    public float percentageModifier = 0.2f;

    private void Start()
    {
        zoomOut = FindObjectOfType<ZoomOut>();
        verticalRot = deckHolderPivot.rotation.eulerAngles;
        rotateDecks.onClick.AddListener(rotateThemDecks);

        inactiveColor = rotateButtonIcon.color;
    }

    void LateUpdate()
    {
        touches = InputHelper.GetTouches();

        if (Input.touches.Length == 1)
        {
            if (ExtendedStandaloneInputModule.GetPointerEventData(Input.touches[0].fingerId).pointerEnter != null && !drawOnDeck.draw && 
                !drawOnDeck.placeIcon)
            {
                if (ExtendedStandaloneInputModule.GetPointerEventData(Input.touches[0].fingerId).pointerEnter.name.StartsWith("DeckMap"))
                {
                    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        tapcount++;
                    }

                    if (tapcount >= 2)
                    {
                        stopZoom = false;
                        StartCoroutine(DoubleTapToZoom(Input.touches[0].position, newCamScale));

                        doubleTapTimer = 0f;
                        tapcount = 0f;
                    }
                }
            }
        }

        if (tapcount > 0)
        {
            doubleTapTimer += Time.deltaTime;
        }


        if (doubleTapTimer > 0.5f)
        {
            doubleTapTimer = 0f;
            tapcount = 0f;
        }

        if (Input.touchCount == 2 && ExtendedStandaloneInputModule.GetPointerEventData(Input.touches[0].fingerId).pointerEnter.name.StartsWith("DeckMap"))
        {
            DetectTouchMovement.Calculate();

            stopZoom = true;

            CenterZoom();

            ZoomMap(true);
        }

        else
        {
            centered = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomMap(false);
        }
    }

    public IEnumerator DoubleTapToZoom(Vector3 doubleTapCenter, float newScaleFactor)
    {
        float curNum = 0f;
        float posX = 0;
        float posY = 0;
        endNum = newScaleFactor / 1.66f;
        pivotSpeed = (endNum * 2);

        leftSide = doubleTapCenter.x < Screen.width / 2 ? true : false;
        bottomSide = doubleTapCenter.y < Screen.height / 2 ? true : false;

        while (curNum < endNum && !stopZoom)
        {
            if (!beginZoom)
            {
                deckholder.parent = canvas.transform;

                UpdatePivotPosition(doubleTapCenter, true);

                deckholder.parent = deckHolderPivot;

                posX = deckHolderPivot.anchoredPosition.x;
                posY = deckHolderPivot.anchoredPosition.y;

                beginZoom = true;
            }

            if (beginZoom)
            {
                float t = curNum / endNum;
                t = LerpSmoothState.SmoothLerp(t, smooth);
                t = t + (t * percentageModifier);

                canvasScaler.scaleFactor = Mathf.Lerp(canvasScaler.scaleFactor, newScaleFactor, t);

                posX = PivotNearCenter(posX, Center.x, leftSide) ? posX = 0 : Mathf.Lerp(deckHolderPivot.anchoredPosition.x, 0, t * pivotSpeed);
                posY = PivotNearCenter(posY, Center.y, bottomSide) ? posY = 0 : Mathf.Lerp(deckHolderPivot.anchoredPosition.y, 0, t * pivotSpeed);

                deckHolderPivot.anchoredPosition = new Vector2(posX, posY);
            }

            curNum += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        stopZoom = false;
        beginZoom = false;
    }

    public void rotateThemDecks()
    {
        if (deckHolderPivot.rotation.z == 0)
        {
            deckHolderPivot.eulerAngles = horizontalRot;
            drawOnDeck.rotated = true;
            rotateButtonIcon.DOColor(activeColor, 0.125f);
            rotateButtonText.DOColor(activeColor, 0.125f);
            if(forwardHori != null && forwardVert != null)
            {
                forwardVert.SetActive(false);
                forwardHori.SetActive(true);
            }
        }

        else
        {
            deckHolderPivot.eulerAngles = verticalRot;
            drawOnDeck.rotated = false;
            rotateButtonIcon.DOColor(inactiveColor, 0.125f);
            rotateButtonText.DOColor(inactiveColor, 0.125f);
            if (forwardHori != null && forwardVert != null)
            {
                forwardVert.SetActive(true);
                forwardHori.SetActive(false);
            }
        }

        zoomOut.zoomOut();
    }

    public void ZoomMap(bool touchBased)
    {
        if (canvasScaler.scaleFactor >= minZoom && canvasScaler.scaleFactor <= 25)
        {
            if (touchBased)
            {
                canvasScaler.scaleFactor += DetectTouchMovement.pinchDistanceDelta / 32;
            }

            else
            {
                canvasScaler.scaleFactor += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 32;
            }

        }

        if (canvasScaler.scaleFactor < minZoom)
        {
            canvasScaler.scaleFactor = minZoom;
        }

        if (canvasScaler.scaleFactor > 25)
        {
            canvasScaler.scaleFactor = 25;
        }
    }

    public void CenterZoom()
    {
        DetectTouchMovement.Calculate();

        newCenter = DetectTouchMovement.center;

        if (!centered)
        {
            deckholder.parent = canvas.transform;

            UpdatePivotPosition(newCenter, true);

            deckholder.parent = deckHolderPivot;

            centered = true;
        }

        else
        {
            UpdatePivotPosition(newCenter, true);
        }
    }

    public void UpdatePivotPosition(Vector2 newPosition, bool camSpace)
    {
        if (camSpace)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, newPosition, canvas.worldCamera, out pos);
            deckHolderPivot.transform.position = canvas.transform.TransformPoint(pos);
        }

        else
        {
            return;
        }
    }

    public bool PivotNearCenter(float point, float centerPoint, bool leftBottomSide)
    {
        if (leftBottomSide && point >= 0)
        {
            return true;
        }

        if (!leftBottomSide && point <= 0)
        {
            return true;
        }

        return false;
    }
}

#endif