using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

#if !SCC_2_5
public class DrawOnDeck : MonoBehaviour
{
    private static volatile DrawOnDeck instance;
    public static DrawOnDeck Instance { get { return instance; } }

    #region shit to fix
    public GameObject LineRendererPrefab;
    public UILineRenderer LineRenderer;
    public Transform parentObject;
    public RectTransform parentRect;
    public Color lineCol;

    public GameObject iconToPlace;

    public IconBehavior.IconType iconType;
    public SelectIcon selectIcon;
    public GameObject newIcon;

    public Button toggleGAEditMode;
    public Button toggleGAOverlay;

    public BuildSaveGAEdits buildSaveGAEdits { get; set; }

    public GameObject editGABorder;

    public Canvas parentCanvasOfImageToMove;
    public CanvasScaler canvasScaler;
    public Image brush;

    public List<Transform> deckSelection;
    public List<GameObject> lineHolders;
    public List<GameObject> gaHolders;

    public Button hideButton;
    public GameObject HideButtonGO;

    public ZoomOut zoomOut;

    public Image hideButtonIcon;
    public TextMeshProUGUI hideButtonText;

    public Color inactiveColor;
    public Color activeColor;

    public Button drawButton;
    public GameObject DrawPanel;
    public WidgetToDock widgetToDock;
    public GameObject annoIcons;
    public GameObject gaIcons;

    public ButtonHighlight highlighterBH;
    public ButtonHighlight penBH;
    public ButtonHighlight eraserBH;
    public ButtonHighlight moveBH;

    public ToggleSwitch toggleAnnotations;
    public ToggleSwitch toggleGAEdits;

    private float Size;

    private string parent;

    public bool draw;
    public bool placeIcon;
    private bool startedPlacing;
    public bool editGA;
    public bool erase;
    private bool hidingDecks;
    private bool highlighter;
    public bool rotated;
    private bool widgetActive;

    private Vector3 mousePos;
    private Vector2 pos;

    private GameObject curDeckMap;

    private IconBehavior iconBehavior;

    public KeyBoardPromptHandler iconKeyBoardPromptHandler;
    public TMP_InputField chemNameField;
    public TMP_InputField iconNumField;
    public Button saveButton;
    public KeyboardManager keyboardManager;

    public string newAnnoString;

    List<Touch> touches;

