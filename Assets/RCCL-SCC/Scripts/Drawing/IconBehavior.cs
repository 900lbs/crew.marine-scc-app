using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

using Object = UnityEngine.Object;
using ContainerSources = Zenject.ZenAutoInjecter.ContainerSources;
using System.Threading.Tasks;

#if SCC_2_5
public class IconBehavior : IShipNetworkObject
{
    AnnotationManager annotationManager;
    DeckManager deckManager;
    INetworkClient networkClient;
    SignalBus _signalBus;

    [Inject]
    public void Construct(INetworkClient netClient, AnnotationManager annoManager, DeckManager deckMan, SignalBus signal)
    {
        networkClient = netClient;
        annotationManager = annoManager;
        deckManager = deckMan;
        _signalBus = signal;
    }

    public SafetyIconData IconData;
    public NewIconSave IconSave;

    public Button deleteButton;

    public TransformGesture transGesture;

    public Image thisImg;

    List<Touch> touches;

    public Transform parentObject;
    public string parent;

    public TextMeshProUGUI nameOrNumber;

    public IconType AssignedIconType;

    public string IconText;

    [NonSerialized]
    public RectTransform thisRect;

    UserNameID userNameID
    {
        get
        {
            UserNameID tryUser = 0;
            Enum.TryParse(PlayerID, out tryUser);
            return tryUser;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public override void Start()
    {
        transGesture = GetComponent<TransformGesture>();
        transGesture.UseUnityEvents = true;

        thisImg = GetComponent<Image>();

        thisRect = GetComponent<RectTransform>();

        if (IconData.AssignedProperties != ShipRoomProperties.GAOverlay)
            base.Start();

        _signalBus.Subscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnoMan_AnnotationStateChanged);
        _signalBus.Subscribe<Signal_AnnoMan_OnEraseAllOnProperty>(AnnoMan_EraseAllOnProperty);
        _signalBus.Subscribe<Signal_AnnoMan_OnToggleRoomProperty>(RoomPropertyActiveToggled);
        _signalBus.Subscribe<Signal_AnnoMan_OnRoomPropertyChanged>(RoomPropertyStateChanged);

        transGesture.OnTransformComplete.AddListener(TransformGesture_OnTransformComplete);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private void Update()
    {
        if (thisRect.localPosition.z != 0)
            thisRect.localPosition = new Vector3(thisRect.localPosition.x, thisRect.localPosition.y, 0);

        if (State != ActiveState.Enabled)
        {
            if (IconData.AssignedProperties == ShipRoomProperties.GAOverlay && networkClient.IsMasterClient)
            {
                touches = InputHelper.GetTouches();

                if (touches.Count == 1 && transGesture.State == TouchScript.Gestures.Gesture.GestureState.Changed)
                {
                    if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent) != null && parentObject != ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent).transform)
                    {
                        SetParent();
                    }
                }

                if (touches.Count == 2)
                {
                    transGesture.enabled = false;
                }

                if (touches.Count < 2 && !transGesture.enabled && annotationManager.GetCurrentPropertyState().HasFlag(IconSave.Prop))
                {
                    transGesture.enabled = true;
                }
            }

            if (IconData.AssignedProperties != ShipRoomProperties.GAOverlay)
            {
                touches = InputHelper.GetTouches();

                if (touches.Count == 1 && transGesture.State == TouchScript.Gestures.Gesture.GestureState.Changed)
                {
                    if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.GetComponent<Deck>() != null && parentObject != ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.GetComponentInChildren<DeckAnnotationHolder>().transform)
                    {
                        SetParent();
                    }
                }

                if (touches.Count == 2)
                {
                    transGesture.enabled = false;
                }

                if (touches.Count < 2 && !transGesture.enabled && annotationManager.GetCurrentPropertyState().HasFlag(IconSave.Prop))
                {
                    transGesture.enabled = true;
                }
            }
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnoMan_AnnotationStateChanged);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnEraseAllOnProperty>(AnnoMan_EraseAllOnProperty);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnRoomPropertyChanged>(RoomPropertyStateChanged);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnToggleRoomProperty>(RoomPropertyActiveToggled);

        transGesture.OnTransformComplete.RemoveListener(TransformGesture_OnTransformComplete);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    async void TransformGesture_OnTransformComplete(Gesture gesture)
    {
        IconSave.CurrentPosition = GetComponent<RectTransform>().anchoredPosition;
        //Debug.Log("<color=cyan>Icon's current position: " + IconSave.CurrentPosition + "</color>", this);
        IconSave.DeckID = GetComponentInParent<Deck>().DeckID;
        FinalizeAndSend(true, false, NetworkEvent.Edit);

        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
            Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log("Sending Edit across the network at positions: " + IconSave.CurrentPosition, this);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void Initialize()
    {
        name = IconData.IconName;
        AssignedIconType = IconData.AssignedType;

        transGesture = GetComponent<TransformGesture>();
        thisImg = GetComponent<Image>();
        deleteButton = GetComponent<Button>();

        deleteButton.onClick.AddListener(async () => await Delete(true));
        transGesture.enabled = false;
        thisImg.sprite = IconData.IconImage;

        if (AssignedIconType != IconType.Standard)
        {
            nameOrNumber = this.GetComponentInChildren<TextMeshProUGUI>();
            nameOrNumber.text = IconSave.Text;
        }

        if (transform.position.z > 0)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        deleteButton.interactable = false;
        thisImg.raycastTarget = false;

        if (thisImg == null)
            Debug.LogError("Image is null.");

        if (IconData.IconImage == null)
            Debug.LogError("The sprite is null.");

        //Ghetto
        if (GetIsEraserValid() && annotationManager.GetCurrentPropertyState().HasFlag(IconSave.Prop))
        {
            State = ActiveState.Enabled;
        }
        else
        {
            if (annotationManager.GetCurrentAnnotationState().HasFlag(AnnotationState.Move) && (IconSave.PlayerID.Equals(NetworkClient.GetUserName().ToString()) || NetworkClient.GetIsPlayerMasterClient()))
                State = ActiveState.Selected;

            else
                State = ActiveState.Disabled;

        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public async override void Edit(object value)
    {
        NewIconSave convertedNetworkObject = new NewIconSave();
        string data = "";
        try
        {
            data = (string) value;
            if (!await convertedNetworkObject.SetBySingleLine(data, ""))
                return;

            IconSave = convertedNetworkObject;

            UserNameID user;
            Enum.TryParse(IconSave.PlayerID, out user);

            RectTransform parentTransform = deckManager.GetHolder(convertedNetworkObject.Prop, convertedNetworkObject.DeckID, user);
            RectTransform thisTransform = GetComponent<RectTransform>();

            thisTransform.SetParent(parentTransform, false);

            thisRect.anchoredPosition = (convertedNetworkObject.CurrentPosition);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

            Debug.Log("<color=cyan>Icon edited vector: " + convertedNetworkObject.CurrentPosition + "</color>");

            thisTransform.localScale = IconData.GetScaleFromPrefab();
        }
        catch
        {
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public override void OnStateChange()
    {
        //Debug.Log("Icon's state was changed to: " + State.ToString(), this);
        switch (State)
        {
            case ActiveState.Disabled:
                deleteButton.interactable = false;
                break;

            case ActiveState.Enabled:
                deleteButton.interactable = true;
                thisImg.raycastTarget = true;
                break;
            case ActiveState.Selected:
                deleteButton.interactable = false;
                transGesture.enabled = true;
                thisImg.raycastTarget = true;
                break;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void AnnoMan_AnnotationStateChanged(Signal_AnnoMan_OnAnnotationStateChanged signal)
    {
        if (GetIsEraserValid() && annotationManager.GetCurrentPropertyState().HasFlag(IconSave.Prop))
        {
            State = ActiveState.Enabled;
        }
        else
        {
            if (signal.annotationState.HasFlag(AnnotationState.Move) && (IconSave.PlayerID.Equals(NetworkClient.GetUserName().ToString()) || NetworkClient.GetIsPlayerMasterClient()) &&
                annotationManager.GetCurrentPropertyState().HasFlag(IconSave.Prop))
                State = ActiveState.Selected;

            else
                State = ActiveState.Disabled;

        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void AnnoMan_EraseAllOnProperty(Signal_AnnoMan_OnEraseAllOnProperty signal)
    {
        if (IconData.AssignedProperties.HasFlag(signal.Property) && signal.User.HasFlag(userNameID))
            Delete(false);

    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    void RoomPropertyActiveToggled(Signal_AnnoMan_OnToggleRoomProperty props)
    {
        gameObject.SetActive(props.Property.HasFlag(IconSave.Prop));
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void RoomPropertyStateChanged(Signal_AnnoMan_OnRoomPropertyChanged props)
    {
        //Debug.Log("Icon's disabled button set to " + (props.roomProperty.HasFlag(IconSave.Prop)), this);
        //gameObject.SetActive(props.roomProperty == IconData.AssignedProperties);
        transGesture.enabled = (props.roomProperty.HasFlag(IconSave.Prop));
        State = (props.roomProperty.HasFlag(IconSave.Prop)) ? ActiveState.Enabled : ActiveState.Disabled;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;

        if (thisRect == null)
            thisRect = GetComponent<RectTransform>();

        if (parentObject != null)
        {
            if (thisRect.GetSiblingIndex() != parentObject?.childCount - 1)
            {
                thisRect.SetAsLastSibling();
            }
        }
        IconSave.CurrentPosition = thisRect.anchoredPosition;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void SetNameOrNumber(string value)
    {
        nameOrNumber.text = value;
        IconSave.Text = value;
        IconText = value;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public void SetParent()
    {
        Debug.Log(ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.name + " was found, touched. Giggity");
        Deck newDeck = ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.GetComponent<Deck>();

        if(parentObject == null)
            Debug.Log("<color=red> Parent Object is Null.</color>");
        if(newDeck == null)
            Debug.Log("<color=red> newDeck is Null.</color>");
        if(deckManager == null)
            Debug.Log("<color=red> deckManager is Null.</color>");

        parentObject = deckManager.GetHolder(IconSave.Prop, newDeck.DeckID, userNameID);
        this.transform.parent = parentObject;
        //Debug.Log("<color=yellow> Icon received a new parent:" + parentObject.name + "</color>", this);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public override async void FinalizeAndSend(bool SendNetwork, bool sendDirty, NetworkEvent netEvent)
    {
        try
        {
            if (SendNetwork)
            {
                Debug.Log("Finalizing and sending a new icon.");
                await new WaitForBackgroundThread();
                IconSave.CurrentPosition.z = 0;
                string networkString = IconSave.GetSingleLine();

                switch (IconSave.Prop)
                {
                    case ShipRoomProperties.Annotations:
                        NetworkAnnotationObject newObject = new NetworkAnnotationObject(
                            netEvent,
                            IconSave.Prop,
                            NetworkStorageType.Icon,
                            Photon.Pun.PhotonNetwork.NickName,
                            ID,
                            networkString,
                            sendDirty);

                        networkClient?.SendNewNetworkEvent(newObject);
                        break;

                    case ShipRoomProperties.GAOverlay:
                    case ShipRoomProperties.None:
                        break;
                }

                await new WaitForUpdate();
            }
        }

        catch (SystemException e)
        {
            await new WaitForUpdate();
            Debug.LogError("Error trying to send icon over the network: " + e.InnerException + e.StackTrace);
            throw;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    bool GetIsEraserValid()
    {
        return ((NetworkClient.GetUserName().ToString() == PlayerID || NetworkClient.GetIsPlayerMasterClient()) &&
            annotationManager.GetCurrentAnnotationState() == AnnotationState.Eraser);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public override async Task Delete(bool sendNetwork)
    {

        if (sendNetwork && annotationManager.GetCurrentPropertyState() != ShipRoomProperties.GAOverlay)
        {
            NetworkAnnotationObject newNetObject = new NetworkAnnotationObject(
                NetworkEvent.Erase,
                annotationManager.GetCurrentPropertyState(),
                NetworkStorageType.Icon,
                Photon.Pun.PhotonNetwork.NickName,
                ID,
                " ",
                true
            );
            await networkClient.SendNewNetworkEvent(newNetObject);
        }

        await base.Delete(sendNetwork);
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public class Factory : PlaceholderFactory<Object, NewIconSave, IconBehavior> { }
}

#elif !SCC_2_5
public class IconBehavior : MonoBehaviour
{
    public Button deleteButton;

    public DrawOnDeck master;

    public TransformGesture transGesture;

    public Image thisImg;

    List<Touch> touches;

    public Transform parentObject;
    public string parent;

    public TextMeshProUGUI nameOrNumber;

    public enum IconType
    {
        normal,
        chemical,
        numbered
    }

    public IconType type;

    public bool isGA;

    public string iconText;

    public void Start()
    {
        transGesture = this.GetComponent<TransformGesture>();
        deleteButton = this.GetComponent<Button>();
        deleteButton.onClick.AddListener(() => Delete(true));
        thisImg = this.GetComponent<Image>();
        transGesture.enabled = false;

        if (type != IconType.normal)
        {
            nameOrNumber = this.GetComponentInChildren<TextMeshProUGUI>();
            nameOrNumber.text = iconText;
        }
    }

    private void Update()
    {
        CheckForErase();

        if (isGA && !master.editGA)
        {
            transGesture.enabled = false;
        }

        if (isGA && master.editGA)
        {
            transGesture.enabled = true;

            touches = InputHelper.GetTouches();

            if (touches.Count == 1 && transGesture.State == TouchScript.Gestures.Gesture.GestureState.Changed)
            {
                if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent) != null && parentObject != ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent).transform)
                {
                    SetParent();
                }
            }

            if (touches.Count == 2)
            {
                transGesture.enabled = false;
            }

            if (touches.Count < 2 && !transGesture.enabled)
            {
                transGesture.enabled = true;
            }
        }

        if (!isGA)
        {
            touches = InputHelper.GetTouches();

            if (touches.Count == 1 && transGesture.State == TouchScript.Gestures.Gesture.GestureState.Changed)
            {
                if (ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent) != null && parentObject != ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent).transform)
                {
                    SetParent();
                }
            }

            if (touches.Count == 2)
            {
                transGesture.enabled = false;
            }

            if (touches.Count < 2 && !transGesture.enabled)
            {
                transGesture.enabled = true;
            }
        }

    }

    public void SetParent()
    {
        parentObject = ExtendedStandaloneInputModule.GetPointerEventData(touches[0].fingerId).pointerEnter.transform.Find(parent).transform;
        this.transform.parent = parentObject;
    }

    public void CheckForErase()
    {
        if (master.erase)
        {
            deleteButton.interactable = true;
            transGesture.enabled = false;
        }

        if (!master.erase)
        {
            deleteButton.interactable = false;
            transGesture.enabled = true;
        }
    }

    public void Delete(bool sendNetwork)
    {
        Destroy(this.gameObject);
    }
}
#endif