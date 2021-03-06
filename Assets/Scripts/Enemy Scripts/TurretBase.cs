using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TurretTimePoint
{
    public float CurrentTime;
    public bool HasFired;
    public Vector3 Rotation;
    public bool IsDisabled;
    public float DisableTimer;
    public int NoOfTimesDisabled;

    public TurretTimePoint(float CurrentTime, bool HasFired, Transform transform, bool isDisabled, float disableTimer, int noOfTimesDisabled)
    {
        this.CurrentTime = CurrentTime;
        this.HasFired = HasFired;
        this.Rotation = transform.localEulerAngles;
        this.IsDisabled = isDisabled;
        this.DisableTimer = disableTimer;
        this.NoOfTimesDisabled = noOfTimesDisabled;
    }
}

public class TurretBase : MonoBehaviour, IRewindable, IShootable
{
    [Header("Disabling Turret")]
    [SerializeField] bool _disableOnly = true;
    [SerializeField] float _disableTime = 5f;
    [SerializeField] Image _activationUI;
    float _disableTimer = 0;

    [Header("Base turret parameters")]
    [SerializeField] float _fireRate = 0.5f;
    [SerializeField] float _projectileSpeed = 5f;
    [SerializeField] GameObject _bullet;

    public bool DisableOnly => _disableOnly;
    protected bool _isDisabled;
    public int _noOfTimesDisabled = 0;

    EnemyHealth _enemyHealth;

    Transform _bulletSpawnPoint;
    float _currentTime;
    bool _hasFired = false;
    public bool AtActiveSector { get; set; }

    List<TurretTimePoint> _turretTimePoints = new List<TurretTimePoint>();

    public void Initialize()
    {
        _enemyHealth = transform.parent.GetComponent<EnemyHealth>();
        _bulletSpawnPoint = transform.GetChild(0);
        _currentTime = 0;
    }

    public void InitializeStart()
    {
        //Time controller event registrations can't be on Awake()
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

    public void OnUpdate()
    {
        if (GameManager.isPaused)
            return;

        UpdateActivationUI();
        _hasFired = _currentTime >= _fireRate ? true : false;

        if (_isDisabled)
            return;

        if (SectorOptimization.Instance == null)
        {
            //for enemies to still function while the optimization is not applied to the level
            FireBullet();
        }
        else if (SectorOptimization.Instance != null)
        {
            //if optimization is already applied to the level and player is at the same sector as the enemy 
            if (AtActiveSector)
                FireBullet();
        }
    }

    private void UpdateDisableTimer()
    {
        if (_isDisabled)
        {
            _disableTimer -= Time.deltaTime;


            if (_disableTimer <= 0)
                _isDisabled = false;
        }
        else
        {
            _activationUI.fillAmount = 1;
        }
    }

    private void UpdateActivationUI()
    {
        if (_isDisabled)
        {
            if (_activationUI != null)
               _activationUI.fillAmount = 1 - _disableTimer / _disableTime;
        }
        else
        {
            _activationUI.fillAmount = 1;
        }
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
                gameObject.transform.localEulerAngles = _turretTimePoints[0].Rotation;
                _isDisabled = _turretTimePoints[0].IsDisabled;
                _disableTimer = _turretTimePoints[0].DisableTimer;
                _noOfTimesDisabled = _turretTimePoints[0].NoOfTimesDisabled;
                _turretTimePoints.Clear();
            }
            else
            {
                _currentTime = _turretTimePoints[_turretTimePoints.Count - timeIndex].CurrentTime;
                gameObject.transform.localEulerAngles = _turretTimePoints[_turretTimePoints.Count - timeIndex].Rotation;
                _isDisabled = _turretTimePoints[_turretTimePoints.Count - timeIndex].IsDisabled;
                _disableTimer = _turretTimePoints[_turretTimePoints.Count - timeIndex].DisableTimer;
                _noOfTimesDisabled = _turretTimePoints[_turretTimePoints.Count - timeIndex].NoOfTimesDisabled;

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
        UpdateDisableTimer();
        _currentTime += Time.deltaTime;
        _turretTimePoints.Add(new TurretTimePoint(_currentTime, _hasFired, gameObject.transform, _isDisabled , _disableTimer, _noOfTimesDisabled));
    }

    private void FireBullet()
    {
        if (_hasFired)
        {
            SpawnBullet();
            _currentTime = 0;
        }
    }

    public void RemoveFrame()
    {
        _turretTimePoints.RemoveAt(0);
    }

    public void Hit(float damage)
    {
        SFXPlayer.Instance.PlayClipAtPoint(SFXPresets.TurretHit, transform.position);
        if (_enemyHealth != null)
            _enemyHealth.DecreaseHealth(damage, _isDisabled);
    }

    public void DisableTurret()
    {
        //can't use coroutine for timers as you can't restart its execution upon rewinding
        _isDisabled = true;
        _disableTimer = _disableTime;
        _noOfTimesDisabled++;
    }
}
