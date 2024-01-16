using System;

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

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zenject;

public class LineBehavior : IShipNetworkObject
{
    #region Injections

    private INetworkClient networkClient;
    private AnnotationManager annotationManager;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(NetworkClient netClient, AnnotationManager anoMan,
        NewLineRendererSave lineSave, SignalBus signal)
    {
        networkClient = netClient;
        annotationManager = anoMan;
        lineRendSave = lineSave;
        _signalBus = signal;
    }

    #endregion Injections

    /*------------------------------------------------------------------------------------------------*/

    #region Variables

    [SerializeField]
    public NewLineRendererSave lineRendSave;

    public UILineRenderer lineRenderer;

    private UserNameID userNameID
    {
        get
        {
            UserNameID tryUser = 0;
            Enum.TryParse(lineRendSave.PlayerID, out tryUser);
            return tryUser;
        }
    }

    private RectTransform deleteButton_Rect;

    private Image thisImg;

    private Button deleteButton;

    private float MaxY;
    private float MinY;
    private float MaxX;
    private float MinX;

    private int curPoints = 0;

    #endregion Variables

    /*------------------------------------------------------------------------------------------------*/

    #region Overrides

    public override void Start()
    {
        lineRenderer = this.GetComponent<UILineRenderer>();

        if (lineRenderer.Points == null)
            Delete(false);
        _signalBus.Subscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnoMan_AnnotationStateChanged);
        _signalBus.Subscribe<Signal_AnnoMan_OnEraseAllOnProperty>(AnnoMan_EraseAllOnProperty);
        _signalBus.Subscribe<Signal_AnnoMan_OnToggleRoomProperty>(RoomPropertyActiveToggled);
        _signalBus.Subscribe<Signal_AnnoMan_OnRoomPropertyChanged>(RoomPropertyStateChanged);

        deleteButton_Rect = transform.GetChild(0).transform.GetComponent<RectTransform>();
        deleteButton = deleteButton_Rect.GetComponent<Button>();
        deleteButton.interactable = false;

        thisImg = deleteButton_Rect.GetComponent<Image>();
        thisImg.raycastTarget = false;

        lineRenderer.raycastTarget = false;

        deleteButton.onClick.AddListener(async () => await Delete(!annotationManager.GetCurrentPropertyState().HasFlag(ShipRoomProperties.GAOverlay)));

        State = (annotationManager.GetCurrentAnnotationState() == AnnotationState.Eraser) ? ActiveState.Enabled : ActiveState.Disabled;

        OnStateChange(); //Force a state change update since we always instantiate Disabled.

