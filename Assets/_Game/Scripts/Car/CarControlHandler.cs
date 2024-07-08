using System;
using UnityEngine;

/// <summary>
/// Handles the car's driving mechanics including steering, acceleration, braking, and effects.
/// </summary>
public class CarControlHandler : MonoBehaviour
{
    [SerializeField] private CarDriveType _carDriveType = CarDriveType.FourWheelDrive;
    [SerializeField] private WheelCollider[] _wheelColliders = new WheelCollider[4];
    [SerializeField] private GameObject[] _wheelMeshes = new GameObject[4];
    [SerializeField] private CarWheelEffectsHandler[] _wheelEffects = new CarWheelEffectsHandler[4];
    [SerializeField] private Vector3 _centreOfMassOffset;
    [SerializeField] private float _maximumSteerAngle;
    [Range(0, 1)] [SerializeField] private float _steerHelper; // 0 is raw physics, 1 the car will grip in the direction it is facing
    [Range(0, 1)] [SerializeField] private float _tractionControl; // 0 is no traction control, 1 is full interference
    [SerializeField] private float _fullTorqueOverAllWheels;
    [SerializeField] private float _reverseTorque;
    [SerializeField] private float _maxHandbrakeTorque;
    [SerializeField] private float _downforce = 100f;
    [SerializeField] private SpeedType _speedType;
    [SerializeField] private float _topspeed = 200;
    [SerializeField] private static int _noOfGears = 5;
    [SerializeField] private float _revRangeBoundary = 1f;
    [SerializeField] private float _slipLimit;
    [SerializeField] private float _brakeTorque;

    private Quaternion[] _wheelMeshLocalRotations;
    private Vector3 _prevpos, _pos;
    private float _steerAngle;
    private int _gearNum;
    private float _gearFactor;
    private float _oldRotation;
    private float _currentTorque;
    private Rigidbody _rigidbody;
    private const float k_ReversingThreshold = 0.01f;

