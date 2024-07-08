using System.Collections;
using UnityEngine;

/// <summary>
/// Fades a canvas over time using a coroutine and a canvas group. Both modes use their own camera so this must be assigned as worldcamera.
/// </summary>
public class FadeHandler : MonoBehaviour
{
    [SerializeField] private GameObject _fadeCanvas;
    [SerializeField] private Camera _cameraBuildMode;
    [SerializeField] private Camera _cameraCarMode;

    private Canvas _fadeCanvasCanvas;
    private CanvasGroup _fadeCanvasGroup;

    private const float TransitionFadeDuration = 3f;
    private const float QuickFadeDuration = 0.5f;

    private void Awake() 
    {
        _fadeCanvasCanvas = _fadeCanvas.GetComponent<Canvas>();
        _fadeCanvasGroup = _fadeCanvas.GetComponent<CanvasGroup>();

        StateHandler.OnGameStateChanged += StartTransitionFade;
    }

    private void StartTransitionFade(GameStateEnum currentState)
    {
        StopAllCoroutines();
        _fadeCanvasCanvas.worldCamera = StateHandler.Instance.CurrentGameState == GameStateEnum.BuildMode ? _cameraBuildMode : _cameraCarMode;
        StartCoroutine(FadeOut(TransitionFadeDuration));
    }

    public void StartQuickFade()
    {
        StopAllCoroutines();
        _fadeCanvasCanvas.worldCamera = _cameraCarMode;
        StartCoroutine(FadeOut(QuickFadeDuration));
    }


    private IEnumerator FadeOut(float duration)
    {
        _fadeCanvasGroup.alpha = 1f;
        float elapsedTime = 0.0f;

        while (_fadeCanvasGroup.alpha > 0.0f)
        {
            _fadeCanvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