    #endregion

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Debug.Log("Destroying singleton duplicate: " + gameObject);
            Destroy(this.gameObject);
            return;
        }
    }

    void Start()
    {
        zoomOut = FindObjectOfType<ZoomOut>();
        HideButtonGO = hideButton.gameObject;
        inactiveColor = hideButtonIcon.color;
        ChangeThickness(5f);

        foreach (Transform deck in deckSelection)
        {
            lineHolders.Add(deck.Find("LineHolder").gameObject);
            gaHolders.Add(deck.Find("GAHolder").gameObject);
        }

        SetUpButtons();
    }

    private void Update()
    {
        if (iconToPlace != null && iconType != iconToPlace.GetComponent<IconBehavior>().type)
        {
            Debug.Log("Icon type changing from: " + iconType.ToString() + " to: " + iconToPlace.GetComponent<IconBehavior>().type.ToString(), this);
            iconType = iconToPlace.GetComponent<IconBehavior>().type;

        }
        if (draw)
        {
            MakeAnnotation();
        }

        if (placeIcon)
        {
            MakeNewIcon();
        }
    }

    public void MakeAnnotation()
    {
        touches = InputHelper.GetTouches();

        if (touches.Count == 1 && OverObject())
        {
            if (LineRenderer == null)
            {
                NewLineRenderer();
            }

            else if (LineRenderer != null)
            {
                UpdateBrushPosition();
                AddNewPoint(mousePos);
            }
        }

        else if (touches.Count == 1 && !OverObject())
        {
            parentObject = null;
            parentRect = null;
        }

        if (touches.Count == 0)
        {
            if (LineRenderer != null)
            {
                SaveAndSendAnnotation();
            }
        }
    }

    public void NewLineRenderer()
    {
        parent = editGA ? "GAHolder" : "LineHolder";
        SetAnnotationParent(parent);

        GameObject newLineRenderer = Instantiate(LineRendererPrefab, parentObject);
        LineRenderer = newLineRenderer.GetComponent<UILineRenderer>();
        newLineRenderer.GetComponent<LineBehavior>().GAEdit = editGA;

        if (highlighter)
        {
            newLineRenderer.AddComponent<UIMultiplyEffect>();
            LineRenderer.color = HighLighterColor(lineCol);
        }

        else
        {
            LineRenderer.color = lineCol;
        }

        LineRenderer.lineThickness = Size;
        UpdateBrushPosition();
    }

    public void MakeNewIcon()
    {
        touches = InputHelper.GetTouches();

        if (touches.Count == 1 && OverObject())
        {
            startedPlacing = true;
            PlaceTheIcon();
        }

        if (touches.Count == 0 && startedPlacing)
        {
            iconToPlace = null;
            placeIcon = false;
            startedPlacing = false;
            newIcon.GetComponent<Image>().raycastTarget = true;
            iconBehavior = newIcon.GetComponent<IconBehavior>();
            iconBehavior.transGesture.enabled = true;
            iconBehavior.parent = parent;
            iconBehavior.parentObject = parentObject;
            iconBehavior.isGA = editGA;

            selectIcon.ToggleBracket();

            if (iconBehavior.type != IconBehavior.IconType.normal)
            {
                StartCoroutine(NameTheIcon());
            }

            else
            {
                selectIcon = null;
                newIcon = null;
                toggleHighlighter(moveBH.GetComponent<Button>());
            }

        }
    }

    public void PlaceTheIcon()
    {
        if (newIcon == null)
        {
            parent = editGA ? "GAHolder" : "LineHolder";
            SetAnnotationParent(parent);

            newIcon = Instantiate(iconToPlace, parentObject);
            newIcon.GetComponent<IconBehavior>().master = this;
            newIcon.GetComponent<Image>().raycastTarget = false;
        }

        if (parentObject != ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent).transform)
        {
            SetAnnotationParent(parent);
            newIcon.transform.parent = parentObject;
        }

        UpdateBrushPosition();
        newIcon.transform.position = mousePos;
    }

    public void SetAnnotationParent(string parentName)
    {
        parentObject = ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parentName).transform;
        parentRect = parentObject.GetComponent<RectTransform>();
    }

    public void UpdateBrushPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvasOfImageToMove.transform as RectTransform, touches[0].position, parentCanvasOfImageToMove.worldCamera, out pos);
        mousePos = parentCanvasOfImageToMove.transform.TransformPoint(pos);
    }

    public IEnumerator NameTheIcon()
    {
        if (iconBehavior.type == IconBehavior.IconType.chemical)
        {
            iconKeyBoardPromptHandler.gameObject.SetActive(true);
            iconKeyBoardPromptHandler.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.name);
            chemNameField.characterLimit = 16;
            yield return new WaitForEndOfFrame();
            keyboardManager.ChangeInputField(chemNameField);
            //chemNameField.GetComponent<InputFieldHighlight>().FrameSelected();
        }

        if (iconBehavior.type == IconBehavior.IconType.numbered)
        {
            iconKeyBoardPromptHandler.gameObject.SetActive(true);
            iconKeyBoardPromptHandler.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.number);
            iconNumField.characterLimit = 2;
            yield return new WaitForEndOfFrame();
            keyboardManager.ChangeInputField(iconNumField);
            //iconNumField.GetComponent<InputFieldHighlight>().FrameSelected();
        }
    }

    public void SaveTheIcon()
    {
        if (iconBehavior.type == IconBehavior.IconType.chemical)
        {
            iconBehavior.iconText = chemNameField.text;
            iconBehavior.nameOrNumber.text = chemNameField.text;
            chemNameField.text = "";

            if (chemNameField.GetComponent<InputFieldHighlight>().selected)
            {
                chemNameField.GetComponent<InputFieldHighlight>().FrameSelected();
            }
        }

        if (iconBehavior.type == IconBehavior.IconType.numbered)
        {
            iconBehavior.iconText = iconNumField.text;
            iconBehavior.nameOrNumber.text = iconNumField.text;
            iconNumField.text = "";

            if (iconNumField.GetComponent<InputFieldHighlight>().selected)
            {
                iconNumField.GetComponent<InputFieldHighlight>().FrameSelected();
            }
        }

        keyboardManager.currentLine = null;
        selectIcon = null;
        newIcon = null;
        toggleHighlighter(moveBH.GetComponent<Button>());

        StartCoroutine(ToggleIconPrompt());
    }

    public IEnumerator ToggleIconPrompt()
    {
        yield return new WaitForSeconds(.3f);

        iconKeyBoardPromptHandler.gameObject.SetActive(false);
    }

    public void CancelIcon()
    {
        if (chemNameField.GetComponent<InputFieldHighlight>().selected)
        {
            chemNameField.GetComponent<InputFieldHighlight>().FrameSelected();
        }

        if (iconNumField.GetComponent<InputFieldHighlight>().selected)
        {
            iconNumField.GetComponent<InputFieldHighlight>().FrameSelected();
        }

        chemNameField.text = "";
        iconNumField.text = "";
        keyboardManager.currentLine = null;
        selectIcon = null;
        newIcon = null;
        toggleHighlighter(moveBH.GetComponent<Button>());
        iconKeyBoardPromptHandler.gameObject.SetActive(false);
    }

    public void AddNewPoint(Vector3 point)
    {
        var pointlist = new List<Vector2>(LineRenderer.Points);
        Vector3[] corners = new Vector3[4];
        parentRect.GetWorldCorners(corners);

        Vector2 drawPoint = point - corners[0];

        drawPoint = drawPoint / canvasScaler.scaleFactor;

        if (rotated)
        {
            drawPoint = new Vector2(drawPoint.y * -1, drawPoint.x);
        }

        if (LineRenderer.Points[0] == Vector2.zero)
        {
            pointlist.RemoveAt(0);
            pointlist.Add(drawPoint);
        }

        else
        {
            pointlist.Add(drawPoint);
        }

        LineRenderer.Points = pointlist.ToArray();
    }

    public void SaveAndSendAnnotation()
    {
        //string annotationToSend;
        string name = LineRenderer.name.Replace("(Clone)", "");
        string position = LineRenderer.transform.parent.transform.parent.GetSiblingIndex().ToString();
        string color = "#" + ColorUtility.ToHtmlStringRGB(LineRenderer.color);
        string thickness = LineRenderer.lineThickness.ToString();
        string highligher = LineRenderer.gameObject.GetComponent<UIMultiplyEffect>() != null ? "true" : "false";
        string[] points = new string[LineRenderer.Points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = LineRenderer.Points[i].x.ToString() + "=" + LineRenderer.Points[i].y.ToString();
        }

        newAnnoString = name + "," + position + "," + color + "," + thickness + "," + highlighter;

        foreach (string newPoint in points)
        {
            newAnnoString = newAnnoString + "," + newPoint;
        }

        newAnnoString = "";
        LineRenderer = null;
        parentObject = null;
        parentRect = null;
        curDeckMap = null;
    }

    public void HideDecks()
    {
        if (!hidingDecks)
        {
            foreach (Transform decks in deckSelection)
            {
                if (!decks.GetComponent<DeckSelect>().deckSelected)
                {
                    decks.gameObject.SetActive(false);
                }

                else
                {
                    continue;
                }
            }

            hidingDecks = true;
        }

        else
        {
            foreach (Transform decks in deckSelection)
            {
                decks.gameObject.SetActive(true);

                if (decks.GetComponent<DeckSelect>().deckSelected)
                {
                    decks.GetComponent<DeckSelect>().selectDeck();
                }
            }

            hidingDecks = false;

            ChangeButtonColor(inactiveColor, false);
        }

        zoomOut.zoomOut();
    }

    public void ChangeColor(Color color)
    {
        lineCol = color;
    }

    public void ChangeThickness(float thickness)
    {
        Size = thickness;
    }

    public void DrawMode()
    {
        widgetActive = !widgetActive;

        if (!widgetActive)
        {
            toggleHighlighter(moveBH.GetComponent<Button>());
            widgetToDock.ButtonColorChange(widgetToDock.inactiveColor, false);
        }

        DrawPanel.transform.SetSiblingIndex(DrawPanel.transform.parent.childCount - 1);

        DrawPanel.SetActive(widgetActive);

        if (DrawPanel.activeSelf)
        {
            widgetToDock.DockWidget();
            widgetToDock.visible = true;
            StartCoroutine(toggleIconPanels());
        }

        if (!DrawPanel.activeSelf)
        {
            widgetToDock.UnDockWidget();
        }
    }

    public IEnumerator toggleIconPanels()
    {
        yield return new WaitForEndOfFrame();
        annoIcons.SetActive(!editGA);
        gaIcons.SetActive(editGA);
    }

    public void toggleHighlighter(Button button)
    {
        if (button == highlighterBH.GetComponent<Button>())
        {
            highlighterBH.Highlight(true);
            penBH.Highlight(false);
            eraserBH.Highlight(false);
            moveBH.Highlight(false);
            highlighter = true;
            draw = true;
            placeIcon = false;
            erase = false;

            if (selectIcon != null)
            {
                selectIcon.ToggleBracket();
                iconToPlace = null;
                selectIcon = null;
            }
        }

        if (button == penBH.GetComponent<Button>())
        {
            penBH.Highlight(true);
            highlighterBH.Highlight(false);
            eraserBH.Highlight(false);
            moveBH.Highlight(false);
            highlighter = false;
            draw = true;
            placeIcon = false;
            erase = false;

            if (selectIcon != null)
            {
                selectIcon.ToggleBracket();
                iconToPlace = null;
                selectIcon = null;
            }
        }

        if (button == eraserBH.GetComponent<Button>())
        {
            eraserBH.Highlight(true);
            penBH.Highlight(false);
            highlighterBH.Highlight(false);
            moveBH.Highlight(false);
            highlighter = false;
            draw = false;
            placeIcon = false;
            erase = true;


            if (selectIcon != null)
            {
                selectIcon.ToggleBracket();
                iconToPlace = null;
                selectIcon = null;
            }
        }

        if (button == moveBH.GetComponent<Button>())
        {
            if (selectIcon != null)
            {
                selectIcon.ToggleBracket();
                iconToPlace = null;
                selectIcon = null;
            }

            moveBH.Highlight(true);
            penBH.Highlight(false);
            highlighterBH.Highlight(false);
            eraserBH.Highlight(false);
            highlighter = false;
            draw = false;
            placeIcon = false;
            erase = false;
        }

        else if (selectIcon != null && button == selectIcon.selectionButton)
        {
            moveBH.Highlight(false);
            penBH.Highlight(false);
            highlighterBH.Highlight(false);
            eraserBH.Highlight(false);
            highlighter = false;
            draw = false;
            erase = false;
        }
    }

    public void ToggleEditGAMode()
    {
        if (!editGA)
        {
            if (!widgetActive)
            {
                DrawMode();
                drawButton.GetComponent<ToggleSwitch>().Toggle();
            }

            else
            {
                StartCoroutine(toggleIconPanels());
            }

            if (!toggleGAEdits.toggled)
            {
                toggleGAEdits.Toggle();
            }

            foreach (GameObject lh in lineHolders)
            {
                lh.SetActive(false);
            }

            buildSaveGAEdits.ToggleReset(activeColor);

            placeIcon = false;
            draw = false;
            editGA = true;
        }

        else
        {
            if (widgetActive)
            {
                DrawMode();
                drawButton.GetComponent<ToggleSwitch>().Toggle();
            }

            foreach (GameObject lh in lineHolders)
            {
                lh.SetActive(true);
            }

            placeIcon = false;
            draw = false;
            editGA = false;

            buildSaveGAEdits.ToggleReset(inactiveColor);

            buildSaveGAEdits.SavePrefabs();
        }

        editGABorder.SetActive(editGA);
    }

    public void eraseAllAnnotations()
    {
        if (!editGA)
        {
            foreach (GameObject lh in lineHolders)
            {
                for (int i = 0; i < lh.transform.childCount; i++)
                {
                    Destroy(lh.transform.GetChild(i).gameObject);
                }
            }
        }

        else
        {
            foreach (GameObject gah in gaHolders)
            {
                for (int i = 0; i < gah.transform.childCount; i++)
                {
                    Destroy(gah.transform.GetChild(i).gameObject);
                }
            }
        }

    }

    public void ToggleArtwork()
    {
        foreach (GameObject lh in lineHolders)
        {
            lh.SetActive(toggleAnnotations.toggled);
        }
    }

    public void ToggleGAEdits()
    {
        foreach (GameObject gah in gaHolders)
        {
            gah.SetActive(toggleGAEdits.toggled);
        }
    }

    public void ChangeButtonColor(Color newColor, bool state)
    {
        HideButtonGO.SetActive(state);
        hideButtonIcon.DOColor(newColor, 0.125f);
        hideButtonText.DOColor(newColor, 0.125f);
    }

    public void SetUpButtons()
    {
        hideButton.onClick.AddListener(HideDecks);
        drawButton.onClick.AddListener(DrawMode);
        toggleGAEditMode.onClick.AddListener(ToggleEditGAMode);
        saveButton?.onClick.AddListener(SaveTheIcon);

        highlighterBH.GetComponent<Button>().onClick.AddListener(delegate
        {
            toggleHighlighter(highlighterBH.GetComponent<Button>());
        });

        penBH.GetComponent<Button>().onClick.AddListener(delegate
        {
            toggleHighlighter(penBH.GetComponent<Button>());
        });

        eraserBH.GetComponent<Button>().onClick.AddListener(delegate
        {
            toggleHighlighter(eraserBH.GetComponent<Button>());
        });

        moveBH.GetComponent<Button>().onClick.AddListener(delegate
        {
            toggleHighlighter(moveBH.GetComponent<Button>());
        });
    }

    public Color HighLighterColor(Color initialColor)
    {
        float H, S, V;

        Color.RGBToHSV(initialColor, out H, out S, out V);

        S = S - 0.4f;

        initialColor = Color.HSVToRGB(H, S, V);

        initialColor.a = .75f;

        return initialColor;
    }

    public bool OverObject()
    {
        if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter != null)
        {
            if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.gameObject == curDeckMap)
            {
                return true;
            }

            else
            {
                if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.name.StartsWith("DeckMap"))
                {
                    curDeckMap = ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.gameObject;
                }

                else
                {
                    curDeckMap = null;
                }

                if (LineRenderer != null)
                {
                    SaveAndSendAnnotation();
                }


                LineRenderer = null;
                return false;
            }
        }

        else
        {
            if (LineRenderer != null)
            {
                SaveAndSendAnnotation();
            }

            curDeckMap = null;
            LineRenderer = null;

            return false;
        }


    }

    public bool CheckTheDecks()
    {
        foreach (Transform decks in deckSelection)
        {
            if (decks.GetComponent<DeckSelect>().deckSelected || !decks.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }
}
#endif
