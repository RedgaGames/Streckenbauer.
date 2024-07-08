//using Unity.VRTemplate;
using UnityEngine;

/// <summary>
/// Handles user control for the car, using VR knob input for steering and button input for acceleration and braking.
/// </summary>
public class CarUserControlHandler : MonoBehaviour
{
    [SerializeField] private XRKnob _xrKnob;
    [SerializeField] private float _accelerationValue = 0.5f;

    private CarControlHandler _carControlHandler;
    private bool _isAccelerating = false;
    private bool _isBraking = false;
    private float _steeringValue = 0f;
    private float _currentSpeedAcceleration = 0f;

    private void Awake()
    {
        _carControlHandler = GetComponent<CarControlHandler>();
    }

    private void FixedUpdate()
    {
        MoveCar();
    }        

    /// <summary>
    /// Moves the car based on the current input states.
    /// </summary>
    private void MoveCar()
    {
        _steeringValue = ConvertSliderValue();

        if (_isAccelerating)
        {
            _currentSpeedAcceleration = _accelerationValue;
        }
        else if (_isBraking)
        {
            _currentSpeedAcceleration = -_accelerationValue;
        }
        else
        {
            _currentSpeedAcceleration = 0;
        }

        _carControlHandler.Move(_steeringValue, _currentSpeedAcceleration, _currentSpeedAcceleration, 0);
    }

    /// <summary>
    /// Sets the acceleration state of the car.
    /// </summary>
    /// <param name="state">If set to true, the car will accelerate.</param>
    public void AccelerateCar(bool state)
    {
        _isAccelerating = state;
    }

    /// <summary>
    /// Sets the braking state of the car.
    /// </summary>
    /// <param name="state">If set to true, the car will brake.</param>
    public void BrakeCar(bool state)
    {
        _isBraking = state;
    }

    /// <summary>
    /// Converts the knob value from [0f, 1f] to [-1f, 1f] for steering.
    /// </summary>
    /// <returns>The converted steering value.</returns>
    private float ConvertSliderValue()
    {
        float knobValue = _xrKnob.value;
        float convertedValue = knobValue * 2f - 1f;

        return convertedValue;
    }
}
