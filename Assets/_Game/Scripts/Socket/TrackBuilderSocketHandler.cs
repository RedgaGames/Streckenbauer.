using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles resizing of objects attached to an XRSocketInteractor based on the game state.
/// </summary>
public class TrackBuilderSocketHandler : MonoBehaviour
{
    private XRSocketInteractor _socketInteractor;
    private IXRSelectInteractable _storedInteractable;

    private void Awake()
    {
        _socketInteractor = GetComponent<XRSocketInteractor>();
    }

    /// <summary>
    /// Starts the coroutine to resize socketed items based on the game mode.
    /// </summary>
    /// <param name="gameMode">The current game mode.</param>
    public void StartResizeSockelItemsRoutine(GameStateEnum gameMode)
    {
        if (_socketInteractor.GetOldestInteractableSelected() == null) return;
        StartCoroutine(ResizeSockelItemsRoutine(gameMode));
    }

    /// <summary>
    /// Coroutine that handles the resizing process of the socketed item.
    /// </summary>
    /// <param name="gameMode">The current game mode.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator ResizeSockelItemsRoutine(GameStateEnum gameMode)
    {
        RemoveCurrentObject();
        ResizeCurrentObject(gameMode);
        yield return new WaitForSeconds(0.1f);
        InsertCurrentObject();
    }

    /// <summary>
    /// Removes the currently attached object from the socket interactor.
    /// </summary>
    private void RemoveCurrentObject()
    {
        _storedInteractable = _socketInteractor.GetOldestInteractableSelected();
        if (_storedInteractable == null) return;
        // Remove the attached object
        _socketInteractor.interactionManager.SelectExit(_socketInteractor, _storedInteractable);
    }

    /// <summary>
    /// Resizes the currently attached object based on the game mode.
    /// </summary>
    /// <param name="gameMode">The current game mode.</param>
    private void ResizeCurrentObject(GameStateEnum gameMode)
    {
        if (_storedInteractable == null) return;

        var scaleFactor = gameMode == GameStateEnum.DriveMode ? 40f : 0.025f;
        _storedInteractable.transform.localScale *= scaleFactor;
    }

    /// <summary>
    /// Re-attaches the resized object to the socket interactor.
    /// </summary>
    private void InsertCurrentObject()
    {
        if (_storedInteractable == null) return;

        _socketInteractor.interactionManager.SelectEnter(_socketInteractor, _storedInteractable);
        _storedInteractable = null;
    }
}
