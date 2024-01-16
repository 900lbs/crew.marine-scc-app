// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : 900lbs
// Created          : 04-18-2019
//
// Last Modified By : 900lbs
// Last Modified On : 06-24-2019
// ***********************************************************************
// <copyright file="AnnotationManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using UnityEngine;

using TMPro;
using DG.Tweening;
using NaughtyAttributes;
using Zenject;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System.Threading.Tasks;
using System.Threading;

#if SCC_2_5

/// <summary>
/// Our main annotation handler class, as of now this class handles all states, functionality and signals that pertain to annotations.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="UnityEngine.EventSystems.IPointerDownHandler" />
/// Implements the <see cref="AnnotationManager" />
/// Implements the <see cref="Zenject.ILateDisposable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="UnityEngine.EventSystems.IPointerDownHandler" />
/// <seealso cref="AnnotationManager" />
/// <seealso cref="Zenject.ILateDisposable" />
public class AnnotationManager : MonoBehaviour, IPointerDownHandler
{
    #region Injection Construction
    /// <summary>
    /// The network client
    /// </summary>
    INetworkClient networkClient;
    /// <summary>
    /// The line factory
    /// </summary>
    LineRendererFactory lineFactory;
    /// <summary>
    /// The icon factory
    /// </summary>
    IconBehavior.Factory iconFactory;
    /// <summary>
    /// The deck manager
    /// </summary>
    DeckManager deckManager;

    Settings SceneSettings;
    /// <summary>
    /// The UI manager
    /// </summary>
    UIManager uiManager;
    /// <summary>
    /// The signal bus
    /// </summary>
    SignalBus _signalBus;

    /// <summary>
    /// The ga save edits
    /// </summary>
    BuildSaveGAEdits gaSaveEdits;

    /// <summary>
    /// Our injection construction method (method is used since Monobehaviours can't have constructors)
    /// </summary>
    /// <param name="netClient">Our network interface.</param>
    /// <param name="lineFact">The factory we call to create a new line renderer.</param>
    /// <param name="iconFact">The factory we call to create a new icon.</param>
    /// <param name="deckMan">Our DeckManager that contains all deck information</param>
    /// <param name="sigBus">Our signal object that allows signal subscriptions and fires.</param>
    /// <param name="uiMan">Our standard c# class that handles high level UI settings and functionality</param>
    /// <param name="gaEdits">The ga edits.</param>
    [Inject]
    public void Construct(INetworkClient netClient,
    LineRendererFactory lineFact,
    IconBehavior.Factory iconFact,
    DeckManager deckMan,
    Settings settings,
    SignalBus sigBus,
    UIManager uiMan,
    BuildSaveGAEdits gaEdits)
    {
        networkClient = netClient;
        lineFactory = lineFact;
        iconFactory = iconFact;
        deckManager = deckMan;
        SceneSettings = settings;
        uiManager = uiMan;
        _signalBus = sigBus;
        gaSaveEdits = gaEdits;
    }
    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Read-Only States
    /// <summary>
    /// The current annotation tools
    /// </summary>
    [Tooltip("Read-only, displays our current tool or lack there of.")]
    [SerializeField] [EnumFlag] ActiveAnnotationTools currentAnnotationTools;
    /// <summary>
    /// The previous property state
    /// </summary>
    [SerializeField] [ReadOnly] ShipRoomProperties previousPropertyState;
    /// <summary>
    /// The current property state
    /// </summary>
    [SerializeField] [ReadOnly] ShipRoomProperties currentPropertyState;
    /// <summary>
    /// The current property active state
    /// </summary>
    [SerializeField] [EnumFlag] private ShipRoomProperties currentPropertyActiveState;
    /// <summary>
    /// The current annotation state
    /// </summary>
    [SerializeField] [ReadOnly] AnnotationState currentAnnotationState;

