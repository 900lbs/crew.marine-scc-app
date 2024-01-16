// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 04-18-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 06-14-2019
// ***********************************************************************
// <copyright file="ShipUIData.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ShipUIData.
/// </summary>

[System.Serializable]
public class ShipUIData
{
    #region Colors
    /// <summary>
    /// The selected col
    /// </summary>
    [Header("Color Presets")]
    [Space(2.5f)]
    [SerializeField]
    Color32 selectedCol;
    /// <summary>
    /// Gets the selected col.
    /// </summary>
    /// <value>The selected col.</value>
    public Color32 SelectedCol { get { return selectedCol; } }

    /// <summary>
    /// The deselected col
    /// </summary>
    [SerializeField]
    Color32 deselectedCol;
    /// <summary>
    /// Gets the deselected col.
    /// </summary>
    /// <value>The deselected col.</value>
    public Color32 DeselectedCol { get { return deselectedCol; } }

    /// <summary>
    /// The selected button col
    /// </summary>
    [SerializeField]
    Color32 selectedButtonCol;
    /// <summary>
    /// Gets the selected button col.
    /// </summary>
    /// <value>The selected button col.</value>
    public Color32 SelectedButtonCol { get { return selectedButtonCol; } }

    /// <summary>
    /// The selected text col
    /// </summary>
    [SerializeField]
    Color32 selectedTextCol;
    /// <summary>
    /// Gets the selected text col.
    /// </summary>
    /// <value>The selected text col.</value>
    public Color32 SelectedTextCol { get { return selectedTextCol; } }

    /// <summary>
    /// The deselected button col
    /// </summary>
    [SerializeField]
    Color32 deselectedButtonCol;
    /// <summary>
    /// Gets the deselected button col.
    /// </summary>
    /// <value>The deselected button col.</value>
    public Color32 DeselectedButtonCol { get { return deselectedButtonCol; } }

    /// <summary>
    /// The de selected text col
    /// </summary>
    [SerializeField]
    Color32 deSelectedTextCol;
    /// <summary>
    /// Gets the de selected text col.
    /// </summary>
    /// <value>The de selected text col.</value>
    public Color32 DeSelectedTextCol { get { return deSelectedTextCol; } }

    /// <summary>
    /// The de selected dash col
    /// </summary>
    [SerializeField]
    Color32 deSelectedDashCol;
    /// <summary>
    /// Gets the de selected dash col.
    /// </summary>
    /// <value>The de selected dash col.</value>
    public Color32 DeSelectedDashCol { get { return deSelectedDashCol; } }

    /// <summary>
    /// The inactive color
    /// </summary>
    [SerializeField]
    Color32 inactiveColor;
    /// <summary>
    /// Gets the color of the inactive.
    /// </summary>
    /// <value>The color of the inactive.</value>
    public Color32 InactiveColor { get { return inactiveColor; } }

    /// <summary>
    /// The color9
    /// </summary>
    [SerializeField]
    Color32 color9;

    /// <summary>
    /// Gets the color9.
    /// </summary>
    /// <value>The color9.</value>
    public Color32 Color9 { get { return color9; } }

	[SerializeField]
    Color32 color10;

    public Color32 Color10 { get { return Color10; } }

    #endregion

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #region Prefabs
    /// <summary>
    /// The deck selection button
    /// </summary>
    [Header("UI Prefabs")]
    [Space(2.5f)]
    [Tooltip("The button used to select specific decks.")]
    [SerializeField]
    GameObject deckSelectionButton;
    /// <summary>
    /// Gets the deck selection button.
    /// </summary>
    /// <value>The deck selection button.</value>
    public GameObject DeckSelectionButton { get { return deckSelectionButton; } }

    /// <summary>
    /// Gets the colors.
    /// </summary>
    /// <value>The colors.</value>
    public Color32[] Colors
    {
        get
        {
            Color32[] newColors = new Color32[]
            {
                selectedCol,
                deselectedCol,
                selectedButtonCol,
                selectedTextCol,
                deselectedButtonCol,
                deSelectedTextCol,
                deSelectedDashCol,
                inactiveColor,
                color9,
				color10
            };


            return newColors;
        }
    }
    /// <summary>
    /// The deck selection spacer
    /// </summary>
    [SerializeField]
    GameObject deckSelectionSpacer;
    /// <summary>
    /// Gets the deck selection spacer.
    /// </summary>
    /// <value>The deck selection spacer.</value>
    public GameObject DeckSelectionSpacer { get { return deckSelectionSpacer; } }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    #endregion
    /// <summary>
    /// Initializes a new instance of the <see cref="ShipUIData"/> class.
    /// </summary>
    /// <param name="newSelectedCol">The new selected col.</param>
    /// <param name="newDeselectedCol">The new deselected col.</param>
    /// <param name="newSelectedButtonCol">The new selected button col.</param>
    /// <param name="newSelectedTextCol">The new selected text col.</param>
    /// <param name="newDeselelectedButtonCol">The new deselelected button col.</param>
    /// <param name="newDeselectedTextCol">The new deselected text col.</param>
    /// <param name="newDeselDashCol">The new desel dash col.</param>
    /// <param name="newInactiveColor">New color of the inactive.</param>
    /// <param name="newColor9">The new color9.</param>
    /// <param name="newDeckSelectionButton">The new deck selection button.</param>
    /// <param name="newDeckSelectionSpacer">The new deck selection spacer.</param>
    public ShipUIData(Color32 newSelectedCol, Color32 newDeselectedCol, Color32 newSelectedButtonCol, Color32 newSelectedTextCol,
    Color32 newDeselelectedButtonCol, Color32 newDeselectedTextCol, Color32 newDeselDashCol, Color32 newInactiveColor, Color32 newColor9,
	Color32 newColor10, GameObject newDeckSelectionButton,
    GameObject newDeckSelectionSpacer)
    {
        selectedCol = newSelectedCol;
        deselectedCol = newDeselectedCol;
        selectedButtonCol = newSelectedButtonCol;
        selectedTextCol = newSelectedTextCol;
        deselectedButtonCol = newDeselelectedButtonCol;
        deSelectedTextCol = newDeselectedTextCol;
        deSelectedDashCol = newDeselDashCol;
        inactiveColor = newInactiveColor;
        color9 = newColor9;
        color10 = newColor10;
        deckSelectionButton = newDeckSelectionButton;
        deckSelectionSpacer = newDeckSelectionSpacer;
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

}