        if (lineRendSave.Props != ShipRoomProperties.GAOverlay)
            base.Start();
    }

    public override void OnStateChange()
    {
        switch (State)
        {
            case ActiveState.Disabled:
                deleteButton.interactable = false;
                break;

            case ActiveState.Enabled:
                deleteButton.interactable = true;
                break;

            case ActiveState.Selected:
                deleteButton.interactable = false;

                break;
        }
        thisImg.raycastTarget = (annotationManager.GetCurrentAnnotationState() == AnnotationState.Eraser);
        Debug.Log("Image raycast is set to " + thisImg.raycastTarget);
    }

    public override async Task Delete(bool sendNetwork)
    {
        if (sendNetwork && annotationManager.GetCurrentPropertyState() != ShipRoomProperties.GAOverlay)
        {
            NetworkAnnotationObject newNetObject = new NetworkAnnotationObject(
                NetworkEvent.Erase,
                lineRendSave.Props,
                lineRendSave.StorageType,
                PlayerID,
                ID,
                "null",
                true
            );
            await networkClient.SendNewNetworkEvent(newNetObject);
        }

        await base.Delete(sendNetwork);
    }

    #endregion Overrides

    /*------------------------------------------------------------------------------------------------*/

    #region Unity/Interface Functions

    private void Update()
    {
        if (lineRenderer.Points != null && lineRenderer.Points.Length > curPoints)
        {
            for (int i = 0; i < lineRenderer.Points.Length; i++)
            {
                if (i == 0)
                {
                    MaxY = lineRenderer.Points[i].y;
                    MinY = lineRenderer.Points[i].y;
                    MaxX = lineRenderer.Points[i].x;
                    MinX = lineRenderer.Points[i].x;
                }

                if (lineRenderer.Points[i].y > MaxY)
                {
                    MaxY = lineRenderer.Points[i].y;
                }

                if (lineRenderer.Points[i].y < MinY)
                {
                    MinY = lineRenderer.Points[i].y;
                }

                if (lineRenderer.Points[i].x > MaxX)
                {
                    MaxX = lineRenderer.Points[i].x;
                }

                if (lineRenderer.Points[i].x < MinX)
                {
                    MinX = lineRenderer.Points[i].x;
                }
            }

            CalculateButton();

            curPoints = lineRenderer.Points.Length;
        }
    }

    public void OnDestroy()
    {
        _signalBus.TryUnsubscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnoMan_AnnotationStateChanged);
        _signalBus.TryUnsubscribe<Signal_AnnoMan_OnEraseAllOnProperty>(AnnoMan_EraseAllOnProperty);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnRoomPropertyChanged>(RoomPropertyStateChanged);
        _signalBus.TryUnsubscribe<Signal_AnnoMan_OnToggleRoomProperty>(RoomPropertyActiveToggled);
    }

    #endregion Unity/Interface Functions

    /*------------------------------------------------------------------------------------------------*/

    #region Signal Handlers

    private void AnnoMan_AnnotationStateChanged(Signal_AnnoMan_OnAnnotationStateChanged signal)
    {
        if (GetIsEraserValid() && annotationManager.GetCurrentPropertyState().HasFlag(lineRendSave.Props))
        {
            State = ActiveState.Enabled;
        }
        else
        {
            if (signal.annotationState.HasFlag(AnnotationState.Move) && (lineRendSave.PlayerID.Equals(NetworkClient.GetUserName().ToString()) || NetworkClient.GetIsPlayerMasterClient()) &&
                annotationManager.GetCurrentPropertyState().HasFlag(lineRendSave.Props))
                State = ActiveState.Selected;
            else
                State = ActiveState.Disabled;
        }
    }

    private void AnnoMan_EraseAllOnProperty(Signal_AnnoMan_OnEraseAllOnProperty signal)
    {
        Debug.Log("Checking to see if this line renderer needs to be deleted | " + signal.User, this);
        if (lineRendSave.Props.HasFlag(signal.Property) && signal.User.HasFlag(userNameID))
        {
            Debug.Log("Found this line renderers user, deleting now.");

            Delete(false);
        }
        else
        {
            Debug.Log("Did not find this line renderer's user, not deleting.");
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private void RoomPropertyActiveToggled(Signal_AnnoMan_OnToggleRoomProperty props)
    {
        gameObject.SetActive(props.Property.HasFlag(lineRendSave.Props));
    }

    public void RoomPropertyStateChanged(Signal_AnnoMan_OnRoomPropertyChanged props)
    {
        State = (props.roomProperty.HasFlag(lineRendSave.Props)) ? ActiveState.Enabled : ActiveState.Disabled;
    }

    #endregion Signal Handlers

    /*------------------------------------------------------------------------------------------------*/

    #region Private Functions

    private bool GetIsEraserValid()
    {
        return ((NetworkClient.GetUserName().ToString() == PlayerID ||
                NetworkClient.GetIsPlayerMasterClient()) &&
            annotationManager.GetCurrentAnnotationState() == AnnotationState.Eraser);
    }

    private void CalculateButton()
    {
        Vector2 position = new Vector2(MinX - 5, MinY - 5);
        deleteButton_Rect.localPosition = position;

        Vector2 size = new Vector2((MaxX - MinX) + 10, (MaxY - MinY) + 10);
        deleteButton_Rect.sizeDelta = size;
    }

    public async Task AddPoints(Vector2 point)
    {
        Debug.Log("Adding points to LineBehaviour.");

        //await new WaitForBackgroundThread();

        List<Vector2> currentPoints = new List<Vector2>();
        currentPoints.AddRange(lineRenderer.Points);

        if (currentPoints.Contains(point))
        {
            return;
        }
        currentPoints.Add(point);
        lineRenderer.Points = currentPoints.ToArray();

        Vector2[] saveUpdatePoints = new Vector2[currentPoints.Count];

        for (int i = 0; i < currentPoints.Count; i++)
        {
            saveUpdatePoints[i] = lineRenderer.Points[i];
        }

        lineRendSave.Points = saveUpdatePoints;

        await new WaitForUpdate();
        Debug.Log("Points were successfully added to LineBehaviour.");
    }

    #endregion Private Functions

    /*------------------------------------------------------------------------------------------------*/

    #region Public Functions

    /// <summary>
    /// Finalizes the annotation, preps for network transfer and sends it.
    /// </summary>
    /// <param name="SendNetwork">If true, send network.</param>
    /// <param name="sendDirty">If true, the annotation will be sent instantly, otherwise it will be queued for sending.</param>
    /// <param name="netEvent">The type of network event this is.</param>
    /// <returns>A Task.</returns>
    public override async void FinalizeAndSend(bool SendNetwork, bool sendDirty, NetworkEvent netEvent)
    {
        if (lineRenderer.Points == null || lineRenderer.Points.Length < 1)
        {
            Debug.Log("Line renderer with no points detected, deleting now.");
            await Delete(false);
            return;
        }
        if (SendNetwork)
        {
            try
            {
                //await new WaitForBackgroundThread();
                string networkString = lineRendSave.GetSingleLine();

                switch (lineRendSave.Props)
                {
                    case ShipRoomProperties.Annotations:
                        NetworkAnnotationObject newObject = new NetworkAnnotationObject(
                            netEvent,
                            lineRendSave.Props,
                            StorageType,
                            Photon.Pun.PhotonNetwork.NickName,
                            ID,
                            networkString,
                            sendDirty
                        );

                        await networkClient.SendNewNetworkEvent(newObject);

                        break;

                    case ShipRoomProperties.GAOverlay:
                    case ShipRoomProperties.None:
                        break;
                }

                await new WaitForUpdate();

                Debug.Log("Sent annotation finalized.");
            }
            catch (System.Exception e)
            {
                await new WaitForUpdate();
                Debug.LogError("Error while trying to send LineRenderer over the network: " + e.InnerException + e.StackTrace);
                throw;
            }
        }
    }

    #endregion Public Functions

    /*------------------------------------------------------------------------------------------------*/
}