using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerShooting : MonoBehaviour, IRewindable
{
    #region ShootingTimePoint inner class
    public class ShootingTimePoint
    {
        //public currentFireTime
        public int CurrentAmmoInMag;
        public bool IsReloading;
        public float ReloadTimer;

        public ShootingTimePoint(int currentAmmoInMag, bool isReloading, float reloadTimer)
        {
            this.CurrentAmmoInMag = currentAmmoInMag;
            this.IsReloading = isReloading;
            this.ReloadTimer = reloadTimer;
        }
    }
    #endregion

    //semi-auto pistol for now 
    [SerializeField] Camera _shootingCamera;
    [Header("Shoot variables")]
    [SerializeField] float _damagePerBullet = 10f;
    [SerializeField] float _range = 100f;
    [SerializeField] int _magCapacity = 10;
    [SerializeField] float _reloadSpeed = 1f;
    [SerializeField] float _maxFireRate = 0.25f;

    [Header("Shoot Audio Clips")]
    [SerializeField] AudioClip _shootSFX;
    [SerializeField] AudioClip _reloadSFX;

    int _currentAmmoInMag;
    float _currentFireTime;
    float _reloadTimer;
    bool _isReloading;
    Coroutine _reloadCoroutine;

    public bool IsReloading { get { return _isReloading; } }
    public int CurrentAmmoInMag { get { return _currentAmmoInMag; } }
    public int MagCapacity { get { return _magCapacity; } }

    //public Action<int, int> OnFire = delegate{};
    public Action<float, float> OnReload = delegate { };
    public Action<int, int> OnFinishedReload = delegate{};

    List<ShootingTimePoint> _timePoints = new List<ShootingTimePoint>();
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        TimeControllerEventsInit();

        _currentAmmoInMag = _magCapacity;
        _currentFireTime = _maxFireRate;
        _reloadTimer = 0;
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
        DrawRay();

        _currentFireTime += Time.deltaTime;

        //if (!TimeController.Instance.IsRewinding)
        //{
        //    if (Input.GetButtonDown("Fire1") && _currentFireTime >= _maxFireRate)
        //        Fire();
        //    else if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInMag != _magCapacity && !_isReloading)
        //        _reloadCoroutine = StartCoroutine(Reload(0));

        //    RecordTimePoints();
        //}
        //else
        //{
        //    RewindTimePoints();
        //}
    }

    private IEnumerator Reload(float startTimer)
    {
        _reloadTimer = startTimer; 
        _isReloading = true;

        PlaySFX(_reloadSFX);

        while (_reloadTimer < _reloadSpeed)
        {
            _reloadTimer += Time.deltaTime;
            OnReload(_reloadTimer, _reloadSpeed);
            yield return null;
        }
        _isReloading = false;
        _currentAmmoInMag = _magCapacity;
        OnFinishedReload(_currentAmmoInMag, _magCapacity);
    }

    private void DrawRay()
    {
        Debug.DrawRay(_shootingCamera.transform.position, _shootingCamera.transform.forward * _range, Color.green);
    }

    private void Fire()
    {
        if (_shootingCamera == null || _currentAmmoInMag < 1)
            return;

        if (_reloadCoroutine != null)
        {
            // stop reload and fire a shot
            StopCoroutine(_reloadCoroutine);
            _isReloading = false;
        }

        RaycastHit hit;
        Physics.Raycast(_shootingCamera.transform.position, _shootingCamera.transform.forward, out hit, _range);

        _currentFireTime = 0;
        _currentAmmoInMag--;
        //OnFire(_currentAmmoInMag, _magCapacity);

        PlaySFX(_shootSFX);

        if (hit.collider != null && hit.collider.GetComponent<IShootable>() != null)
            hit.collider.GetComponent<IShootable>().Hit(_damagePerBullet);

        //auto reload 
        if (_currentAmmoInMag == 0)
            StartCoroutine(Reload(0));

    }

    private void PlaySFX(AudioClip clip)
    {
        if (_audioSource == null)
            return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void Rewind()
    {

    }

    public void StopRewind()
    {
        if (_isReloading)
        {
            _reloadCoroutine = StartCoroutine(Reload(_reloadTimer));
        }
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
        if (Input.GetButtonDown("Fire1") && _currentFireTime >= _maxFireRate)
            Fire();
        else if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInMag != _magCapacity && !_isReloading)
            _reloadCoroutine = StartCoroutine(Reload(0));

        _timePoints.Add(new ShootingTimePoint(_currentAmmoInMag, _isReloading, _reloadTimer));
    }

    private void SetTimePoint(ShootingTimePoint timePoint)
    {
        _currentAmmoInMag = timePoint.CurrentAmmoInMag;
        _isReloading = timePoint.IsReloading;
        _reloadTimer = timePoint.ReloadTimer;
    }

    public void RemoveFrame()
    {
        _timePoints.RemoveAt(0);
    }
}
