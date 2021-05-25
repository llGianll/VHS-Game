using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IRewindable
{
    List<TimePoint> _timePoints = new List<TimePoint>();

    Rigidbody rb;
    private MeshRenderer _meshRenderer;
    private SphereCollider _sphereCollider;

    float _timeSpawned;

    bool _isActive = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        _isActive = true;
        _timePoints.Clear();
        _timeSpawned = TimeController.Instance.Timer;
    }

    private void Start()
    {
        TimeController.Instance.OnRewind += Rewind;
        TimeController.Instance.OnResume += StopRewind;
    }

    private void FixedUpdate()
    {
        if (TimeController.Instance.IsRewinding)
            RewindTimePoints();
        else
            RecordTimePoints();
    }

    private void Update()
    {
        if (TimeController.Instance.Timer <= _timeSpawned)
            gameObject.SetActive(false);

        HideObject();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //gameObject.SetActive(false);
        _isActive = false;

        if (collision.gameObject.GetComponent<PlayerMovement>())
            TimeController.Instance.Rewind();
    }

    private void OnTriggerEnter(Collider other)
    {
        _isActive = false;

        if (other.GetComponent<PlayerMovement>())
            TimeController.Instance.Rewind();
    }

    private void HideObject()
    {
        _meshRenderer.enabled = _isActive;
        _sphereCollider.enabled = _isActive;

        if(_isActive == false)
            rb.velocity = Vector3.zero;
    }

    public void Rewind()
    {

    }

    public void StopRewind()
    {

    }

    public void RewindTimePoints()
    {
        if (_timePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if(_timePoints.Count < timeIndex)
            {
                SetTimePoint(_timePoints[0]);
                _timePoints.Clear();
            }
            else
            {
                SetTimePoint(_timePoints[_timePoints.Count - timeIndex]);
                for (int i = 0; i < timeIndex; i++)
                {
                    _timePoints.RemoveAt(_timePoints.Count - 1);
                }
            }

        }
        else
            StopRewind();
    }

    public void RecordTimePoints()
    {
        //if (_isActive == false)
        //    return;

        TimePoint timePoint = new TimePoint(transform.position, transform.rotation, transform.localScale, rb.velocity, _isActive);
        _timePoints.Add(timePoint);
    }

    private void SetTimePoint(TimePoint timePoint)
    {
        transform.position = timePoint.Position;
        transform.rotation = timePoint.Rotation;
        transform.localScale = timePoint.Scale;
        rb.velocity = timePoint.Velocity;
        _isActive = timePoint.IsActive;
    }
}
