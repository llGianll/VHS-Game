using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyControllerRewind : MonoBehaviour, IRewindable
{
    #region
    public class CrouchValues
    {
        //add more fields if necessary
        public bool IsCrouching; 

        public CrouchValues(bool isCrouching)
        {
            IsCrouching = isCrouching;
        }
    }


    #endregion

    Transform _orientationT;
    List<TimePoint> _timePoints = new List<TimePoint>();
    List<CrouchValues> _crouchValues = new List<CrouchValues>();

    RigidbodyPlayerMovement _rbPlayerMovement;
    RigidbodyWallrun _rbWallrun;
    Rigidbody rb;

    private void Awake()
    {
        _rbPlayerMovement = GetComponent<RigidbodyPlayerMovement>();
        _rbWallrun = GetComponent<RigidbodyWallrun>();
        _orientationT = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeControllerEventsInit();
    }

    private void TimeControllerEventsInit()
    {
        TimeController.Instance.OnRewindBegin += Rewind;
        TimeController.Instance.OnRewindEnd += StopRewind;
        TimeController.Instance.OnRewindUpdate += RewindTimePoints;
        TimeController.Instance.OnResumeUpdate += RecordTimePoints;
        TimeController.Instance.OnReachedFrameThreshold += RemoveFrame;
    }

    private void Update()
    {

    }

    public void Rewind()
    {
        _rbPlayerMovement.enabled = false;
        _rbWallrun.enabled = false;
        rb.velocity = Vector3.zero;
    }

    public void StopRewind()
    {
        _rbPlayerMovement.enabled = true;
        _rbWallrun.enabled = true;
    }

    public void RewindTimePoints()
    {
        if (_timePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_timePoints.Count < timeIndex)
            {
                SetTimePoint(_timePoints[0]);
                SetCrouchPoint(_crouchValues[0]);
                _timePoints.Clear();
            }
            else
            {
                SetTimePoint(_timePoints[_timePoints.Count - timeIndex]);
                SetCrouchPoint(_crouchValues[_crouchValues.Count - timeIndex]);
                for (int i = 0; i < timeIndex; i++)
                {
                    _timePoints.RemoveAt(_timePoints.Count - 1);
                    _crouchValues.RemoveAt(_crouchValues.Count - 1);
                }
                
            }

        }
        else
            StopRewind();

    }


    public void RecordTimePoints()
    {
        TimePoint timePoint = new TimePoint(transform.position, _orientationT.rotation, transform.localScale, rb.velocity);
        _timePoints.Add(timePoint);

        CrouchValues crouchPoint = new CrouchValues(_rbPlayerMovement.isCrouching);
        _crouchValues.Add(crouchPoint);
    }

    private void SetTimePoint(TimePoint timePoint)
    {
        transform.position = timePoint.Position;
        _orientationT.rotation = timePoint.Rotation;
        transform.localScale = timePoint.Scale;
        rb.velocity = timePoint.Velocity;
    }

    private void SetCrouchPoint(CrouchValues crouchPoint)
    {
        _rbPlayerMovement.isCrouching = crouchPoint.IsCrouching;
    }

    public void RemoveFrame()
    {
        _timePoints.RemoveAt(0);
    }
}
