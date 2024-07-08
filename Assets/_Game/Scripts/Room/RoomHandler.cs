using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles changes to the room based on the game state.
/// </summary>
public class RoomHandler : MonoBehaviour
{
    private RoomScaleHandler _roomScaleHandler;
    private RoomDecorationElementsHandler _roomDecorationElementsHandler;

    private void Awake()
    {
        _roomScaleHandler = GetComponent<RoomScaleHandler>();
        _roomDecorationElementsHandler = GetComponent<RoomDecorationElementsHandler>();
        
        StateHandler.OnGameStateChanged += ChangeRoomForGameMode;
    }

    private void OnDestroy()
    {
        StateHandler.OnGameStateChanged -= ChangeRoomForGameMode;
    }

    /// <summary>
    /// Changes the room settings based on the current game state.
    /// </summary>
    /// <param name="currentState">The current game state.</param>
    private void ChangeRoomForGameMode(GameStateEnum currentState)
    {
        _roomScaleHandler.ScaleRoom();

        if (currentState == GameStateEnum.BuildMode)
        {
            _roomDecorationElementsHandler.ChangeActiveStateForDecorations(true);
        }
        else if (currentState == GameStateEnum.DriveMode)
        {
            _roomDecorationElementsHandler.ChangeActiveStateForDecorations(false);
        }
    }
}
