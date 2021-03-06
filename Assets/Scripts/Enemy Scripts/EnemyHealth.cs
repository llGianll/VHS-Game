using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IRewindable
{
    #region EnemyTimePoints class 

    #endregion

    [SerializeField] bool isInvincible = false;
    [SerializeField] float _maxHealth = 30f;
    float _currentHealth;

    [SerializeField] float _scoreOnKill = 250f;
    [SerializeField] float _diminishedScore = 50f;

    [SerializeField] GameObject _enemy;
    List<float> _healthTimePoints = new List<float>();

    private void Awake()
    {
        //_enemy = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        TimeControllerEventsInit();
        _currentHealth = _maxHealth;
    }

    public void DecreaseHealth(float damage, bool isDisabled)
    {
        if (isInvincible || isDisabled)
            return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        if (_currentHealth <= 0)
        {
            SFXPlayer.Instance.PlayClipAtPoint(SFXPresets.Explosion_1, transform.position);

            float score = GetEquivalentScore();

            if(_enemy.GetComponent<TurretBase>() != null)
                ScoreManager.Instance.AddKillScore(score, _enemy.GetComponent<TurretBase>()._noOfTimesDisabled < 1);
            else
                ScoreManager.Instance.AddKillScore(score, true);


            IsEnemyActive(false);
        }
    }

    private float GetEquivalentScore()
    {
        float score = 0;
        if(_enemy != null)
        {
            if(_enemy.GetComponent<TurretBase>() != null && _enemy.GetComponent<TurretBase>().DisableOnly)
            {
                score = (_enemy.GetComponent<TurretBase>()._noOfTimesDisabled >= 1) ? _diminishedScore : _scoreOnKill;
            }
        }

        return score;
    }

    private void IsEnemyActive(bool isActive)
    {
        if (_enemy != null)
        {
            if (_enemy.GetComponent<TurretBase>() != null && _enemy.GetComponent<TurretBase>().DisableOnly)
            {
                _currentHealth = _maxHealth; //set to max health for reactivation
                _enemy.GetComponent<TurretBase>().DisableTurret();
            }
            else
                _enemy.SetActive(isActive);
        }
    }

    public void Rewind()
    {

    }

    public void StopRewind()
    {

    }

    public void RewindTimePoints()
    {
        if (_healthTimePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_healthTimePoints.Count < timeIndex)
            {
                SetTimePoint(_healthTimePoints[0]);
                _healthTimePoints.Clear();
            }
            else
            {
                SetTimePoint(_healthTimePoints[_healthTimePoints.Count - timeIndex]);
                for (int i = 0; i < timeIndex; i++)
                {
                    _healthTimePoints.RemoveAt(_healthTimePoints.Count - 1);
                }

            }

            //reactive enemy when health > 0
            if (_currentHealth > 0)
                IsEnemyActive(true);

        }
        else
            StopRewind();
    }

    public void RecordTimePoints()
    {
        _healthTimePoints.Add(_currentHealth);
    }

    private void SetTimePoint(float currentHealth)
    {
        _currentHealth = currentHealth;
    }

    public void RemoveFrame()
    {
        _healthTimePoints.RemoveAt(0);
    }

    private void TimeControllerEventsInit()
    {
        TimeController.Instance.OnRewindBegin += Rewind;
        TimeController.Instance.OnRewindEnd += StopRewind;
        TimeController.Instance.OnRewindUpdate += RewindTimePoints;
        TimeController.Instance.OnResumeUpdate += RecordTimePoints;
        TimeController.Instance.OnReachedFrameThreshold += RemoveFrame;
    }
}
