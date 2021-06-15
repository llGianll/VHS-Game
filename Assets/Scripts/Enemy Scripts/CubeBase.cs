using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeTimePoint
{
    public float CurrentTime;
    public bool HasFired;
    public Vector3 Rotation;

    public CubeTimePoint(float CurrentTime, bool HasFired, Transform transform)
    {
        this.CurrentTime = CurrentTime;
        this.HasFired = HasFired;
        this.Rotation = transform.localEulerAngles;
    }
}

public class CubeBase : MonoBehaviour, IRewindable
{
    [SerializeField] bool _isRotating = false;
    [SerializeField] Vector3 _rotationVelocity = Vector3.zero;

    List<Transform> _bulletSpawnPoints = new List<Transform>();
    public List<Transform> bulletSpawnPoints
    {
        get { return _bulletSpawnPoints; }
        set { _bulletSpawnPoints = value; }
    }

    float _currentTime = 0;
    public float currentTime
    {
        get { return _currentTime; }
        set { _currentTime = value; }
    }
    bool _hasFired = false;
    public bool hasfired
    {
        get { return _hasFired; }
        set { _hasFired = value; }
    }

    List<CubeTimePoint> _cubeTimePoints = new List<CubeTimePoint>();
    
    public void Initialize()
    {
        _bulletSpawnPoints = new List<Transform>(transform.GetComponentsInChildren<Transform>(false));
        foreach (Transform transform in _bulletSpawnPoints.ToArray())
        {
            if(transform.tag != "Barrel")
            {
                _bulletSpawnPoints.Remove(transform);
            }
        }
        _currentTime = 0;
    }

    public void InitializeStart()
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

    public void RotateCube()
    {
        if (_isRotating)
        {
            transform.Rotate(Quaternion.Euler(_rotationVelocity.x, _rotationVelocity.y, _rotationVelocity.z).eulerAngles * Time.deltaTime, Space.Self);
        }
    }

    public void RewindUpdate()
    {
        //if (TimeController.Instance.IsRewinding)
        //    RewindTimePoints();
        //else
        //    RecordTimePoints();
    }

    public void Rewind()
    {

    }

    public void StopRewind()
    {

    }

    public void RewindTimePoints()
    {
        if (_cubeTimePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_cubeTimePoints.Count < timeIndex)
            {
                _currentTime = _cubeTimePoints[0].CurrentTime;
                gameObject.transform.localEulerAngles = _cubeTimePoints[0].Rotation;
                _cubeTimePoints.Clear();
            }
            else
            {
                _currentTime = _cubeTimePoints[_cubeTimePoints.Count - timeIndex].CurrentTime;
                gameObject.transform.localEulerAngles = _cubeTimePoints[_cubeTimePoints.Count - timeIndex].Rotation;
                for (int i = 0; i < timeIndex; i++)
                {
                    _cubeTimePoints.RemoveAt(_cubeTimePoints.Count - 1);
                }

            }

        }
        else
            StopRewind();
    }

    public void RecordTimePoints()
    {
        _currentTime += Time.deltaTime;
        _cubeTimePoints.Add(new CubeTimePoint(_currentTime, _hasFired, gameObject.transform));
    }

    public void RemoveFrame()
    {
        _cubeTimePoints.RemoveAt(0);
    }
}
