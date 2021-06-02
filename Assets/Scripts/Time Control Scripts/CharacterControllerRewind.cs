using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerRewind : MonoBehaviour, IRewindable
{
    List<TimePoint> _timePoints = new List<TimePoint>();
    CharacterControllerPlayerMovement _playerMovement;
    CharacterController _characterController;

    //[NOTE] this script is currently tightly coupled with PlayerMovement, decouple if necessary

    private void Awake()
    {
        _playerMovement = GetComponent<CharacterControllerPlayerMovement>();
        _characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeController.Instance.OnRewind += Rewind;
        TimeController.Instance.OnResume += StopRewind;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeController.Instance.IsRewinding)
            RewindTimePoints();
        else
            RecordTimePoints();
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
        TimePoint timePoint = new TimePoint(transform.position, transform.rotation, transform.localScale, _playerMovement.NetVelocity);
        _timePoints.Add(timePoint);
    }


    public void Rewind()
    {
        _characterController.enabled = false;
    }

    public void StopRewind()
    {
        _characterController.enabled = true;
    }

    private void SetTimePoint(TimePoint timePoint)
    {
        transform.position = timePoint.Position;
        transform.rotation = timePoint.Rotation;
        transform.localScale = timePoint.Scale;
        _playerMovement.NetVelocity = timePoint.Velocity;
    }
}
