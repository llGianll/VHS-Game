﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    private float totalTimeSubtracted;
    private float _accumulatedTimeDelta; //optimization variable 

    public bool IsRewinding { get; private set; }
    public float Timer { get; private set; }
    public Action OnRewindBegin = delegate { }; 
    public Action OnRewindEnd = delegate { }; 
    public Action OnRewindUpdate = delegate { };
    public Action OnResumeUpdate = delegate { };
    public Action OnReachedFrameThreshold = delegate { };

    [Header("Dev variables")]
    [SerializeField] bool _manualRewind = false;
    //[Optimization] only store last n seconds worth of frames, remove the rest
    [SerializeField] float _rewindFrameThreshold = 30f; //how long until a certain frame is unreachable through rewind 

    public float RewindFrameThreshold => _rewindFrameThreshold;

    [Header("Time variables")]
    [SerializeField] float _rewindTime = 5f;
    [SerializeField] float _rewindSpeedMult = 5;

    public float RewindSpeedMult => _rewindSpeedMult;

    [Header("Ui variables")]
    [SerializeField] Image _statusImg;
    [SerializeField] Sprite _playIcon;
    [SerializeField] Sprite _rewindIcon;
    [SerializeField] Text _timerText;

    List<float> _timeDeltaRecord = new List<float>();

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Timer = 0;
        OnReachedFrameThreshold += RemoveFrame;
    }

    private void RemoveFrame()
    {
        _accumulatedTimeDelta -= _timeDeltaRecord[0];
        _timeDeltaRecord.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        //if (_manualRewind)
        //{
        //    //for testing only 
        //    ManualRewindOnKeyPress();
        //}

        if (IsRewinding)
        {
            int rewindFrameCount = CountRewindFrames();
            OnRewindUpdate();
            DecreaseTimer(rewindFrameCount);
        }
        else
        {
            OnResumeUpdate();
            IncreaseTimer();
            if (_accumulatedTimeDelta > _rewindFrameThreshold)
                OnReachedFrameThreshold();
        }

        UpdateStatusUI();
    }

    private void IncreaseTimer()
    {
        totalTimeSubtracted = 0;
        Timer += Time.deltaTime;
        _accumulatedTimeDelta += Time.deltaTime;
        _timeDeltaRecord.Add(Time.deltaTime);
    }

    private void DecreaseTimer(int rewindFrameCount)
    {
        //Timer -= Time.deltaTime * _rewindSpeedMult;
        for (int i = 0; i < rewindFrameCount; i++)
        {
            if (_timeDeltaRecord.Count <= 0)
                break;

            Timer -= _timeDeltaRecord[_timeDeltaRecord.Count - 1];
            _accumulatedTimeDelta -= _timeDeltaRecord[_timeDeltaRecord.Count - 1];
            _timeDeltaRecord.RemoveAt(_timeDeltaRecord.Count - 1);
        }

        Timer = Mathf.Clamp(Timer, 0, Mathf.Infinity);

        if (Timer <= 0)
        {
            IsRewinding = false;
        }
    }

    private void ManualRewindOnKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            IsRewinding = true;
            StartCoroutine(RewindTimer());
            OnRewindBegin();
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            IsRewinding = false;
            OnRewindEnd();
        }
        else
            IncreaseTimer();

    }

    public void Rewind()
    {
        if (_manualRewind)
            return;

        StartCoroutine(RewindTimer());
    }

    public int CountRewindFrames()
    {
        int totalFrameCount = 0;

        for (int i = 1; i <= RewindSpeedMult; i++)
        {
            if (totalTimeSubtracted > _rewindTime || _timeDeltaRecord.Count <= 0 || (_timeDeltaRecord.Count - i) < 0 )
                break;

            totalFrameCount++;
            totalTimeSubtracted += _timeDeltaRecord[_timeDeltaRecord.Count - i];
        }

        return totalFrameCount;
        //return 0;
    }

    private IEnumerator RewindTimer()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1;

        totalTimeSubtracted = 0;

        IsRewinding = true;

        if (_audioSource != null)
            _audioSource.Play();

        OnRewindBegin();


        while(totalTimeSubtracted < _rewindTime)
        {
            //int rewindFrameCount = CountRewindFrames();
            //OnRewindUpdate(rewindFrameCount);
            //DecreaseTimer(rewindFrameCount);

            if (Timer < 0.0001)
                break;

            yield return null;
            //yield return new WaitForSeconds(_rewindTime/_rewindSpeedMult); //potential time inaccuracy problem 
        }
        IsRewinding = false;
        OnRewindEnd();
    }

    //[TODO]should be transferred to it's own UI class 
    private void UpdateStatusUI()
    {
        if (IsRewinding)
            _statusImg.sprite = _rewindIcon;
        else
            _statusImg.sprite = _playIcon;

        _timerText.text = string.Format("{0:#.00} secs", Timer);
    }
}
