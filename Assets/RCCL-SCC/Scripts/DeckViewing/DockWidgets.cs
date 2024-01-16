using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockWidgets : MonoBehaviour
{
    public List<RectTransform> dockedWidgets;

    public Vector2 dockPos;

    public float initialX = 414f;

    public void DockWidget(RectTransform widget)
    {
        dockedWidgets.Add(widget);

        if(dockedWidgets.Count > 1)
        {
            dockPos.x = dockPos.x + initialX;
        }
        
        widget.localPosition = dockPos;
    }

    public void UnDockWidget(RectTransform widget)
    {
        if (dockPos.x > -1920)
        {
            dockPos.x -= initialX;
            //Debug.Log("Done " + dockPos.x);
        }

        dockedWidgets.Remove(widget);

        reOrderWidgets(dockedWidgets);
    }

    public void reOrderWidgets(List<RectTransform> widgets)
    {

        dockPos.x = -1920;


        foreach (RectTransform ReWidget in widgets)
        {
            ReWidget.GetComponent<WidgetToDock>().curDockedPos = dockPos;
            ReWidget.localPosition = dockPos;

            if(widgets.Count > 1 && ReWidget != widgets[widgets.Count - 1])
            {
                dockPos.x += initialX;
            }
            //dockPos.x = dockPos.x - initialX;
        }
    }
}
