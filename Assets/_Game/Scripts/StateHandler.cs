using System;
using UnityEngine;

/// <summary>
/// Manages the game state and handles state transitions.
/// </summary>
public class StateHandler : MonoBehaviour
{
    public static StateHandler Instance;
    public GameStateEnum CurrentGameState;

    public static event Action<GameStateEnum> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Updates the current game state to the specified new state and invokes the OnGameStateChanged event.
    /// </summary>
    /// <param name="newState">The new game state to transition to.</param>
    public void UpdateGameState(GameStateEnum newState)
    {
        CurrentGameState = newState;

        switch (newState)
        {
            case GameStateEnum.Intro:
                break;
            case GameStateEnum.BuildMode:
                break;
            case GameStateEnum.DriveMode:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        // Invoke the OnGameStateChanged event if there are subscribers
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Swaps the game state between BuildMode and DriveMode.
    /// </summary>
    public void SwapGameState()
    {
        CurrentGameState = CurrentGameState == GameStateEnum.BuildMode ? GameStateEnum.DriveMode : GameStateEnum.BuildMode;
        UpdateGameState(CurrentGameState);
    }
}