    [SerializeField] [EnumFlag] UserNameID currentActiveUserAnnotations;

    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Variables
    /// <summary>
    /// The line renderer prefab
    /// </summary>
    GameObject LineRendererPrefab;
    /// <summary>
    /// Gets or sets the current line behaviour.
    /// </summary>
    /// <value>The current line behaviour.</value>
    LineBehavior CurrentLineBehaviour { get; set; }

    /// <summary>
    /// The line col
    /// </summary>
    public Color lineCol;
    public List<Image> selBrackets = new List<Image>();
    public Image curSelBracket;

    /// <summary>
    /// The current icon data
    /// </summary>
    [ReadOnly] public SafetyIconData CurrentIconData;
    /// <summary>
    /// The select icon
    /// </summary>
    public SelectIcon SelectIcon;
    /// <summary>
    /// Gets or sets the spawned new icon.
    /// </summary>
    /// <value>The spawned new icon.</value>
    [SerializeField] public IconBehavior SpawnedNewIcon { get; set; }

    /// <summary>
    /// Gets or sets the build save ga edits.
    /// </summary>
    /// <value>The build save ga edits.</value>
    public BuildSaveGAEdits buildSaveGAEdits { get; set; }

    /// <summary>
    /// The edit ga border
    /// </summary>
    public GameObject editGABorder;

    /// <summary>
    /// The parent canvas of image to move
    /// </summary>
    public Canvas parentCanvasOfImageToMove;
    /// <summary>
    /// The canvas scaler
    /// </summary>
    public CanvasScaler canvasScaler;
    /// <summary>
    /// The zoom out
    /// </summary>
    public ZoomOut zoomOut;

    /// <summary>
    /// The draw panel
    /// </summary>
    public GameObject DrawPanel;


    /// <summary>
    /// The size
    /// </summary>
    public float Size;

    /// <summary>
    /// The parent
    /// </summary>
    string parent;

