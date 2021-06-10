using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyControllerRewind : MonoBehaviour, IRewindable
{
    Transform _orientationT;
    List<TimePoint> _timePoints = new List<TimePoint>();

    Rigidbody rb;

    private void Awake()
    {
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
        TimePoint timePoint = new TimePoint(transform.position, _orientationT.rotation, transform.localScale, rb.velocity);
        _timePoints.Add(timePoint);
    }

    private void SetTimePoint(TimePoint timePoint)
    {
        transform.position = timePoint.Position;
        _orientationT.rotation = timePoint.Rotation;
        transform.localScale = timePoint.Scale;
        rb.velocity = timePoint.Velocity;
    }

    public void RemoveFrame()
    {
        _timePoints.RemoveAt(0);
    }
}
