using UnityEngine;

/// <summary>
/// Handles scaling of the room based on the current game state.
/// </summary>
public class RoomScaleHandler : MonoBehaviour
{
    [SerializeField] private GameObject _roomObject;

    private readonly Vector3 _roomScaleBuildMode = new Vector3(1f, 1f, 1f);
    private readonly Vector3 _roomScaleDriveMode = new Vector3(40f, 40f, 40f);

    /// <summary>
    /// Sets the room scale based on the current game state.
    /// </summary>
    public void ScaleRoom()
    {
        if (_roomObject != null)
        {
            if (StateHandler.Instance.CurrentGameState == GameStateEnum.DriveMode)
            {
                _roomObject.transform.localScale = _roomScaleDriveMode;
            }
            else if (StateHandler.Instance.CurrentGameState == GameStateEnum.BuildMode)
            {
                _roomObject.transform.localScale = _roomScaleBuildMode;
            }
        }
    }
}
