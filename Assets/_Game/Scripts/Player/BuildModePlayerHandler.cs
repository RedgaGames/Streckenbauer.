using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the player actions in Build Mode.
/// </summary>
public class BuildModePlayerHandler : MonoBehaviour
{
    [SerializeField] private GameObject _playerBuildMode;

    private PlayerRespawnHandler _playerRespawnHandler;
    private RemoteReturnHandler _remoteReturnHandler;

    private void Awake()
    {
        _playerRespawnHandler = FindObjectOfType<PlayerRespawnHandler>();
        _remoteReturnHandler = FindObjectOfType<RemoteReturnHandler>();
    }

    /// <summary>
    /// Sets the player state to Build Mode.
    /// </summary>
    /// <param name="state">If true, activates the player build mode. Otherwise, deactivates it.</param>
    public void SetStateBuildMode(bool state)
    {
        _playerBuildMode.SetActive(state);
        RespawnBuildMode(false);

        _remoteReturnHandler.ReturnRemoteToSocket(false);
    }

    /// <summary>
    /// Respawns the player in Build Mode at the designated respawn point.
    /// </summary>
    /// <param name="activateTransition">If true, activates a transition effect during respawn.</param>
    public void RespawnBuildMode(bool activateTransition)
    {
        _playerRespawnHandler.RespawnPlayer(GameStateEnum.BuildMode, _playerBuildMode, activateTransition);
    }
}
