// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 03-26-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="EnumData.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

/// <summary>
/// Enum ShipID
/// </summary>
[Flags]
public enum ShipID
{
    /// <summary>
    /// None
    /// </summary>
    None = 0x0,

    /// <summary>
    /// Ddge
    /// </summary>
    Edge = 0x1,

    /// <summary>
    /// Symphony
    /// </summary>
    Symphony = 0x2,

    /// <summary>
    /// Spectrum
    /// </summary>
    Spectrum = 0x4,

    Oasis = 0x8,
    Apex = 0x16,
    Odyssey = 0x32,
    Wonder = 0x64,
    Beyond = 0x128
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum AuthorityID
/// </summary>
[Flags]
public enum AuthorityID
{
    /// <summary>
    /// The master
    /// </summary>
    Master = 0x1,

    /// <summary>
    /// The tablet
    /// </summary>
    Tablet = 0x2,

    /// <summary>
    /// The shore table
    /// </summary>
    ShoreTable = 0x4
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum UserNameID:
/// If you add/remove values, be sure to update the name functions in UserProfileFactory and anywhere else needed.
/// </summary>
[Flags]
public enum UserNameID
{
    SafetyCommandCenter = 1,

    EngineControlRoom = 2,

    StagingArea = 4,

    ForwardControlPoint = 8,

    EvacuationControlCenter = 16,

    IncidentCommandCenter = 32,
    ShoresideOperations = 64
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum UserAction
/// </summary>
public enum UserAction
{
    /// <summary>
    /// The login action
    /// </summary>
    Login,

    /// <summary>
    /// The share view action
    /// </summary>
    ShareView,

    ToggleAnnotations,
    Select,
    ReturnToUserSelection,
    ReturnToShipSelection
}

public enum ConnectionInfoType
{
    Disconnected,
    LoginFailed,
    JoinRoomFailed
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Our main reference for actions concerning the network/client relationship.
/// </summary>
public enum NetworkAction
{
    /// <summary>
    /// Attempt to join a room.
    /// </summary>
    Join,

    ///<summary>
    /// Attempt to create a room.
    /// </sumnmary>
    Create,

    /// <summary>
    /// Create a room "offline".
    /// </summary>
    OfflineMode,

    Rejoin,
    StartOver
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum NetworkStorageType
/// </summary>
[Flags]
public enum NetworkStorageType
{
    /// <summary>
    /// The line renderer
    /// </summary>
    LineRenderer = 0x1,

    /// <summary>
    /// The icon
    /// </summary>
    Icon = 0x2,

    /// <summary>
    /// The timer
    /// </summary>
    Timer = 0x4
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Custom event handles for creating and updating data shared across the network.
/// </summary>
public enum NetworkEvent
{
    /// <summary>
    /// The create
    /// </summary>
    Create,

    /// <summary>
    /// The edit
    /// </summary>
    Edit,

    /// <summary>
    /// The erase
    /// </summary>
    Erase,

    /// <summary>
    /// The erase all
    /// </summary>
    EraseAll,

    /// <summary>
    /// The play
    /// </summary>
    Play,

    /// <summary>
    /// The pause
    /// </summary>
    Pause,

    /// <summary>
    /// The reset
    /// </summary>
    Reset,

    Stop
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum ShipRoomProperties
/// </summary>
[Flags]
public enum ShipRoomProperties
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0x1,

    /// <summary>
    /// The ga overlay
    /// </summary>
    GAOverlay = 0x2,

    /// <summary>
    /// The annotations
    /// </summary>
    Annotations = 0x4,

    Timers = 0x8,
    KillCards = 0x16
}

/*------------------------------------------------------------------------------------------------*/

public enum FeatureTool
{
    None,
    Hatches,
    Framing
}

/*----------------------------------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum ColumnID
/// </summary>
public enum ColumnID
{
    /// <summary>
    /// The left
    /// </summary>
    Left,

    /// <summary>
    /// The middle
    /// </summary>
    Middle,

    /// <summary>
    /// The right
    /// </summary>
    Right
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum ActiveState
/// </summary>
public enum ActiveState
{
    /// <summary>
    /// The disabled
    /// </summary>
    Disabled,

    /// <summary>
    /// The enabled
    /// </summary>
    Enabled,

    /// <summary>
    /// The selected
    /// </summary>
    Selected
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum ActiveAnnotationTools
/// </summary>
[Flags]
public enum ActiveAnnotationTools
{
    //None = 0x0,
    /// <summary>
    /// The art
    /// </summary>
    Art = 0x1,

    /// <summary>
    /// The timer
    /// </summary>
    Timer = 0x2,

    /// <summary>
    /// The stopwatch
    /// </summary>
    Stopwatch = 0x4,

    /// <summary>
    /// The killcards
    /// </summary>
    KillCards = 0x8
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum AnnotationState
/// </summary>
public enum AnnotationState
{
    /// <summary>
    /// The highlight
    /// </summary>
    Highlight,

    /// <summary>
    /// The pen
    /// </summary>
    Pen,

    /// <summary>
    /// The move
    /// </summary>
    Move,

    /// <summary>
    /// The eraser
    /// </summary>
    Eraser,

    /// <summary>
    /// The erase all
    /// </summary>
    EraseAll,

    /// <summary>
    /// The icon
    /// </summary>
    Icon
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum IconType
/// </summary>
public enum IconType
{
    /// <summary>
    /// The standard
    /// </summary>
    Standard,

    /// <summary>
    /// The chemical
    /// </summary>
    Chemical,

    /// <summary>
    /// The numbered
    /// </summary>
    Numbered
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum WidgetInteraction
/// </summary>
public enum WidgetInteraction
{
    /// <summary>
    /// The toggle
    /// </summary>
    Toggle,

    /// <summary>
    /// The dock
    /// </summary>
    Dock,

    /// <summary>
    /// The slide
    /// </summary>
    Slide,

    ToggleIfDisabled
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum CornersID
/// </summary>
public enum CornersID
{
    /// <summary>
    /// The bottom left
    /// </summary>
    BottomLeft,

    /// <summary>
    /// The top left
    /// </summary>
    TopLeft,

    /// <summary>
    /// The top right
    /// </summary>
    TopRight,

    /// <summary>
    /// The bottom right
    /// </summary>
    BottomRight
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum DebugType
/// </summary>
public enum DebugType
{
    /// <summary>
    /// The networking
    /// </summary>
    Networking,

    /// <summary>
    /// The annotations
    /// </summary>
    Annotations
}

/*------------------------------------------------------------------------------------------------*/

/// <summary>
/// Enum XMLType
/// </summary>
public enum XMLType
{
    /// <summary>
    /// The main page
    /// </summary>
    MainPage,

    /// <summary>
    /// The timers
    /// </summary>
    Timers,

    /// <summary>
    /// The overlays
    /// </summary>
    Overlays,

    /// <summary>
    /// The widgets
    /// </summary>
    Widgets,

    /// <summary>
    /// The isolate
    /// </summary>
    Isolate,

    /// <summary>
    /// The minimize
    /// </summary>
    Minimize,

    /// <summary>
    /// The close
    /// </summary>
    Close,

    /// <summary>
    /// The close confirmation
    /// </summary>
    CloseConfirmation
}

public enum PromptMenuAction
{
    Prompt,
    Confirmation,
    Cancel
}

/*----------------------------------------------------------------------------------------------------------------------------*/