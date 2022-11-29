using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGameEventList : MonoBehaviour
{
    /// [FINAL UI + ANIMATION] can track:
    /// 1. What and If the praise text should be shown
    /// 2. Success rate control - go up or down

    public static Action<float> PlayerChangedDress = delegate (float _timeAfterCrossingTheLine) { };

    /// [GAMEPLAY: End/Finish] can track:
    /// 1. Hide all not needed buttons
    /// 2. Change camera angle and position
    /// 3. Start confetis
    public static Action<bool> PlayerCrossedTheFinishLine = delegate( bool _playerWinStatus ) { };

    public static Action<Control> CharacterCrossedTheFinishLine = delegate(Control _characterType) { };

    /// <summary>
    /// Usage:
    /// 1. Camera Control
    /// 2. Location Management
    /// 3. 
    /// </summary>
    public static Action<GameState> OnChangeGameState;

    public static Action CutSceneSkipped;
    public static Action SkipLevel;

    public static Action<Appearance> TutorialCheckpoinReached = delegate (Appearance _dressToCheck) { };
    public static Action<bool> TurnOnHint = delegate( bool _hintStatus) { };
}
