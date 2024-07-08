using UnityEngine;

/// <summary>
/// Handles resizing of all tile maps based on the current game state.
/// </summary>
public class TrackResizeHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _mapTileSets;

    private void Awake()
    {
        // Subscribe to the game state change event
        StateHandler.OnGameStateChanged += ResizeAllTileMaps;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the game state change event
        StateHandler.OnGameStateChanged -= ResizeAllTileMaps;
    }

    /// <summary>
    /// Resizes all tile maps based on the given game state.
    /// </summary>
    /// <param name="currentState">The current game state.</param>
    private void ResizeAllTileMaps(GameStateEnum currentState)
    {
        foreach (GameObject mapTileSet in _mapTileSets)
        {
            if (mapTileSet.TryGetComponent<TrackBuilderSocketHandler>(out var socketHandler))
            {
                socketHandler.StartResizeSockelItemsRoutine(currentState);
            }
        }
    }
}
