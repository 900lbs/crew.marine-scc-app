using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

[CustomEditor(typeof(XMLWriter)), CanEditMultipleObjects]
public class XMLWriterEditor : Editor
{
    public SerializedProperty
        xmlType_prop,
        mainPage_prop,
        timer_prop,
        overlay_prop,
        widget_close_prop,
        isolateDecks_prop,
        min_conf_prop,
        filePath_prop;

    void OnEnable()
    {
        xmlType_prop = serializedObject.FindProperty("xmlType");
        mainPage_prop = serializedObject.FindProperty("mainPage");
        timer_prop = serializedObject.FindProperty("timer");
        overlay_prop = serializedObject.FindProperty("overlay");
        widget_close_prop = serializedObject.FindProperty("widget_close");
        isolateDecks_prop = serializedObject.FindProperty("isolateDecks");
        min_conf_prop = serializedObject.FindProperty("min_conf");
        filePath_prop = serializedObject.FindProperty("filePath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(xmlType_prop);

        XMLWriter.XMLType st = (XMLWriter.XMLType)xmlType_prop.enumValueIndex;

        switch(st)
        {
            case XMLWriter.XMLType.MainPage:
                EditorGUILayout.PropertyField(mainPage_prop, new GUIContent("Main Page Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.Timers:
                EditorGUILayout.PropertyField(timer_prop, new GUIContent("Timer Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.Overlays:
                EditorGUILayout.PropertyField(overlay_prop, new GUIContent("Overlay Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.Widgets:
                EditorGUILayout.PropertyField(widget_close_prop, new GUIContent("Widget Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.Isolate:
                EditorGUILayout.PropertyField(isolateDecks_prop, new GUIContent("Isolate Decks Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.Minimize:
                EditorGUILayout.PropertyField(min_conf_prop, new GUIContent("Minimize Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.Close:
                EditorGUILayout.PropertyField(widget_close_prop, new GUIContent("Close Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;

            case XMLWriter.XMLType.CloseConfirmation:
                EditorGUILayout.PropertyField(min_conf_prop, new GUIContent("Close Confirmation Request"), true);
                EditorGUILayout.PropertyField(filePath_prop, new GUIContent("File Path"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

}
