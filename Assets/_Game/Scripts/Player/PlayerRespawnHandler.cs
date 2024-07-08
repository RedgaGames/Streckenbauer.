using UnityEngine;

/// <summary>
/// Handles the respawn of the player based on the game state and mode.
/// </summary>
public class PlayerRespawnHandler : MonoBehaviour
{
    private FadeHandler _fadeHandler;

    [Header("Respawnpoints")]
    [SerializeField] private Transform _respawnPointCarMode;
    [SerializeField] private Transform _respawnPointBuildMode;

    private void Awake() {
        _fadeHandler = FindObjectOfType<FadeHandler>();
    }

    /// <summary>
    /// Respawns the player at the appropriate respawn point based on the game state.
    /// </summary>
    /// <param name="currentGameState">The current game state.</param>
    /// <param name="player">The player GameObject to respawn.</param>
    /// <param name="activateTransition">Whether to activate a transition effect.</param>
    public void RespawnPlayer(GameStateEnum currentGameState, GameObject player, bool activateTransition)
    {
        if (activateTransition)
        {
            _fadeHandler.StartQuickFade();
        }

        Transform respawnPoint = currentGameState == GameStateEnum.BuildMode ? _respawnPointBuildMode : _respawnPointCarMode;
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;
    }
}
