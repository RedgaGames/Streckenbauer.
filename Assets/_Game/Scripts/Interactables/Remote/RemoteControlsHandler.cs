using UnityEngine;

/// <summary>
/// Handles the remote control interactions.
/// </summary>
public class RemoteControlsHandler : MonoBehaviour
{
    /// <summary>
    /// Changes the game mode by swapping the current game state.
    /// </summary>
    public void ChangeGameMode()
    {
        StateHandler.Instance.SwapGameState();
    }
}