    public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; }
    public float CurrentSteerAngle { get { return _steerAngle; } }
    public float CurrentSpeed { get { return _rigidbody.velocity.magnitude * 2.23693629f; } }
    public float MaxSpeed { get { return _topspeed; } }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    private void Start()
    {
        _wheelMeshLocalRotations = new Quaternion[4];
        for (int i = 0; i < 4; i++)
        {
            _wheelMeshLocalRotations[i] = _wheelMeshes[i].transform.localRotation;
        }
        _wheelColliders[0].attachedRigidbody.centerOfMass = _centreOfMassOffset;

        _maxHandbrakeTorque = float.MaxValue;

        _rigidbody = GetComponent<Rigidbody>();
        _currentTorque = _fullTorqueOverAllWheels - (_tractionControl * _fullTorqueOverAllWheels);
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upgearlimit = (1 / (float)_noOfGears) * (_gearNum + 1);
        float downgearlimit = (1 / (float)_noOfGears) * _gearNum;

        if (_gearNum > 0 && f < downgearlimit)
        {
            _gearNum--;
        }

        if (f > upgearlimit && (_gearNum < (_noOfGears - 1)))
        {
            _gearNum++;
        }
    }

    /// <summary>
    /// Adds a curved bias towards 1 for a value in the 0-1 range.
    /// </summary>
    /// <param name="factor">The input factor.</param>
    /// <returns>The curved factor.</returns>
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }

    /// <summary>
    /// Unclamped version of Lerp, to allow value to exceed the from-to range.
    /// </summary>
    /// <param name="from">The start value.</param>
    /// <param name="to">The end value.</param>
    /// <param name="value">The interpolation value between 0 and 1.</param>
    /// <returns>The interpolated value.</returns>
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }

    private void CalculateGearFactor()
    {
        float f = (1 / (float)_noOfGears);
        var targetGearFactor = Mathf.InverseLerp(f * _gearNum, f * (_gearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        _gearFactor = Mathf.Lerp(_gearFactor, targetGearFactor, Time.deltaTime * 5f);
    }

    private void CalculateRevs()
    {
        CalculateGearFactor();
        var gearNumFactor = _gearNum / (float)_noOfGears;
        var revsRangeMin = ULerp(0f, _revRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(_revRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, _gearFactor);
    }

    /// <summary>
    /// Handles the car's movement including steering, acceleration, braking, and effects.
    /// </summary>
    /// <param name="steering">The steering input.</param>
    /// <param name="accel">The acceleration input.</param>
    /// <param name="footbrake">The footbrake input.</param>
    /// <param name="handbrake">The handbrake input.</param>
    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            _wheelColliders[i].GetWorldPose(out position, out quat);
            _wheelMeshes[i].transform.position = position;
            _wheelMeshes[i].transform.rotation = quat;
        }

        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        _steerAngle = steering * _maximumSteerAngle;
        _wheelColliders[0].steerAngle = _steerAngle;
        _wheelColliders[1].steerAngle = _steerAngle;

        SteerHelper();
        ApplyDrive(accel, footbrake);
        CapSpeed();

        if (handbrake > 0f)
        {
            var hbTorque = handbrake * _maxHandbrakeTorque;
            _wheelColliders[2].brakeTorque = hbTorque;
            _wheelColliders[3].brakeTorque = hbTorque;
        }

        CalculateRevs();
        GearChanging();

        AddDownForce();
        CheckForWheelSpin();
        TractionControl();
    }

    private void CapSpeed()
    {
        float speed = _rigidbody.velocity.magnitude;
        switch (_speedType)
        {
            case SpeedType.MPH:
                speed *= 2.23693629f;
                if (speed > _topspeed)
                    _rigidbody.velocity = (_topspeed / 2.23693629f) * _rigidbody.velocity.normalized;
                break;

            case SpeedType.KMH:
                speed *= 3.6f;
                if (speed > _topspeed)
                    _rigidbody.velocity = (_topspeed / 3.6f) * _rigidbody.velocity.normalized;
                break;
        }
    }

    private void ApplyDrive(float accel, float footbrake)
    {
        float thrustTorque;
        switch (_carDriveType)
        {
            case CarDriveType.FourWheelDrive:
                thrustTorque = accel * (_currentTorque / 4f);
                for (int i = 0; i < 4; i++)
                {
                    _wheelColliders[i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveType.FrontWheelDrive:
                thrustTorque = accel * (_currentTorque / 2f);
                _wheelColliders[0].motorTorque = _wheelColliders[1].motorTorque = thrustTorque;
                break;

            case CarDriveType.RearWheelDrive:
                thrustTorque = accel * (_currentTorque / 2f);
                _wheelColliders[2].motorTorque = _wheelColliders[3].motorTorque = thrustTorque;
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f)
            {
                _wheelColliders[i].brakeTorque = _brakeTorque * footbrake;
            }
            else if (footbrake > 0)
            {
                _wheelColliders[i].brakeTorque = 0f;
                _wheelColliders[i].motorTorque = -_reverseTorque * footbrake;
            }
        }
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            _wheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels aren't on the ground so don't realign the rigidbody velocity
        }

        if (Mathf.Abs(_oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - _oldRotation) * _steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            _rigidbody.velocity = velRotation * _rigidbody.velocity;
        }
        _oldRotation = transform.eulerAngles.y;
    }

    private void AddDownForce()
    {
        _wheelColliders[0].attachedRigidbody.AddForce(-transform.up * _downforce * _wheelColliders[0].attachedRigidbody.velocity.magnitude);
    }

    private void CheckForWheelSpin()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            _wheelColliders[i].GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= _slipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= _slipLimit)
            {
                _wheelEffects[i].EmitTyreSmoke();

                if (!AnySkidSoundPlaying())
                {
                    _wheelEffects[i].PlayAudio();
                }
                continue;
            }

            if (_wheelEffects[i].PlayingAudio)
            {
                _wheelEffects[i].StopAudio();
            }

            _wheelEffects[i].EndSkidTrail();
        }
    }

    private void TractionControl()
    {
        WheelHit wheelHit;
        switch (_carDriveType)
        {
            case CarDriveType.FourWheelDrive:
                for (int i = 0; i < 4; i++)
                {
                    _wheelColliders[i].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                }
                break;

            case CarDriveType.RearWheelDrive:
                _wheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                _wheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

            case CarDriveType.FrontWheelDrive:
                _wheelColliders[0].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                _wheelColliders[1].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;
        }
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= _slipLimit && _currentTorque >= 0)
        {
            _currentTorque -= 10 * _tractionControl;
        }
        else
        {
            _currentTorque += 10 * _tractionControl;
            if (_currentTorque > _fullTorqueOverAllWheels)
            {
                _currentTorque = _fullTorqueOverAllWheels;
            }
        }
    }

    private bool AnySkidSoundPlaying()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_wheelEffects[i].PlayingAudio)
            {
                return true;
            }
        }
        return false;
    }
}
