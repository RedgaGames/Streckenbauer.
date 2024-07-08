using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the visual and audio effects for car wheels, including skid marks and tire smoke.
/// </summary>
public class CarWheelEffectsHandler : MonoBehaviour
{
    [SerializeField] private Transform _skidTrailPrefab;
    [SerializeField] private ParticleSystem _skidParticles;

    private static Transform _skidTrailsDetachedParent;

    private AudioSource _audioSource;
    private Transform _skidTrail;
    private WheelCollider _wheelCollider;
    private bool _skidding;

    /// <summary>
    /// Gets a value indicating whether the car is skidding.
    /// </summary>
    public bool Skidding
    {
        get => _skidding;
        private set => _skidding = value;
    }

    /// <summary>
    /// Gets a value indicating whether the audio is playing.
    /// </summary>
    public bool PlayingAudio { get; private set; }

    private void Awake()
    {
        _skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();
        if (_skidParticles != null)
        {
            _skidParticles.Stop();
        }

        _wheelCollider = GetComponent<WheelCollider>();
        _audioSource = GetComponent<AudioSource>();

        if (_skidTrailsDetachedParent == null)
        {
            _skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
        }
    }

    /// <summary>
    /// Emits tire smoke at the wheel's position.
    /// </summary>
    public void EmitTyreSmoke()
    {
        if (_skidParticles == null) return;

        _skidParticles.transform.position = transform.position - transform.up * _wheelCollider.radius;
        _skidParticles.Emit(1);
        if (!Skidding)
        {
            StartCoroutine(StartSkidTrail());
        }
    }

    /// <summary>
    /// Plays the audio for skidding.
    /// </summary>
    public void PlayAudio()
    {
        if (_audioSource == null) return;

        _audioSource.Play();
        PlayingAudio = true;
    }

    /// <summary>
    /// Stops the audio for skidding.
    /// </summary>
    public void StopAudio()
    {
        if (_audioSource == null) return;

        _audioSource.Stop();
        PlayingAudio = false;
    }

    /// <summary>
    /// Starts the skid trail effect.
    /// </summary>
    private IEnumerator StartSkidTrail()
    {
        Skidding = true;
        _skidTrail = Instantiate(_skidTrailPrefab);
        if (_skidTrail == null)
        {
            yield break;
        }

        _skidTrail.parent = transform;
        _skidTrail.localPosition = -Vector3.up * _wheelCollider.radius;
    }

    /// <summary>
    /// Ends the skid trail effect and detaches the skid trail from the car.
    /// </summary>
    public void EndSkidTrail()
    {
        if (!Skidding) return;

        Skidding = false;
        if (_skidTrail != null)
        {
            _skidTrail.parent = _skidTrailsDetachedParent;
            Destroy(_skidTrail.gameObject, 10);
        }
    }
}
