using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegendsTouchManager : MonoBehaviour
{

    private Vector2 pos;
    private Vector2 newCenter;
    Vector2 Center = new Vector2(0, 0);

    List<Touch> touches;

    private bool centered;
    private bool beginZoom;
    private bool leftSide;
    private bool bottomSide;

    //public ZoomOut zoomOut;

    public CanvasScaler canvasScaler;
    public Canvas canvas;

    public CanvasGroup CG;

    public RectTransform legendsPivot;
    public RectTransform legendsHolder;

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

    private void Start()
    {
        if (CG == null)
            CG = GetComponent<CanvasGroup>();
        //zoomOut = FindObjectOfType<ZoomOut>();
    }

    void LateUpdate()
    {
        touches = InputHelper.GetTouches();

        if (CG.alpha == 1)
        {
            if (Input.touches.Length == 1)
            {
                if (ExtendedStandaloneInputModule.GetPointerEventData(Input.touches[0].fingerId).pointerEnter != null)
                {
                    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        tapcount++;
                    }

                    if (tapcount >= 2 || (Input.GetKeyDown(KeyCode.Space)))
                    {
                        stopZoom = false;
                        StartCoroutine(DoubleTapToZoom(Input.touches[0].position, newCamScale));

                        doubleTapTimer = 0f;
                        tapcount = 0f;
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

            if (Input.touchCount == 2)
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
                legendsHolder.parent = canvas.transform;

                UpdatePivotPosition(doubleTapCenter, true);

                legendsHolder.parent = legendsPivot;

                posX = legendsPivot.anchoredPosition.x;
                posY = legendsPivot.anchoredPosition.y;

                beginZoom = true;
            }

            if (beginZoom)
            {
                float t = curNum / endNum;
                t = LerpSmoothState.SmoothLerp(t, smooth);
                t = t + (t * percentageModifier);

                canvasScaler.scaleFactor = Mathf.Lerp(canvasScaler.scaleFactor, newScaleFactor, t);

                posX = PivotNearCenter(posX, Center.x, leftSide) ? posX = 0 : Mathf.Lerp(legendsPivot.anchoredPosition.x, 0, t * pivotSpeed);
                posY = PivotNearCenter(posY, Center.y, bottomSide) ? posY = 0 : Mathf.Lerp(legendsPivot.anchoredPosition.y, 0, t * pivotSpeed);

                legendsPivot.anchoredPosition = new Vector2(posX, posY);
            }

            curNum += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        stopZoom = false;
        beginZoom = false;
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
            legendsHolder.parent = canvas.transform;

            UpdatePivotPosition(newCenter, true);

            legendsHolder.parent = legendsPivot;

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
            legendsPivot.transform.position = canvas.transform.TransformPoint(pos);
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

