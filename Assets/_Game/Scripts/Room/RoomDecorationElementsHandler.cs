using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the active state of room decoration objects.
/// </summary>
public class RoomDecorationElementsHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _roomDecorations;

    /// <summary>
    /// Changes the active state for all decorations.
    /// </summary>
    /// <param name="isActive">The desired active state.</param>
    public void ChangeActiveStateForDecorations(bool isActive)
    {
        if (_roomDecorations == null) return;

        foreach (var decoration in _roomDecorations)
        {
            if (decoration != null)
            {
                decoration.SetActive(isActive);
            }
        }
    }
}
