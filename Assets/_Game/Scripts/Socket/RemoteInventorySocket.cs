using UnityEngine;

/// <summary>
/// Manages the positioning and inventory of sockets based on the HMD (Head-Mounted Display) position.
/// </summary>
public class RemoteInventorySocket : MonoBehaviour
{
    [SerializeField] private GameObject _hmd;
    [SerializeField] private BodySocket[] _bodySockets;

    private Transform _hmdTransform;
    private Vector3 _currentHMDLocalPosition;
    private Quaternion _currentHMDRotation;

    private void Awake()
    {
        // Cache the Transform component to avoid repeated GetComponent calls
        if (_hmd != null)
        {
            _hmdTransform = _hmd.transform;
        }
    }

    private void Update()
    {
        if (_hmdTransform != null)
        {
            _currentHMDLocalPosition = _hmdTransform.localPosition;
            _currentHMDRotation = _hmdTransform.rotation;
            UpdateBodySocketHeights();
            UpdateSocketInventory();
        }
    }

    /// <summary>
    /// Updates the heights of all body sockets based on the HMD's local position and each socket's height ratio.
    /// </summary>
    private void UpdateBodySocketHeights()
    {
        foreach (var socket in _bodySockets)
        {
            if (socket.BodySocketObject != null)
            {
                var socketTransform = socket.BodySocketObject.transform;
                socketTransform.localPosition = new Vector3(socketTransform.localPosition.x, _currentHMDLocalPosition.y * socket.HeightRatio, socketTransform.localPosition.z);
            }
        }
    }

    /// <summary>
    /// Updates the position and rotation of the inventory socket based on the HMD's position and rotation.
    /// </summary>
    private void UpdateSocketInventory()
    {
        transform.localPosition = new Vector3(_currentHMDLocalPosition.x, 0, _currentHMDLocalPosition.z);
        transform.rotation = new Quaternion(transform.rotation.x, _currentHMDRotation.y, transform.rotation.z, _currentHMDRotation.w);
    }
}
