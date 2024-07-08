using UnityEngine;

/// <summary>
/// Triggers the respawn of the player when they enter the respawn zone.
/// </summary>
public class RespawnZoneTrigger : MonoBehaviour
{
    private DriveModePlayerHandler _driveModePlayerHandler;

    private void Awake()
    {
        _driveModePlayerHandler = FindObjectOfType<DriveModePlayerHandler>();
    }

    /// <summary>
    /// Called when another collider enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _driveModePlayerHandler.RespawnDriveMode(true);
        }
    }
}
