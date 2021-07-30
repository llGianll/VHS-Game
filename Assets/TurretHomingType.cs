using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHomingType : TurretBase
{
    [SerializeField] float _startAngle = 0f;
    [SerializeField] float _angleRange = 90f;
    [SerializeField] float _rotateSpeed = 5f;
    //Rotation direction in reference from startAngle to startAngle + angleRange
    //[SerializeField] bool _rotateClockW;
    [SerializeField] GameObject _targetObject;
    [SerializeField] float _targetRange = 20f;
    [SerializeField] Transform _barrel;

    Vector3 _initialForwardVector;
    Vector3 _currentForwardVector;
    float _initialAngle;
    float _targetAngle;
    float _currentAngle;
    float _rotationDirection;
    Vector3 _targetDirection;
    bool _isTargetinRange;

    //For some reason transform.forward is pointing straight up and I cant figure out why, so using dirty workaround
    private void Awake()
    {
        _initialAngle = (transform.localEulerAngles.z + _startAngle) < 180 ? transform.localEulerAngles.z + _startAngle : transform.localEulerAngles.z + _startAngle - 360; ;

        _initialForwardVector = _barrel.position - this.transform.position;
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, _initialAngle);
        if (_targetObject == null)
        {
            _targetObject = GameObject.FindGameObjectWithTag("Player");
        }
        Initialize();
    }

    private void Start()
    {
        InitializeStart();
    }

    private void FixedUpdate()
    {
        _isTargetinRange = TargetInRange();

        if (_isTargetinRange)
        {
            OnUpdate();
        }

        if (!TimeController.Instance.IsRewinding && !_isDisabled)
        {
            if (_isTargetinRange)
            {
                RotateTurret();
            }
        }
    }

    private bool TargetInRange()
    {
        if (Vector3.Distance(this.transform.position, _targetObject.transform.position) <= _targetRange)
        {
            _targetDirection = (_targetObject.transform.position - this.transform.position).normalized;
            _targetDirection.Set(_targetDirection.x, 0, _targetDirection.z);
            _targetAngle = Vector3.Angle(_initialForwardVector, _targetDirection);
            return ((_targetAngle <= _angleRange / 2) && (_targetAngle >= -_angleRange / 2));
        }
        //Debug.Log("Distance: " + Vector3.Distance(this.transform.position, _targetObject.transform.position));
        return false;
    }

    private void RotateTurret()
    {
        //Debug.Log("ROTATING TURRET");
        //Debug.DrawRay(transform.position, _currentForwardVector * 10);
        //transform.localRotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotateSpeed * Time.deltaTime);

        //_currentAngle = transform.localEulerAngles.z < 180 ? transform.localEulerAngles.z : transform.localEulerAngles.z - 360;
        _currentForwardVector = _barrel.position - this.transform.position;
        _targetAngle = Vector3.Angle(_currentForwardVector, _targetDirection);
        Debug.Log("Angle: " + _targetAngle);

        if (_targetAngle > 0.01f)
        {
            _rotationDirection = Vector3.Cross(_currentForwardVector, _targetDirection).y > 0 ? 1 : -1;
            Debug.Log("Direction: " + _rotationDirection);
            transform.Rotate(Quaternion.Euler(0, 0, _rotateSpeed).eulerAngles * _rotateSpeed * _rotationDirection * Time.deltaTime, Space.Self);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _targetRange);
    }
}