    /// <summary>
    /// The started placing
    /// </summary>
    bool startedPlacing;
    /// <summary>
    /// Gets or sets a value indicating whether this instance is ga edit enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is ga edit enabled; otherwise, <c>false</c>.</value>
    public bool IsGAEditEnabled { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AnnotationManager"/> is rotated.
    /// </summary>
    /// <value><c>true</c> if rotated; otherwise, <c>false</c>.</value>
    public bool Rotated { get; set; }

    /// <summary>
    /// The mouse position
    /// </summary>
    Vector3 mousePos;
    /// <summary>
    /// The position
    /// </summary>
    Vector2 pos;

    /// <summary>
    /// The current deck map
    /// </summary>
    Deck curDeckMap;

    /// <summary>
    /// Gets or sets the current deck map.
    /// </summary>
    /// <value>The current deck map.</value>
    public Deck CurDeckMap { get { return curDeckMap; } set { if (curDeckMap != value) { curDeckMap = value; } } }

    /// <summary>
    /// The icon key board prompt handler
    /// </summary>
    public KeyBoardPromptHandler iconKeyBoardPromptHandler;
    /// <summary>
    /// The chem name field
    /// </summary>
    public TMP_InputField chemNameField;
    /// <summary>
    /// The icon number field
    /// </summary>
    public TMP_InputField iconNumField;
    /// <summary>
    /// The save button
    /// </summary>
    public Button saveButton;
    /// <summary>
    /// The keyboard manager
    /// </summary>
    public KeyboardManager keyboardManager;

    /// <summary>
    /// The new anno string
    /// </summary>
    public string newAnnoString;

    public CancellationTokenSource annotationCancellationToken;

    /// <summary>
    /// The touches
    /// </summary>
    List<Touch> touches;

    #endregion

    /// <summary>
    /// Called when [pointer down].
    /// </summary>
    /// <param name="args">The arguments.</param>
    public void OnPointerDown(PointerEventData args) { }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        _signalBus.Subscribe<Signal_Widget_StateChanged>(Widget_ToggledVisibility);
        _signalBus.Subscribe<Signal_MainMenu_OnGameStateChanged>(MainMenu_OnGameStateChanged);

        zoomOut = FindObjectOfType<ZoomOut>();
        ChangeThickness(5f);

        annotationCancellationToken = new CancellationTokenSource();

        SetCurrentAnnotationState(AnnotationState.Move);
        SetCurrentPropertyState(ShipRoomProperties.None);
        previousPropertyState = ShipRoomProperties.Annotations;
        saveButton?.onClick.AddListener(SaveTheIcon);

        currentPropertyActiveState = ShipRoomProperties.Annotations | ShipRoomProperties.GAOverlay;
        _signalBus.Fire<Signal_AnnoMan_OnToggleRoomProperty>(new Signal_AnnoMan_OnToggleRoomProperty(currentPropertyActiveState));
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Updates this instance.
    /// </summary>
    async void Update()
    {
        await CheckAnnotationState();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Lates the dispose.
    /// </summary>
    public void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_Widget_StateChanged>(Widget_ToggledVisibility);
        _signalBus.Unsubscribe<Signal_MainMenu_OnGameStateChanged>(MainMenu_OnGameStateChanged);

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Widgets the toggled visibility.
    /// </summary>
    /// <param name="signal">The signal.</param>
    void Widget_ToggledVisibility(Signal_Widget_StateChanged signal)
    {
        if (signal.State == WidgetState.Disabled && GetCurrentAnnotationTools() == 0)
        {
            SetCurrentPropertyState(0);
        }
        /* else if (previousPropertyState != ShipRoomProperties.None)
        {
            SetCurrentPropertyState(previousPropertyState);
        } */
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Checks the state of the annotation.
    /// </summary>
    async Task CheckAnnotationState()
    {
        switch (GetCurrentAnnotationState())
        {
            case AnnotationState.Move:
            case AnnotationState.Eraser:
            case AnnotationState.EraseAll:
                return;

            case AnnotationState.Pen:
            case AnnotationState.Highlight:
                await MakeAnnotation();
                break;
            case AnnotationState.Icon:
                MakeNewIcon();
                break;

            default:
                return;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Interface Functions

    /// <summary>
    /// Gets the current annotation tools.
    /// </summary>
    /// <returns>ActiveAnnotationTools.</returns>
    public ActiveAnnotationTools GetCurrentAnnotationTools()
    {
        return currentAnnotationTools;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the current annotation tools.
    /// </summary>
    /// <param name="tool">The tool.</param>
    /// <param name="active">if set to <c>true</c> [active].</param>
    public void SetCurrentAnnotationTools(ActiveAnnotationTools tool, bool active)
    {
        currentAnnotationTools = (active) ? (currentAnnotationTools | tool) : (currentAnnotationTools ^ tool);

        _signalBus.Fire<Signal_AnnoMan_OnAnnotationToolsChanged>(new Signal_AnnoMan_OnAnnotationToolsChanged(currentAnnotationTools));
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the state of the current annotation.
    /// </summary>
    /// <returns>AnnotationState.</returns>
    public AnnotationState GetCurrentAnnotationState()
    {
        return currentAnnotationState;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the state of the current annotation.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="extraData">The extra data.</param>
    public void SetCurrentAnnotationState(AnnotationState state, object extraData = null)
    {
        currentAnnotationState = state;
        _signalBus.Fire(new Signal_AnnoMan_OnAnnotationStateChanged(state, extraData));
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the state of the current property.
    /// </summary>
    /// <returns>ShipRoomProperties.</returns>
    public ShipRoomProperties GetCurrentPropertyState()
    {
        return currentPropertyState;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the state of the current property.
    /// </summary>
    /// <param name="prop">The property.</param>
    public void SetCurrentPropertyState(ShipRoomProperties prop)
    {
        //If we're not already in this state
        if (currentPropertyState != prop)
        {
            // As long as property does not equal none and our current state doesn't either, update previous state to current.

            Debug.Log("Current property state changed to: " + prop.ToString(), this);

            previousPropertyState = currentPropertyState;
            currentPropertyState = prop;

            _signalBus.Fire(new Signal_AnnoMan_OnRoomPropertyChanged(prop));

            bool checkIfPropertyIsActive = currentPropertyActiveState.HasFlag(prop);

            SetCurrentPropertyActiveState(prop, checkIfPropertyIsActive);

            if (NetworkClient.GetIsPlayerMasterClient())
                editGABorder?.SetActive(prop.HasFlag(ShipRoomProperties.GAOverlay));

            if (previousPropertyState == ShipRoomProperties.GAOverlay)
            {
                gaSaveEdits.SavePrefabs();
            }

        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the state of the current property active.
    /// </summary>
    /// <returns>ShipRoomProperties.</returns>
    public ShipRoomProperties GetCurrentPropertyActiveState()
    {
        return currentPropertyActiveState;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the state of the current property active.
    /// </summary>
    /// <param name="prop">The property.</param>
    /// <param name="active">if set to <c>true</c> [active].</param>
    public void SetCurrentPropertyActiveState(ShipRoomProperties prop, bool active)
    {
        if (currentPropertyActiveState == 0)
        {
            currentPropertyActiveState = currentPropertyState;
        }
        else
        {
            if (active && (!currentPropertyActiveState.HasFlag(prop)))
            {
                currentPropertyActiveState = currentPropertyActiveState | prop;
            }
            else if (!active && (currentPropertyActiveState.HasFlag(prop)))
            {
                currentPropertyActiveState = currentPropertyActiveState ^ prop;
            }
            _signalBus.Fire<Signal_AnnoMan_OnToggleRoomProperty>(new Signal_AnnoMan_OnToggleRoomProperty(currentPropertyActiveState));
        }

    }

    public ShipRoomProperties GetCurrentActiveUserAnnotations()
    {
        return currentPropertyState;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the state of the current property.
    /// </summary>
    /// <param name="prop">The property.</param>
    public void SetCurrentActiveUserAnnotations(UserNameID user)
    {
        if (!currentActiveUserAnnotations.HasFlag(user))
        {
            //Debug.Log("User " + user.ToString() + " was selected.", this);
            currentActiveUserAnnotations = currentActiveUserAnnotations | user;
        }
        else
        {
            //Debug.Log("User " + user.ToString() + " was enabled.", this);
            currentActiveUserAnnotations = currentActiveUserAnnotations ^ user;
        }

        _signalBus.Fire(new Signal_AnnoMan_OnActiveUserAnnotationsUpdated(currentActiveUserAnnotations));

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the color of the current line.
    /// </summary>
    /// <returns>Color.</returns>
    public Color GetCurrentLineColor()
    {
        return lineCol;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the color of the current line.
    /// </summary>
    /// <param name="newColor">The new color.</param>
    public void SetCurrentLineColor(Color newColor)
    {
        if (lineCol != newColor)
        {
            lineCol = newColor;
        }

        foreach (Image selBracket in selBrackets)
        {
            if (selBracket != curSelBracket)
            {
                selBracket.enabled = false;
            }

            else
            {
                selBracket.enabled = true;
            }
        }

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the current line renderer object.
    /// </summary>
    /// <returns>GameObject.</returns>
    public GameObject GetCurrentLineRendererObject()
    {
        if (LineRendererPrefab != null)
            return LineRendererPrefab;

        Debug.LogError("Line renderer prefab not found.", this);
        return null;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the current line renderer object.
    /// </summary>
    /// <param name="lineRenderer">The line renderer.</param>
    public void SetCurrentLineRendererObject(GameObject lineRenderer)
    {
        LineRendererPrefab = lineRenderer;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Gets the current icon object.
    /// </summary>
    /// <returns>SafetyIconData.</returns>
    public SafetyIconData GetCurrentIconObject()
    {
        if (CurrentIconData != null)
            return CurrentIconData;

        Debug.LogError("Icon object not found.", this);
        return null;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Sets the current icon object.
    /// </summary>
    /// <param name="selectIconComponent">The select icon component.</param>
    public void SetCurrentIconObject(SelectIcon selectIconComponent)
    {
        if ((selectIconComponent == null) || (SelectIcon == selectIconComponent))
        {
            CurrentIconData = null;
            SelectIcon = null;
            SetCurrentAnnotationState(AnnotationState.Move);
            return;
        }
        else
        {
            SelectIcon = selectIconComponent;
            CurrentIconData = SelectIcon.IconData;
            SetCurrentAnnotationState(AnnotationState.Icon, selectIconComponent);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion

    /// <summary>
    /// Makes the annotation.
    /// </summary>
    public async Task MakeAnnotation()
    {
        touches = InputHelper.GetTouches();

        try
        {
            if (!annotationCancellationToken.Token.IsCancellationRequested)
            {
                await new WaitForUpdate();

                if (touches.Count == 1 && OverObject())
                {
                    if (CurrentLineBehaviour == null)
                    {
                        SetCurrentAnnotationState(currentAnnotationState);

                        NewLineRendererSave newSave = new NewLineRendererSave();
                        newSave.StorageType = NetworkStorageType.LineRenderer;
                        newSave.Color = ColorUtility.ToHtmlStringRGB(lineCol);
                        newSave.DeckID = CurDeckMap.DeckID;
                        newSave.IsHighlighter = (GetCurrentAnnotationState() == AnnotationState.Highlight).ToString();
                        newSave.Points = new Vector2[0];
                        newSave.PrefabName = "Gobblygook";
                        newSave.Props = GetCurrentPropertyState();
                        newSave.Thickness = Size.ToString();
                        newSave.PlayerID = Photon.Pun.PhotonNetwork.NickName;

                        CurrentLineBehaviour = lineFactory.Create(newSave);

                        await UpdateBrushPosition();
                    }
                    else if (CurrentLineBehaviour != null)
                    {
                        await UpdateBrushPosition();
                        await AddNewPoint(mousePos);
                    }
                }
                else if (touches.Count == 1 && !OverObject())
                {
                    CurrentLineBehaviour = null;
                }

                if (touches.Count == 0)
                {
                    if (CurrentLineBehaviour != null)
                    {
                        await SaveAndSendAnnotation();
                        CurrentLineBehaviour = null;
                    }
                }
            }
        }
        catch (Exception e)
        {
            await new WaitForUpdate();
            Debug.LogError("Error: " + e.Message + "                          " +
                "Stacktrace: " + e.StackTrace);

            annotationCancellationToken.Cancel();
            throw;
        }
        
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Makes the new icon.
    /// </summary>
    public async void MakeNewIcon()
    {
        touches = InputHelper.GetTouches();

        if (touches.Count == 1 && OverObject())
        {
            startedPlacing = true;
            SelectIcon.ToggleBracket(true);
            PlaceTheIcon();
        }

        if (touches.Count == 0 && startedPlacing && SpawnedNewIcon != null)
        {
            SpawnedNewIcon.GetComponent<Image>().raycastTarget = true;
            SpawnedNewIcon.transGesture.enabled = true;

            SelectIcon.ToggleBracket(false);

            if (SpawnedNewIcon.AssignedIconType != IconType.Standard)
            {
                StartCoroutine(NameTheIcon());
            }

            else
            {
                SpawnedNewIcon.FinalizeAndSend(true, false, NetworkEvent.Create);
                SpawnedNewIcon = null;
                SetCurrentIconObject(null);
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Places the icon.
    /// </summary>
    public void PlaceTheIcon()
    {
        if (SpawnedNewIcon == null)
        {
            parent = IsGAEditEnabled ? "GAHolder" : "LineHolder";

            NewIconSave newSave = new NewIconSave();
            if (string.IsNullOrEmpty(CurrentIconData.IconName))
            {
                Debug.LogError("Icon name empty");
                return;
            }
            newSave.StorageType = NetworkStorageType.Icon;
            newSave.PrefabName = CurrentIconData.IconName;
            newSave.DeckID = curDeckMap.DeckID;
            newSave.CurrentPosition = mousePos;
            newSave.Prop = currentPropertyState;

            SpawnedNewIcon = iconFactory.Create(CurrentIconData.PrefabReference, newSave);
            SpawnedNewIcon.GetComponent<Image>().raycastTarget = false;
            SpawnedNewIcon.IconData = CurrentIconData;
        }

        if (SpawnedNewIcon.transform.parent != ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent).transform)
        {
            SpawnedNewIcon.transform.parent = deckManager.GetHolder(currentPropertyState, curDeckMap.DeckID, NetworkClient.GetUserName());
        }

        UpdateBrushPosition();
        SpawnedNewIcon.SetPosition(mousePos);
        SpawnedNewIcon.IconSave.CurrentPosition = SpawnedNewIcon.thisRect.anchoredPosition;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Updates the brush position.
    /// </summary>
    public async Task UpdateBrushPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvasOfImageToMove.transform as RectTransform, touches[0].position, parentCanvasOfImageToMove.worldCamera, out pos);
        mousePos = parentCanvasOfImageToMove.transform.TransformPoint(pos);
        await new WaitForUpdate();
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Names the icon.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    public IEnumerator NameTheIcon()
    {

        if (SpawnedNewIcon.AssignedIconType == IconType.Chemical)
        {
            iconKeyBoardPromptHandler.gameObject.SetActive(true);
            iconKeyBoardPromptHandler.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.name);
            chemNameField.characterLimit = 16;
            yield return new WaitForEndOfFrame();
            keyboardManager.ChangeInputField(chemNameField);
        }

        if (SpawnedNewIcon?.AssignedIconType == IconType.Numbered)
        {
            iconKeyBoardPromptHandler.gameObject.SetActive(true);
            iconKeyBoardPromptHandler.ChangeKeyboardState(KeyBoardPromptHandler.KeyboardState.number);
            iconNumField.characterLimit = 2;
            yield return new WaitForEndOfFrame();
            keyboardManager.ChangeInputField(iconNumField);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Saves the icon.
    /// </summary>
    public async void SaveTheIcon()
    {
        if (SpawnedNewIcon.AssignedIconType == IconType.Chemical)
        {
            SpawnedNewIcon.SetNameOrNumber(chemNameField.text);
            chemNameField.text = "";

            if (chemNameField.GetComponent<InputFieldHighlight>().selected)
            {
                chemNameField.GetComponent<InputFieldHighlight>().FrameSelected();
            }
        }

        if (SpawnedNewIcon.AssignedIconType == IconType.Numbered)
        {
            SpawnedNewIcon.SetNameOrNumber(iconNumField.text);
            iconNumField.text = "";

            if (iconNumField.GetComponent<InputFieldHighlight>().selected)
            {
                iconNumField.GetComponent<InputFieldHighlight>().FrameSelected();
            }
        }

        keyboardManager.currentLine = null;

        SpawnedNewIcon.IconSave.CurrentPosition = SpawnedNewIcon.GetComponent<RectTransform>().anchoredPosition;

        SpawnedNewIcon.FinalizeAndSend(true, false, NetworkEvent.Create);
        SpawnedNewIcon = null;

        SetCurrentAnnotationState(AnnotationState.Move);

        StartCoroutine(ToggleIconPrompt());
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Toggles the icon prompt.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    public IEnumerator ToggleIconPrompt()
    {
        yield return new WaitForSeconds(.3f);

        iconKeyBoardPromptHandler.gameObject.SetActive(false);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Cancels the icon.
    /// </summary>
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
        Destroy(SpawnedNewIcon?.gameObject);
        iconKeyBoardPromptHandler.gameObject.SetActive(false);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Adds the new point.
    /// </summary>
    /// <param name="point">The point.</param>
    public async Task AddNewPoint(Vector3 point)
    {
        try
        {
            Debug.Log("Adding new point.");
            Vector3[] corners = new Vector3[4];
            RectTransform result = await curDeckMap.GetLocalHolder(currentPropertyState, NetworkClient.GetUserName());
            result.GetWorldCorners(corners);
            Debug.Log("Got new point corners.");

            Vector2 drawPoint = point - corners[0];

            drawPoint = drawPoint / canvasScaler.scaleFactor;

            if (Rotated)
            {
                drawPoint = new Vector2(drawPoint.y * -1, drawPoint.x);
            }

            await CurrentLineBehaviour.AddPoints(drawPoint);
            Debug.Log("New point added.");
        }
        catch (Exception e)
        {
            Debug.Log("Adding a new point caused an exception: " + e.Message);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Saves the and send annotation.
    /// </summary>
    public async Task SaveAndSendAnnotation()
    {
        CurrentLineBehaviour.FinalizeAndSend(true, false, NetworkEvent.Create);
        CurrentLineBehaviour = null;
        newAnnoString = "";
        CurDeckMap = null;
    }

    /// <summary>
    /// Changes the thickness.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    public void ChangeThickness(float thickness)
    {
        Size = thickness;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Overs the object.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool OverObject()
    {
        if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter != null)
        {
            if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.gameObject == CurDeckMap?.gameObject)
            {
                return true;
            }

            else
            {
                GameObject enteredGameObject = ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter;
                if (enteredGameObject.GetComponent<Deck>())
                {
                    CurDeckMap = ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.gameObject.GetComponent<Deck>();
                }

                else
                {
                    CurDeckMap = null;
                }

                if (CurrentLineBehaviour != null)
                {
                    SaveAndSendAnnotation();
                }

                CurrentLineBehaviour = null;
                return false;
            }
        }

        else
        {
            if (CurrentLineBehaviour != null)
            {
                SaveAndSendAnnotation();
            }

            CurDeckMap = null;
            CurrentLineBehaviour = null;
            return false;
        }
    }

    internal void MainMenu_OnGameStateChanged(Signal_MainMenu_OnGameStateChanged signal)
    {
        if (signal.State == GameState.Room)
            SetCurrentLineColor(GetLineColorBasedOnUser(NetworkClient.GetUserName().ToString()));
    }

    public static Color GetLineColorBasedOnUser(UserNameID user)
    {
        Color color;
        switch (user)
        {
            case UserNameID.SafetyCommandCenter:
                if (ColorUtility.TryParseHtmlString("#03B6FF", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.ForwardControlPoint:
                if (ColorUtility.TryParseHtmlString("#FF002C", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.IncidentCommandCenter:
                if (ColorUtility.TryParseHtmlString("#FF8B00", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.StagingArea:
                if (ColorUtility.TryParseHtmlString("#FB1EFF", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.EngineControlRoom:
                if (ColorUtility.TryParseHtmlString("#0B35E8", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.EvacuationControlCenter:
                if (ColorUtility.TryParseHtmlString("#B0FF15", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.ShoresideOperations:
                if (ColorUtility.TryParseHtmlString("#04a777", out color))
                    return color;
                else
                    return Color.black;

            default:
                return Color.black;
        }
    }

    internal Color GetLineColorBasedOnUser(string user)
    {
        UserNameID newUser = 0;
        Enum.TryParse(user, false, out newUser);

        Color color;
        switch (newUser)
        {
            case UserNameID.SafetyCommandCenter:
                if (ColorUtility.TryParseHtmlString("#03B6FF", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.ForwardControlPoint:
                if (ColorUtility.TryParseHtmlString("#FF002C", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.IncidentCommandCenter:
                if (ColorUtility.TryParseHtmlString("#E87F20", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.StagingArea:
                if (ColorUtility.TryParseHtmlString("#FB1EFF", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.EngineControlRoom:
                if (ColorUtility.TryParseHtmlString("#0B35E8", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.EvacuationControlCenter:
                if (ColorUtility.TryParseHtmlString("#B0FF15", out color))
                    return color;
                else
                    return Color.black;
            case UserNameID.ShoresideOperations:
                if (ColorUtility.TryParseHtmlString("#04a777", out color))
                    return color;
                else
                    return Color.black;

            default:
                return Color.black;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}
#endif