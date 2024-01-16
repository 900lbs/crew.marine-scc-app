using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckAnnotationHolder : MonoBehaviour
{
    public RectTransform Rect;

    public UserNameID User;

    public List<IShipNetworkObject> Annotations;

    void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }
}
