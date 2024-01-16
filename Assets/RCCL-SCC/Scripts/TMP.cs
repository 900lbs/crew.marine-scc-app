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

using UnityEngine;
using UnityEngine.UI;

public class TMP : MonoBehaviour
{
    void Update()
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<Image>().rectTransform.GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
        Debug.Log(newRect.Contains(Input.mousePosition));
        Debug.Log(Input.mousePosition - corners[0]);
    }
}
