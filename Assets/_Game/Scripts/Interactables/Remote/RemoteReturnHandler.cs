using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles returning the remote to the body socket after a specified delay.
/// </summary>
public class RemoteReturnHandler : MonoBehaviour
{
    [SerializeField]private XRSocketInteractor _bodySocketInteractor;
    [SerializeField]private XRGrabInteractable _remoteGrabInteractor;
    [SerializeField]private float _returnDelay = 6f;

    /// <summary>
    /// Initiates the return of the remote to the body socket with or without a delay.
    /// </summary>
    /// <param name="withReturnDelay">Indicates whether the return should be delayed.</param>
    public void ReturnRemoteToSocket(bool withReturnDelay)
    {
        StartCoroutine(ReturnRemoteToSocketRoutine(withReturnDelay, GameStateEnum.BuildMode));
    }

    /// <summary>
    /// Coroutine to handle the delayed return of the remote to the body socket.
    /// </summary>
    /// <param name="withReturnDelay">Indicates whether the return should be delayed.</param>
    /// <param name="gameMode">The current game mode.</param>
    /// <returns>IEnumerator for coroutine handling.</returns>
    private IEnumerator ReturnRemoteToSocketRoutine(bool withReturnDelay, GameStateEnum gameMode)
    {
        if (withReturnDelay)
        {
            yield return new WaitForSeconds(_returnDelay);
        }

        if (gameMode == GameStateEnum.BuildMode && !_remoteGrabInteractor.isSelected)
        {
            _bodySocketInteractor.StartManualInteraction(_remoteGrabInteractor as IXRSelectInteractable);
        }
    }
}
