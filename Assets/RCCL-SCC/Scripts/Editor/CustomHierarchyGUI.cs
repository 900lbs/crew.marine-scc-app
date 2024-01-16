using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

/// <summary>
/// Created By: Jonathon Wigley - 7/2/2018
/// Last Edited By: Jonathon Wigley - 7/6/2018
///
/// Purpose:
/// Custom inspector window for the hiearchy
/// Preferences on what is shown can be edited through Unity in Edit->Preferences->Custom Hierarchy
/// <summary>

[InitializeOnLoad]
[System.Serializable]
public class CustomHierarchyGUI
{
    private const float fadedInHierarchyAlpha = .4f;
	private static float currentRectOffset;

	// Constructor
	static CustomHierarchyGUI()
	{
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
	}

	// Function that draws onto the GUI of a hierarchy object
	// Additive, not a replacement for the GUI
	private static void HierarchyOnGUI(int instanceID, Rect position)
	{
		bool UseActiveToggle = EditorPrefs.GetBool("ActiveToggleKey");
		bool UseLayerSelect = EditorPrefs.GetBool("LayerSelectKey");
		bool UseTagSelect = EditorPrefs.GetBool("TagSelectKey");
		currentRectOffset = 0;

		GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if(obj == null) return;

		EditorGUI.BeginChangeCheck();

		bool active = false;
		string tag = "";
		int layerIndex = 0;

		if(UseActiveToggle)
			active = ActiveToggle(position, obj);
		if(UseLayerSelect)
			layerIndex = LayerSelect(position, obj);
		if(UseTagSelect)
			tag = TagSelect(position, obj);

		if(EditorGUI.EndChangeCheck())
		{
			if(UseActiveToggle && active != obj.activeSelf)
			{
				Undo.RecordObject(obj, "Set Active: " + active);
				obj.SetActive(active);
			}

			if(UseTagSelect && obj.tag.CompareTo(tag) != 0)
			{
				Undo.RecordObject(obj, "Change Tag: " + tag);
				obj.tag = tag;
			}

			// Check if the layer was changed
			if(UseLayerSelect && obj.layer != layerIndex)
			{
				Undo.RecordObject(obj, "Change Layer: " + layerIndex);
				if(obj.transform.childCount > 0)
				{
					int result = EditorUtility.DisplayDialogComplex( "Change Layer",
					"Do you want to set layer to " + layerIndex + " for all child objects as well?",
					"Yes, change children",
					"No, this object only",
					"Cancel" );

					// Selected yes
					if(result == 0)
						ChangeChildLayers(obj, layerIndex);

					// Selected only this object
					else if(result == 1)
						obj.layer = layerIndex;
				}

				else
				{
					obj.layer = layerIndex;
				}
			}
		}
	}

	// Draw the active toggle
	private static bool ActiveToggle(Rect position, GameObject obj)
	{
		// Draw the active toggle rect
		Rect toggleRect = new Rect(position);
		toggleRect.xMin = position.xMax - currentRectOffset - 20;
		currentRectOffset += 20;

		// Return the result of the toggle
		return GUI.Toggle(toggleRect, obj.activeInHierarchy, GUIContent.none);
	}

	// Draw the layer selection popup
	private static int LayerSelect(Rect position, GameObject obj)
	{
		// Color the popup
		Color backgroundColor = GUI.backgroundColor;
		Color contentColor = GUI.contentColor;
		GUI.backgroundColor = new Color(1f, 1f, 1f, obj.activeInHierarchy ? 1f : fadedInHierarchyAlpha);
		GUI.contentColor = new Color(1f, 1f, 1f, obj.activeInHierarchy ? 1f : fadedInHierarchyAlpha);

		// Draw the layer select rect
		Rect layerRect = new Rect(position);
		layerRect.xMin = position.xMax - currentRectOffset - 70f;
		currentRectOffset += 70;
		layerRect.width = 65;

		// Layer Selection
		int layerIndex = EditorGUI.LayerField(layerRect, obj.layer);

		// Reset the color
		GUI.backgroundColor = backgroundColor;
		GUI.contentColor = contentColor;

		// Return the layer index
		return layerIndex;
	}

	// Draw the Tag Selection popup
	private static string TagSelect(Rect position, GameObject obj)
	{
		// Color the popup
		Color backgroundColor = GUI.backgroundColor;
		Color contentColor = GUI.contentColor;
		GUI.backgroundColor = new Color(1f, 1f, 1f, obj.activeInHierarchy ? 1f : fadedInHierarchyAlpha);
		GUI.contentColor = new Color(1f, 1f, 1f, obj.activeInHierarchy ? 1f : fadedInHierarchyAlpha);

		// Draw the tag select rect
		Rect tagRect = new Rect(position);
		tagRect.xMin = position.xMax - currentRectOffset - 70f;
		currentRectOffset += 70f;
		tagRect.width = 65;

		// Tag Selection
		string tag = EditorGUI.TagField(tagRect, obj.tag);

		// Reset the color
		GUI.backgroundColor = backgroundColor;
		GUI.contentColor = contentColor;

		// Return the tag index
		return tag;
	}

	// Changes the layer of all children to the given layer
	private static void ChangeChildLayers(GameObject obj, int layerIndex)
	{
		Undo.RecordObject(obj, "Change Layer: " + layerIndex);
		obj.layer = layerIndex;
		foreach(Transform child in obj.transform)
		{
			ChangeChildLayers(child.gameObject, layerIndex);
		}
	}
}

public class HierarchyPrefs : MonoBehaviour
{
    // Have we loaded the prefs yet
    private static bool prefsLoaded = false;

    // The Preferences
    public static bool bUseActiveToggle = true;
	public static bool bUseLayerSelect = true;
	public static bool bUseTagSelect = true;

    // Add preferences section named "My Preferences" to the Preferences Window
    [PreferenceItem("Custom Hierarchy")]

    public static void PreferencesGUI()
    {
        // Load the preferences
        if (!prefsLoaded)
        {
            bUseActiveToggle = EditorPrefs.GetBool("ActiveToggleKey", true);
            bUseLayerSelect = EditorPrefs.GetBool("LayerSelectKey", true);
            bUseTagSelect = EditorPrefs.GetBool("TagSelectKey", true);
            prefsLoaded = true;
        }

        // Preferences GUI
        bUseActiveToggle = EditorGUILayout.Toggle("Use Active Toggle", bUseActiveToggle);
        bUseLayerSelect = EditorGUILayout.Toggle("Use Layer Select", bUseLayerSelect);
        bUseTagSelect = EditorGUILayout.Toggle("Use Tag Select", bUseTagSelect);

        // Save the preferences
        if (GUI.changed)
		{
            EditorPrefs.SetBool("ActiveToggleKey", bUseActiveToggle);
            EditorPrefs.SetBool("LayerSelectKey", bUseLayerSelect);
            EditorPrefs.SetBool("TagSelectKey", bUseTagSelect);
		}
    }
}