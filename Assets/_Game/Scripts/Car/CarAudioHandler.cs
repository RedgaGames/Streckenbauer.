using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles the car engine audio based on the car's speed and state.
/// </summary>
public class CarAudioHandler : MonoBehaviour
{
    [SerializeField] private EngineAudioOptions _engineSoundStyle = EngineAudioOptions.FourChannel;
    [SerializeField] private AudioClip _lowAccelClip; 
    [SerializeField] private AudioClip _lowDecelClip; 
    [SerializeField] private AudioClip _highAccelClip; 
    [SerializeField] private AudioClip _highDecelClip; 
    [SerializeField] private float _pitchMultiplier = 1f; 
    [SerializeField] private float _lowPitchMin = 1f; 
    [SerializeField] private float _lowPitchMax = 6f;
    [SerializeField] private float _highPitchMultiplier = 0.25f;
    [SerializeField] private float _maxRolloffDistance = 500; 
    [SerializeField] private float _dopplerLevel = 1; 
    [SerializeField] private bool _useDoppler = true; 

    private AudioSource _lowAccel;
    private AudioSource _lowDecel; 
    private AudioSource _highAccel; 
    private AudioSource _highDecel; 
    private bool _startedSound;

    private CarControlHandler _carControlHandler;


    /// <summary>
    /// Initializes the audio sources and starts the engine sound if the car is near enough.
    /// </summary>
    private void StartSound()
    {
        _carControlHandler = GetComponent<CarControlHandler>();
        _highAccel = SetUpEngineAudioSource(_highAccelClip);

        if (_engineSoundStyle == EngineAudioOptions.FourChannel)
        {
            _lowAccel = SetUpEngineAudioSource(_lowAccelClip);
            _lowDecel = SetUpEngineAudioSource(_lowDecelClip);
            _highDecel = SetUpEngineAudioSource(_highDecelClip);
        }

        _startedSound = true;
    }

    /// <summary>
    /// Stops and destroys all audio sources.
    /// </summary>
    private void StopSound()
    {
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }

        _startedSound = false;
    }

    /// <summary>
    /// Updates the audio sources based on the car's state and distance to the camera.
    /// </summary>
    private void Update()
    {
        float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

        if (_startedSound && camDist > _maxRolloffDistance * _maxRolloffDistance)
        {
            StopSound();
        }

        if (!_startedSound && camDist < _maxRolloffDistance * _maxRolloffDistance)
        {
            StartSound();
        }

        if (_startedSound)
        {
            UpdateEngineSounds();
        }
    }

    /// <summary>
    /// Updates the pitch and volume of the engine sounds based on the car's acceleration and speed.
    /// </summary>
    private void UpdateEngineSounds()
    {
        float pitch = ULerp(_lowPitchMin, _lowPitchMax, _carControlHandler.Revs);
        pitch = Mathf.Min(_lowPitchMax, pitch);

        if (_engineSoundStyle == EngineAudioOptions.Simple)
        {
            _highAccel.pitch = pitch * _pitchMultiplier * _highPitchMultiplier;
            _highAccel.dopplerLevel = _useDoppler ? _dopplerLevel : 0;
            _highAccel.volume = 1;
        }
        else
        {
            _lowAccel.pitch = pitch * _pitchMultiplier;
            _lowDecel.pitch = pitch * _pitchMultiplier;
            _highAccel.pitch = pitch * _highPitchMultiplier * _pitchMultiplier;
            _highDecel.pitch = pitch * _highPitchMultiplier * _pitchMultiplier;

            float accFade = Mathf.Abs(_carControlHandler.AccelInput);
            float decFade = 1 - accFade;

            float highFade = Mathf.InverseLerp(0.2f, 0.8f, _carControlHandler.Revs);
            float lowFade = 1 - highFade;

            highFade = 1 - ((1 - highFade) * (1 - highFade));
            lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
            accFade = 1 - ((1 - accFade) * (1 - accFade));
            decFade = 1 - ((1 - decFade) * (1 - decFade));

            _lowAccel.volume = lowFade * accFade;
            _lowDecel.volume = lowFade * decFade;
            _highAccel.volume = highFade * accFade;
            _highDecel.volume = highFade * decFade;

            _highAccel.dopplerLevel = _useDoppler ? _dopplerLevel : 0;
            _lowAccel.dopplerLevel = _useDoppler ? _dopplerLevel : 0;
            _highDecel.dopplerLevel = _useDoppler ? _dopplerLevel : 0;
            _lowDecel.dopplerLevel = _useDoppler ? _dopplerLevel : 0;
        }
    }

    /// <summary>
    /// Sets up and adds a new audio source to the game object.
    /// </summary>
    /// <param name="clip">The audio clip to be played by the audio source.</param>
    /// <returns>The created AudioSource component.</returns>
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.loop = true;
        source.time = Random.Range(0f, clip.length);
        source.Play();
        source.minDistance = 5;
        source.maxDistance = _maxRolloffDistance;
        source.dopplerLevel = 0;
        return source;
    }

    /// <summary>
    /// Unclamped version of Lerp.
    /// </summary>
    /// <param name="from">The start value.</param>
    /// <param name="to">The end value.</param>
    /// <param name="value">The interpolation value between 0 and 1.</param>
    /// <returns>The interpolated value.</returns>
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}