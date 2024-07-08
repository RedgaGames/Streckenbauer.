using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates the mirror effect based on the player's position.
/// </summary>
public class MirrorMovementHandler : MonoBehaviour
{
    [SerializeField] private Transform _playerTargetTransform;
    [SerializeField] private Transform _mirrorTransform;

    private void Update()
    {
        CalculateMirrorEffect();
    }

    /// <summary>
    /// Calculates and applies the mirrored position and rotation to the current transform.
    /// </summary>
    private void CalculateMirrorEffect()
    {
        // Player position as local position relative to the mirror
        Vector3 localPlayerPosition = _mirrorTransform.InverseTransformPoint(_playerTargetTransform.position);
        
        // Calculate mirrored position
        Vector3 mirroredPosition = _mirrorTransform.TransformPoint(new Vector3(localPlayerPosition.x, localPlayerPosition.y, localPlayerPosition.z));
        transform.position = mirroredPosition;

        // Calculate mirrored look direction
        Vector3 lookAtMirrorPosition = _mirrorTransform.TransformPoint(new Vector3(-localPlayerPosition.x, localPlayerPosition.y, localPlayerPosition.z));
        transform.LookAt(lookAtMirrorPosition);
    }
}
