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

using UnityEngine.EventSystems;

public class ExtendedStandaloneInputModule : StandaloneInputModule
{
    public static PointerEventData GetPointerEventData(int pointerId = -1)
    {
        PointerEventData eventData;
        _instance.GetPointerData(pointerId, out eventData, true);
        return eventData;
    }

    private static ExtendedStandaloneInputModule _instance;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
    }
}
