using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Behaviors;
using TMPro;

public class WidgetToDock : MonoBehaviour
{
    public RectTransform widgetTransform;

    public Vector2 dockRange;
    public Vector2 curDockedPos;

    public DockWidgets dockWidgets;

    public bool docked;
    public bool visible;

    public ScreenTransformGesture screenTransformGesture;
    public Transformer transformer;

    public Color activeColor;
    public Color inactiveColor;

    public Image outlineImage;
    public TextMeshProUGUI dockText;

    public Button buttonForDocking;

    // Use this for initialization
    void Start ()
    {
        widgetTransform = this.GetComponent<RectTransform>();

        buttonForDocking.onClick.AddListener(DockWidget);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(dockRange.x == dockWidgets.dockPos.x)
        {
            dockRange.x = dockRange.x + 414;
        }

        if(!docked && visible)
        {
            if(outlineImage.color != activeColor)
            {
                ButtonColorChange(activeColor, true);
            }
            
            if (widgetTransform.localPosition.x < dockRange.x)
            {
                if (widgetTransform.localPosition.y < dockRange.y)
                {
                    DockWidget();
                }
            }
        }
		
        if(docked && visible)
        {
            if(outlineImage.color != inactiveColor)
            {
                ButtonColorChange(inactiveColor, false);
            }
            
            if (widgetTransform.localPosition.x != curDockedPos.x || widgetTransform.localPosition.y != curDockedPos.y)
            {
                UnDockWidget();

                docked = false;

                curDockedPos = Vector2.zero;
            }
        }

        if(!visible)
        {
            if (outlineImage.color != inactiveColor)
            {
                ButtonColorChange(inactiveColor, false);
            }
        }
	}

    public void DockWidget()
    {
        //screenTransformGesture.enabled = false;
        //transformer.enabled = false;

        dockWidgets.DockWidget(widgetTransform);

        docked = true;

        curDockedPos = widgetTransform.localPosition;
        buttonForDocking.interactable = false;
    }

    public void UnDockWidget()
    {
        if(docked)
        {
            dockWidgets.UnDockWidget(widgetTransform);
            buttonForDocking.interactable = true;
            docked = false;
        }
    }

    public void ButtonColorChange(Color newColor, bool state)
    {
        outlineImage.color = newColor;
        dockText.color = newColor;
        buttonForDocking.interactable = state;
    }
}
