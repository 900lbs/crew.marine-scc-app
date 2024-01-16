using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Zenject;

using Object = UnityEngine.Object;
#if SCC_2_5

public class SelectIcon : UI_Button, ILateDisposable
{
    public AnnotationManager AnnotationMan;

    IMemoryPool _pool;



    [Inject]
    public void Construct(AnnotationManager annotationMan, IMemoryPool pool)
    {
        AnnotationMan = annotationMan;
        _pool = pool;
    }
    public SafetyIconData IconData;

    public Vector2 PrefabTransform;

    public Image bracket;


    /*----------------------------------------------------------------------------------------------------------------------------*/


    public void Start()
    {
        
        bracket = transform.GetChild(0).GetComponent<Image>();
        State = ActiveState.Enabled;

        _signalBus.Subscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnotationStateChanged);
        _signalBus.Subscribe<Signal_AnnoMan_OnRoomPropertyChanged>(AnnotationManager_CurrentPropertyState);
    }

/*----------------------------------------------------------------------------------------------------------------------------*/

    public void LateDispose()
    {
        //AnnotationManager.OnCurrentPropertyState -= AnnotationManager_CurrentPropertyState;
        _signalBus.Unsubscribe<Signal_AnnoMan_OnAnnotationStateChanged>(AnnotationStateChanged);
        _signalBus.Unsubscribe<Signal_AnnoMan_OnRoomPropertyChanged>(AnnotationManager_CurrentPropertyState);
    }

    protected override void OnStateChange()
    {
        switch (State)
        {
            case ActiveState.Disabled:
                gameObject.SetActive(false);
                break;

            case ActiveState.Enabled:
                gameObject.SetActive(true);
                ToggleBracket(false);
                break;

            case ActiveState.Selected:
                ToggleBracket(true);
                break;

        }
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        AnnotationMan.SetCurrentIconObject(this);
    }

    void AnnotationManager_CurrentPropertyState(Signal_AnnoMan_OnRoomPropertyChanged current)
    {
        //gameObject.SetActive((IconData.AssignedProperties & current.roomProperty) == current.roomProperty);
    }

    public void AnnotationStateChanged(Signal_AnnoMan_OnAnnotationStateChanged stateData)
    {
        SelectIcon newIcon = stateData.selectIcon as SelectIcon;
        if (newIcon != null)
        {
            if (this == newIcon && stateData.annotationState == AnnotationState.Icon)
            {
                State = ActiveState.Selected;
                return;
            }
        }

        State = ActiveState.Enabled;
    }

    public void ToggleBracket(bool value)
    {
        bracket.enabled = value;
    }

    public new class Factory : PlaceholderFactory<Object, SafetyIconData, SelectIcon>
    { }
}

#elif !SCC_2_5
public class SelectIcon : MonoBehaviour
{
    public DrawOnDeck drawOnDeck;

    public GameObject iconSelection;

    public Button selectionButton;

    public Image bracket;

	// Use this for initialization
	void Start ()
    {
        drawOnDeck = FindObjectOfType<DrawOnDeck>();
        selectionButton = this.GetComponent<Button>();
        selectionButton.onClick.AddListener(IconPicker);

        bracket = this.transform.GetChild(0).GetComponent<Image>();
	}
	
	public void IconPicker()
    {
        if(drawOnDeck.placeIcon)
        {
            drawOnDeck.toggleHighlighter(drawOnDeck.moveBH.GetComponent<Button>());
        }


        if (!drawOnDeck.placeIcon)
        {
            ToggleBracket();
            drawOnDeck.iconToPlace = iconSelection;
            drawOnDeck.selectIcon = this;
            drawOnDeck.placeIcon = true;
            drawOnDeck.toggleHighlighter(selectionButton);
        }

        
    }

    public void ToggleBracket()
    {
        if(!bracket.enabled)
        {
            bracket.enabled = true;
        }

        else
        {
            bracket.enabled = false;
        }
    }
}
#endif