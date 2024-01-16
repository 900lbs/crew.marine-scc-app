using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Behaviors;

#if SCC_2_5
public class WidgetMoveMask : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScreenTransformGesture screenTransform;
    public Transformer transformer;

    public void OnPointerEnter(PointerEventData args)
    {
        StopMovingWidget();
    }

    public void OnPointerExit(PointerEventData args)
    {
        MoveWidget();
    }

    public void MoveWidget()
    {
        screenTransform.enabled = true;
        transformer.enabled = true;
    }

    public void StopMovingWidget()
    {
        screenTransform.enabled = false;
        transformer.enabled = false;
    }
}

#elif !SCC_2_5
public class WidgetMoveMask : MonoBehaviour
{
    public ScreenTransformGesture screenTransform;
    public Transformer transformer;

    public void MoveWidget()
    {
        screenTransform.enabled = true;
        transformer.enabled = true;
    }

    public void StopMovingWidget()
    {
        screenTransform.enabled = false;
        transformer.enabled = false;
    }
}
#endif