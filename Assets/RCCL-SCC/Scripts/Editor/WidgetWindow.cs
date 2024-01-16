using System;
using System.Collections.Generic;
using PrimitiveFactory.ScriptableObjectSuite;
using UnityEditor;
using UnityEngine;

#if SCC_2_5
public class WidgetWindow : ScriptableObjectEditorWindow<Widget>
{
    private List<UnityEngine.Object> m_AllObjects;

    public static List<UnityEngine.Object> GetAllRealtimeComponents()
    {
        List<UnityEngine.Object> res = new List<UnityEngine.Object>();

        res.AddRange(GameObject.FindObjectsOfType(typeof(WidgetRuntime)));
        res.AddRange(GameObject.FindObjectsOfType(typeof(Button_ControlWidget)));

        return res;
    }

    // Name of the object (Display purposes)
    protected override string c_ObjectName { get { return "Widget"; } }
    // Relative path from Project Root
    protected override string c_ObjectFullPath { get { return "Assets/Scenes/Experimental/Scripts/Scriptable Objects/Widgets"; } }

    [MenuItem("RCCL/Widget Window")]
    public static void ShowWindow()
    {
        WidgetWindow window = GetWindow<WidgetWindow>("Widget Editor");
        window.Show();
    }

    protected override void DrawEditor(Widget widget)
    {
        base.DrawEditor(widget);

        m_AllObjects = GetAllRealtimeComponents();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Scene objects referencing this widget.", EditorStyles.centeredGreyMiniLabel);
        for (int i = 0; i < m_AllObjects.Count; i++)
        {
            if (m_AllObjects[i].GetType() == typeof(WidgetRuntime))
            {
                WidgetRuntime thisWidgetRuntime = (WidgetRuntime)m_AllObjects[i];

                if (thisWidgetRuntime.CurrentWidget == m_CurrentObject)
                {
                    UnityEngine.Object displayObject = m_AllObjects[i];

                    displayObject = EditorGUILayout.ObjectField("Runtime: ", displayObject, typeof(object), true);
                }

                continue;
            }

            if (m_AllObjects[i].GetType() == typeof(Button_ControlWidget))
            {
                Button_ControlWidget thisWidgetRuntime = (Button_ControlWidget)m_AllObjects[i];

                if (thisWidgetRuntime.CurrentWidget == m_CurrentObject)
                {
                    UnityEngine.Object displayObject = m_AllObjects[i];

                    displayObject = EditorGUILayout.ObjectField(thisWidgetRuntime.AssignedWidgetInteraction.ToString() + ": ", displayObject, typeof(object), true);
                }

                continue;
            }
        }

        EditorGUILayout.EndVertical();

    }

}

#endif