using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShooting : MonoBehaviour
{
    [SerializeField] Text _bulletText;
    [SerializeField] Image _reloadBar;
    [SerializeField] Image _crosshair;

    [SerializeField] Color _enemyInSight;
    [SerializeField] Color _enemyOutOfSight;

    PlayerShooting _playerShooting;

    private void Awake()
    {
        _playerShooting = FindObjectOfType<PlayerShooting>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(_playerShooting != null)
        {
            //_playerShooting.OnFire += UpdateAmmoCount;
            _playerShooting.OnReload += UpdateReloadBar;
            //_playerShooting.OnFinishedReload += UpdateAmmoCount;
        }

        CrosshairColor();
    }

    private void UpdateReloadBar(float timer, float reloadSpeed)
    {
        if (_reloadBar != null)
        {
            _reloadBar.fillAmount = timer / reloadSpeed;
        }

    }

    private void UpdateAmmoCount(int ammo, int maxAmmoInMag)
    {
        if(_bulletText != null)
            _bulletText.text = ammo.ToString() + "/" + maxAmmoInMag.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _reloadBar.gameObject.SetActive(_playerShooting.IsReloading);
        UpdateAmmoCount(_playerShooting.CurrentAmmoInMag, _playerShooting.MagCapacity);
        CrosshairColor();

    }

    private void CrosshairColor()
    {
        _crosshair.color = _playerShooting.IsInSightAndShootable ? _enemyInSight : _enemyOutOfSight;
    }
}
