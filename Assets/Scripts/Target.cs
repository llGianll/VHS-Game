using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour, IShootable, IRewindable
{
    //[Temporary Script] for testing
    [SerializeField] float _maxHP;
    [SerializeField] Color _hitColor;
    [SerializeField] Image _healthImg;

    float _currentHP;
    Color _originalColor;
    MeshRenderer _meshRenderer;

    List<float> _timePoints = new List<float>();

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        TimeControllerEventsInit();

        _currentHP = _maxHP;
        _originalColor = _meshRenderer.material.color;
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
        _meshRenderer.material.color = _originalColor;

        //if (TimeController.Instance.IsRewinding)
        //    RewindTimePoints();
        //else
        //    RecordTimePoints();    

        if (_healthImg != null)
            _healthImg.fillAmount = _currentHP / _maxHP;
    }

    public void Hit(float damage)
    {
        _meshRenderer.material.color = _hitColor;
        _currentHP = Mathf.Clamp(_currentHP - damage, 0, _maxHP);
    }

    public void Rewind()
    {

    }

    public void StopRewind()
    {

    }

    public void RewindTimePoints()
    {
        if (_timePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_timePoints.Count < timeIndex)
            {
                _currentHP = _timePoints[0];
                _timePoints.Clear();
            }
            else
            {
                 _currentHP = _timePoints[_timePoints.Count - timeIndex];
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
        _timePoints.Add(_currentHP);
    }

    public void RemoveFrame()
    {
        _timePoints.RemoveAt(0);
    }
}
