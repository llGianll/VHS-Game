using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretConeType : TurretBase
{
    [SerializeField] float _startAngle = -45f;
    [SerializeField] float _angleRange = 90f;
    [SerializeField] float _rotateSpeed = 5f;
    //Rotation direction in reference from startAngle to startAngle + angleRange
    [SerializeField] bool _rotateClockW;

    float _initialAngle, _tempInitialAngle;
    float _targetAngle, _tempTargetAngle;
    float _currentAngle;

    private void Awake()
    {
        _initialAngle = (transform.localEulerAngles.z + _startAngle) < 180 ? transform.localEulerAngles.z + _startAngle : transform.localEulerAngles.z + _startAngle - 360; ;
        _targetAngle = (_initialAngle + _angleRange) < 180 ? _initialAngle + _angleRange : _initialAngle + _angleRange - 360; ;
        _tempInitialAngle = _initialAngle;
        _tempTargetAngle = _targetAngle;
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, _initialAngle);
        Initialize();
    }

    private void Start()
    {
    }

    private void Update()
    {
        OnUpdate();

        if (!TimeController.Instance.IsRewinding)
        {
            RotateTurret();
        }
    }

    private void RotateTurret()
    {
        _currentAngle = transform.localEulerAngles.z < 180 ? transform.localEulerAngles.z: transform.localEulerAngles.z - 360;
        //Debug.Log("current turret angle: " + _currentAngle + _rotateClockW);
        //Debug.Log("initial angle: " + _initialAngle);
        //Debug.Log("cw: " + Quaternion.Euler(0, 0, EasingFunction.EaseOutQuad(_initialAngle, _targetAngle, (Mathf.Abs(_currentAngle - _initialAngle) + 1) / _angleRange)).eulerAngles * _rotateSpeed * Time.deltaTime);
        //Debug.Log("ccw: " + Quaternion.Euler(0, 0, EasingFunction.EaseInOutCubic(_initialAngle, _targetAngle, (Mathf.Abs(_currentAngle + _initialAngle)) / _angleRange)).eulerAngles * -_rotateSpeed * Time.deltaTime);
        if (_rotateClockW)
        {
            if ((_targetAngle - _currentAngle) > 0.01)
            {
                //transform.Rotate(Quaternion.Euler(0, 0, EasingFunction.EaseInOutCubic(_initialAngle,  _targetAngle , (Mathf.Abs(_currentAngle - _initialAngle)) / _angleRange)).eulerAngles * _rotateSpeed * Time.deltaTime, Space.Self);
                transform.Rotate(Quaternion.Euler(0, 0, _rotateSpeed).eulerAngles * _rotateSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                _tempTargetAngle = _currentAngle;
                _rotateClockW = false;
            }
        }
        else
        {
            //_rotateClockW = !_rotateClockW;
            if ((_initialAngle - _currentAngle) < -0.01)
            {
                //transform.Rotate(Quaternion.Euler(0, 0, EasingFunction.EaseInOutCubic(_initialAngle,  _targetAngle , (Mathf.Abs(_currentAngle + _initialAngle)) / _angleRange)).eulerAngles * -_rotateSpeed * Time.deltaTime, Space.Self);
                transform.Rotate(Quaternion.Euler(0, 0, _rotateSpeed).eulerAngles * -_rotateSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                _tempInitialAngle = _currentAngle;
                _rotateClockW = true;
            }
        }
    }
}
