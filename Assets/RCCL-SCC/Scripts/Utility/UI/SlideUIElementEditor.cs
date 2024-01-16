#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(SlideUIElement))]
public class SlideUIElementEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var customScript = target as SlideUIElement;

        customScript.elementToSlide = EditorGUILayout.ObjectField("Element To Slide", customScript.elementToSlide, typeof(RectTransform), true) as RectTransform;
        customScript.inPos = EditorGUILayout.Vector2Field("In Position", customScript.inPos);
        customScript.outPos = EditorGUILayout.Vector2Field("Out Position", customScript.outPos);
        customScript.buttonToSlide = EditorGUILayout.ObjectField("Button To Slide", customScript.buttonToSlide, typeof(Button), true) as Button;

        customScript.iconToChange = GUILayout.Toggle(customScript.iconToChange, "Has Icon");

        if (customScript.iconToChange)
        {
            customScript.icon = EditorGUILayout.ObjectField("Icon Image", customScript.icon, typeof(Image), true) as Image;
            customScript.inCol = EditorGUILayout.ColorField("In Color", customScript.inCol);
            customScript.outCol = EditorGUILayout.ColorField("Out Color", customScript.outCol);
        }
    }
}
#endif

