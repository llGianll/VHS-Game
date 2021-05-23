﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PosRBRewind : MonoBehaviour, IRewindable
{
    List<TimePoint> _timePoints = new List<TimePoint>();

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

    public void Rewind()
    {
        //rb.useGravity = false;
    }

    public void StopRewind()
    {
        //rb.useGravity = true;
    }

    public void RewindTimePoints()
    {
        if (_timePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_timePoints.Count < timeIndex)
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
        TimePoint timePoint = new TimePoint(transform.position, transform.rotation, transform.localScale, rb.velocity);
        _timePoints.Add(timePoint);
    }

    private void SetTimePoint(TimePoint timePoint)
    {
        transform.position = timePoint.Position;
        transform.rotation = timePoint.Rotation;
        transform.localScale = timePoint.Scale;
        rb.velocity = timePoint.Velocity;
    }
}
