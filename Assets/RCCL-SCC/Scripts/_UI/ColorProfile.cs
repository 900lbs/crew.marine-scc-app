// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 05-08-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="ColorProfile.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;

using TMPro;
using Zenject;
using System.Threading.Tasks;

#if SCC_2_5
/// <summary>
/// Class that can be used on any monobehaviour component and setup for dynamic color selection
/// based on the UI Settings object, can add TextMeshProUGUI's and/or Images to the component array.
/// Implements the <see cref="UnityEngine.ISerializationCallbackReceiver" />
/// </summary>
/// <seealso cref="UnityEngine.ISerializationCallbackReceiver" />
[System.Serializable]
public class ColorProfile : ISerializationCallbackReceiver
{
	#region Injection Construction
	/// <summary>
	/// The UI manager
	/// </summary>
	readonly UIManager uiManager;

	/// <summary>
	/// Initializes a new instance of the <see cref="ColorProfile"/> class.
	/// </summary>
	/// <param name="uiMan">The UI man.</param>
	[Inject]
    public ColorProfile(UIManager uiMan)
    {
        uiManager = uiMan;
    }

	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	#region properties

	/// <summary>
	/// The current UI settings
	/// </summary>
	[Tooltip("Assign a settings object here for setting up index values, this is set automatically before serialization from the UI manager however, so no need to change this anytime we use different settings.")]
    public ShipUIVariable CurrentUISettings;
	/// <summary>
	/// The inactive color index
	/// </summary>
	[Range(0, 9)]
    public int DisabledColorIndex;
	/// <summary>
	/// The inactive color
	/// </summary>
	[SerializeField]
    [ReadOnly]
    protected Color32 disabledColor;

	/// <summary>
	/// Gets or sets the color of the inactive.
	/// </summary>
	/// <value>The color of the inactive.</value>
	public Color32 InactiveColor
    {
        get
        {
            return disabledColor;
        }

        set
        {
            if (!disabledColor.CompareRGB(value))
            {
                disabledColor = value;
            }
        }
    }
	/// <summary>
	/// The active color index
	/// </summary>
	[Range(0, 9)]
    public int EnabledColorIndex;
	/// <summary>
	/// The active color
	/// </summary>
	[SerializeField]
    [ReadOnly]
    protected Color32 enabledColor;

	/// <summary>
	/// Gets or sets the color of the active.
	/// </summary>
	/// <value>The color of the active.</value>
	public Color32 EnabledColor
    {
        get
        {
            return enabledColor;
        }

        set
        {
            if (!enabledColor.CompareRGB(value))
            {
                enabledColor = value;
            }
            else
            {
            }
        }
    }
	/// <summary>
	/// The selected color index
	/// </summary>
	[Range(0, 9)]
    public int SelectedColorIndex;

	/// <summary>
	/// The selected color
	/// </summary>
	[SerializeField]
    [ReadOnly]
    protected Color32 selectedColor;
	/// <summary>
	/// Gets or sets the color of the selected.
	/// </summary>
	/// <value>The color of the selected.</value>
	public Color32 SelectedColor
    {
        get
        {
            return selectedColor;
        }

        set
        {
            if (!selectedColor.CompareRGB(value))
            {
                selectedColor = value;
            }
        }
    }

    public bool IsDynamicSelectedColorBasedOnUser;
	/// <summary>
	/// The components
	/// </summary>
	public GameObject[] Components;
	#endregion

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback before Unity serializes your object.
	/// </summary>
	public void OnBeforeSerialize()
    {
        if (uiManager != null)
        {
            CurrentUISettings = uiManager.UI;
            SetColorsFromSettings();
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Implement this method to receive a callback after Unity deserializes your object.
	/// </summary>
	public void OnAfterDeserialize()
    {
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/


	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Initializes a new instance of the <see cref="ColorProfile"/> class.
	/// </summary>
	/// <param name="newInactiveColorIndex">New index of the inactive color.</param>
	/// <param name="newInactiveColor">New color of the inactive.</param>
	/// <param name="newActiveColorIndex">New index of the active color.</param>
	/// <param name="newActiveColor">New color of the active.</param>
	/// <param name="newComponents">The new components.</param>
	public ColorProfile(int newInactiveColorIndex, Color32 newInactiveColor, int newActiveColorIndex, Color32 newActiveColor, GameObject[] newComponents)
    {
        DisabledColorIndex = newInactiveColorIndex;
        disabledColor = newInactiveColor;
        EnabledColorIndex = newActiveColorIndex;
        enabledColor = newActiveColor;
        Components = newComponents;
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Since this is a custom c# class and not monobehaviour, this has to be called from the parent
	/// monobehaviour's OnValidate().
	/// </summary>
	public void OnValidate()
    {
        if (CurrentUISettings != null)
        {
            SetColorsFromSettings();
        }
    }

    public void ManuallyAssignColorIndex(ActiveState targetState, int targetIndex)
    {
        switch (targetState)
        {
            case ActiveState.Disabled:
            DisabledColorIndex = targetIndex;
            break;
            case ActiveState.Enabled:
            EnabledColorIndex = targetIndex;
            break;
            case ActiveState.Selected:
            EnabledColorIndex = targetIndex;
            break;
        }

        if(IsDynamicSelectedColorBasedOnUser)
        {
            SelectedColor = AnnotationManager.GetLineColorBasedOnUser(NetworkClient.GetUserName());
        }
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// Sets the colors from settings.
	/// </summary>
	public void SetColorsFromSettings()
    {
        if (CurrentUISettings == null)
            return;

        Color32[] cols = CurrentUISettings.Settings.Colors;

        InactiveColor = cols[DisabledColorIndex];
        EnabledColor = cols[EnabledColorIndex];
        SelectedColor = cols[SelectedColorIndex];
    }

	/*----------------------------------------------------------------------------------------------------------------------------*/

	/// <summary>
	/// States the change.
	/// </summary>
	/// <param name="state">The state.</param>
	public async Task StateChange(ActiveState state)
    {

        Color32 newColor = Color.black;

        switch (state)
        {
            case ActiveState.Disabled:
                newColor = InactiveColor;
                break;

            case ActiveState.Enabled:
                newColor = EnabledColor;

                break;

            case ActiveState.Selected:
                newColor = SelectedColor;

                break;
        }

        for (int u = 0; u < Components.Length; u++)
        {
            await UIHelpers.ToggleColor(Components[u], newColor);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public async void ColorFlash(ActiveState beginState, ActiveState endState)
    {
        await StateChange(endState);
        await StateChange(beginState);
    }

}

#endif