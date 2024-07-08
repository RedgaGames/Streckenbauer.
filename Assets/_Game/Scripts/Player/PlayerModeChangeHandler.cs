using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the transition between different player modes based on the game state.
/// </summary>
public class PlayerModeChangeHandler : MonoBehaviour
{
    private BuildModePlayerHandler _buildModePlayerHandler;
    private DriveModePlayerHandler _driveModePlayerHandler;
    private RoomHandler _roomHandler;

    private void Awake()
    {
        _buildModePlayerHandler = GetComponent<BuildModePlayerHandler>();
        _driveModePlayerHandler = GetComponent<DriveModePlayerHandler>();
        _roomHandler = GetComponent<RoomHandler>();

        StateHandler.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        StateHandler.OnGameStateChanged -= HandleGameStateChanged;
    }

    /// <summary>
    /// Handles the game state change event and starts the coroutine to swap game modes.
    /// </summary>
    /// <param name="newState">The new game state.</param>
    private void HandleGameStateChanged(GameStateEnum newState)
    {
        StartCoroutine(SwapGameModeRoutine(newState));
    }

    /// <summary>
    /// Coroutine to handle the smooth transition between build mode and drive mode.
    /// </summary>
    /// <param name="currentState">The current game state.</param>
    /// <returns>IEnumerator for coroutine handling.</returns>
    private IEnumerator SwapGameModeRoutine(GameStateEnum currentState)
    {
        if (currentState == GameStateEnum.BuildMode)
        {
            _buildModePlayerHandler.SetStateBuildMode(true);
            yield return new WaitForSeconds(0.1f);
            _driveModePlayerHandler.SetStateDriveMode(false);
        }
        else if (currentState == GameStateEnum.DriveMode)
        {
            _driveModePlayerHandler.SetStateDriveMode(true);
            yield return new WaitForSeconds(0.1f);
            _buildModePlayerHandler.SetStateBuildMode(false);
        }
    }
}
