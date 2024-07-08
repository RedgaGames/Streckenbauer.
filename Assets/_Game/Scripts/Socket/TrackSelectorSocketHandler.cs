using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles deactivation of the socket interactor and resets the game object's size to its original dimensions.
/// </summary>
public class TrackSelectorSocketHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer _panelMeshRenderer;
    [SerializeField] private Material _panelMaterial;

    private XRSocketInteractor _socketInteractor;
    private readonly Vector3 _objectSizeDetached = new Vector3(0.4f, 0.3f, 0.4f);

    private void Awake()
    {
        _socketInteractor = GetComponent<XRSocketInteractor>();
        _socketInteractor.selectExited.AddListener(OnSelectExited);
    }

    private void OnDestroy()
    {
        _socketInteractor.selectExited.RemoveListener(OnSelectExited);
    }

    /// <summary>
    /// Called when an object is deselected from the socket interactor.
    /// Resizes the deselected object and deactivates the socket interactor.
    /// </summary>
    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Resize the socketed object
        GameObject socketedObject = args.interactableObject.transform.gameObject;
        socketedObject.transform.localScale = _objectSizeDetached;
        
        // Change the panel material to yellow and deactivate the socket interactor
        _panelMeshRenderer.material = _panelMaterial;
        _socketInteractor.socketActive = false;
    }
}
