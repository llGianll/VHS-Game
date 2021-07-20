using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRadialType : TurretBase
{
    [SerializeField] float _startAngle = -45f;
    [SerializeField] float _rotateSpeed = 5f;
    //Rotation direction in reference from startAngle to startAngle + angleRange
    [SerializeField] bool _rotateClockW;

    float _initialAngle;
    float _currentAngle;

    private void Awake()
    {
        _initialAngle = (transform.localEulerAngles.z + _startAngle) < 180 ? transform.localEulerAngles.z + _startAngle : transform.localEulerAngles.z + _startAngle - 360;
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, _initialAngle);
        Initialize();
    }

    private void Start()
    {
        InitializeStart();
    }

    private void Update()
    {
        OnUpdate();

        if (!TimeController.Instance.IsRewinding && !_isDisabled)
        {
            RotateTurret();
        }
    }

    private void RotateTurret()
    {
        _currentAngle = transform.localEulerAngles.z < 180 ? transform.localEulerAngles.z : transform.localEulerAngles.z - 360;
        transform.Rotate(Quaternion.Euler(0, 0, _rotateSpeed).eulerAngles * _rotateSpeed * Time.deltaTime, Space.Self);
    }
}
