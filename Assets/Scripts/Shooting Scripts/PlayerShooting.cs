using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    //semi-auto pistol for now 

    [SerializeField] Camera _shootingCamera;
    [Header("Shoot variables")]
    [SerializeField] float _damagePerBullet = 10f;
    [SerializeField] float _range = 100f;
    [SerializeField] int _magCapacity = 10;
    [SerializeField] float _reloadSpeed = 1f;
    [SerializeField] float _maxFireRate = 0.25f;

    int _currentAmmoInMag;
    float _currentTime;
    bool _isReloading;
    Coroutine _reloadCoroutine;

    public bool IsReloading { get { return _isReloading; } }

    public Action<int, int> OnFire = delegate{};
    public Action<float, float> OnReload = delegate { };
    public Action<int, int> OnFinishedReload = delegate{};

    private void Start()
    {
        _currentAmmoInMag = _magCapacity;
        _currentTime = _maxFireRate;
    }

    private void Update()
    {
        DrawRay();

        _currentTime += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && _currentTime >= _maxFireRate)
            Fire();

        if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInMag != _magCapacity && !_isReloading)
            _reloadCoroutine = StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        float timer = 0; 
        _isReloading = true;

        while(timer < _reloadSpeed)
        {
            timer += Time.deltaTime;
            OnReload(timer, _reloadSpeed);
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

        _currentTime = 0;
        _currentAmmoInMag--;
        OnFire(_currentAmmoInMag, _magCapacity);

        if (hit.collider != null && hit.collider.GetComponent<IShootable>() != null)
            hit.collider.GetComponent<IShootable>().Hit();

        //auto reload 
        if (_currentAmmoInMag == 0)
            StartCoroutine(Reload());

    }
}
