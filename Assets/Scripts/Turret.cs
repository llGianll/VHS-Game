using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Turret : MonoBehaviour, IRewindable
{
    #region TurretTimePoint inner class

    [System.Serializable]
    public class TurretTimePoint
    {
        public float CurrentTime;
        public bool HasFired;

        public TurretTimePoint(float CurrentTime, bool HasFired)
        {
            this.CurrentTime = CurrentTime;
            this.HasFired = HasFired;
        }
    }

    #endregion

    [SerializeField] float _fireRate = 0.5f;
    [SerializeField] float _projectileSpeed = 5f;
    [SerializeField] GameObject _bullet;

    Transform _bulletSpawnPoint;
    float _currentTime;
    bool _hasFired = false;
    List<TurretTimePoint> _turretTimePoints = new List<TurretTimePoint>();

    private void Awake()
    {
        _bulletSpawnPoint = transform.GetChild(0);
    }

    void Start()
    {
        TimeControllerEventsInit();
        _currentTime = 0;
    }

    private void TimeControllerEventsInit()
    {
        TimeController.Instance.OnRewindBegin += Rewind;
        TimeController.Instance.OnRewindEnd += StopRewind;
        TimeController.Instance.OnRewindUpdate += RewindTimePoints;
        TimeController.Instance.OnResumeUpdate += RecordTimePoints;
    }

    void Update()
    {
        _hasFired = _currentTime >= _fireRate ? true : false;        
    }

    private void SpawnBullet()
    {
        GameObject bullet = PooledObjectManager.Instance.GetPooledObject("Bullet");
        bullet.transform.position = _bulletSpawnPoint.position;
        bullet.GetComponent<Rigidbody>().velocity = _bulletSpawnPoint.forward * _projectileSpeed;
        bullet.SetActive(true);
    }

    public void Rewind()
    {

    }

    public void StopRewind()
    {

    }

    public void RewindTimePoints()
    {
        if (_turretTimePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_turretTimePoints.Count < timeIndex)
            {
                _currentTime = _turretTimePoints[0].CurrentTime;
                _turretTimePoints.Clear();
            }
            else
            {
                _currentTime = _turretTimePoints[_turretTimePoints.Count - timeIndex].CurrentTime;
                for (int i = 0; i < timeIndex; i++)
                {
                    _turretTimePoints.RemoveAt(_turretTimePoints.Count - 1);
                }

            }

        }
        else
            StopRewind();
    }

    public void RecordTimePoints()
    {
        if (_hasFired)
        {
            SpawnBullet();
            _currentTime = 0;
        }

        _currentTime += Time.deltaTime;
        _turretTimePoints.Add(new TurretTimePoint(_currentTime, _hasFired));
    }
}
