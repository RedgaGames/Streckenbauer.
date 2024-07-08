using UnityEngine;

/// <summary>
/// Handles the player actions in Drive Mode.
/// </summary>
public class DriveModePlayerHandler : MonoBehaviour
{
    [SerializeField] private GameObject _playerCarMode;

    private PlayerRespawnHandler _playerRespawnHandler;

    private void Awake()
    {
        _playerRespawnHandler = FindObjectOfType<PlayerRespawnHandler>();
    }

    /// <summary>
    /// Sets the player state to Drive Mode.
    /// </summary>
    /// <param name="state">If true, activates the player car mode. Otherwise, deactivates it.</param>
    public void SetStateDriveMode(bool state)
    {
        _playerCarMode.SetActive(state);
        RespawnDriveMode(false);
    }

    /// <summary>
    /// Respawns the player in Drive Mode at the designated respawn point.
    /// </summary>
    /// <param name="activateTransition">If true, activates a transition effect during respawn.</param>
    public void RespawnDriveMode(bool activateTransition)
    {
        _playerRespawnHandler.RespawnPlayer(GameStateEnum.DriveMode, _playerCarMode, activateTransition);
    }
}
