using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : Singleton<ScoreManager>, IRewindable
{
    #region ScoreTimePoint class
    private class ScoreTimePoint
    {
        public float LiveScore;
        public float KillMultiplier;
        public float WallrunDuration;

        public ScoreTimePoint(float liveScore, float killMultiplier, float wallrunDuration)
        {
            LiveScore = liveScore;
            KillMultiplier = killMultiplier;
            WallrunDuration = wallrunDuration;
        }
    }
    #endregion 

    public Transform ScoreBreakdownParent { get; set; }

    [SerializeField] String _scoreUID;
    public String ScoreUID => _scoreUID;

    KillScoring _killScoring;
    HitlessScoring _hitlessScoring;
    WallrunScoring _wallrunScoring;
    EndOfLevelScoring _endOfLevelScoring;

    [SerializeField] TextMeshProUGUI _liveScoreText;
    public float LiveScore { get; set; }

    List<ScoreTimePoint> _scoreTimePoints = new List<ScoreTimePoint>();

    protected override void Awake()
    {
        base.Awake();
        _killScoring = GetComponent<KillScoring>();
        _hitlessScoring = GetComponent<HitlessScoring>();
        _wallrunScoring = GetComponent<WallrunScoring>();
        _endOfLevelScoring = GetComponent<EndOfLevelScoring>();
    }

    private void Start()
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

    // Update is called once per frame
    void Update()
    {
        _liveScoreText.text =  "Score: " + LiveScore.ToString();
    }

    public void AddKillScore(float score)
    {
        _killScoring.AddKillScore(score);
    }

    public Color GetMultiplierUIColor(ScoreType scoreType)
    {
        switch (scoreType)
        {
            case ScoreType.Kill:
                return _killScoring.TextColor;
            case ScoreType.Hitless:
                return _hitlessScoring.TextColor;
            default:
                return Color.white;
        }
    }

    public float GetMultiplier(ScoreType scoreType)
    {
        switch (scoreType)
        {
            case ScoreType.Kill:
                return _killScoring.Multiplier;
            case ScoreType.Hitless:
                return _hitlessScoring.Multiplier;
            default:
                return 5f;
        }
    }

    public void DisplayEndScreen()
    {
        _endOfLevelScoring.MultiplierTimeEquivalent();
        _endOfLevelScoring.ScoreAndRank();
        _endOfLevelScoring.SetInfoToUI();
    }

    public void Rewind()
    {
        //EnableScoring(false);
    }

    private void EnableScoring(bool enabled)
    {
        _killScoring.enabled = enabled;
        _hitlessScoring.enabled = enabled;
        _wallrunScoring.enabled = enabled;
    }

    public void StopRewind()
    {
        //EnableScoring(true);
    }

    public void RewindTimePoints()
    {
        if (_scoreTimePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_scoreTimePoints.Count < timeIndex)
            {
                SetScoreTimePoint(_scoreTimePoints[0]);
                _scoreTimePoints.Clear();
            }
            else
            {
                SetScoreTimePoint(_scoreTimePoints[_scoreTimePoints.Count - timeIndex]);
                for (int i = 0; i < timeIndex; i++)
                {
                    _scoreTimePoints.RemoveAt(_scoreTimePoints.Count - 1);
                }
            }

        }
        else
            StopRewind();
    }

    private void SetScoreTimePoint(ScoreTimePoint scoreTimePoint)
    {
        LiveScore = scoreTimePoint.LiveScore;
        _killScoring.Multiplier = scoreTimePoint.KillMultiplier;
        _wallrunScoring.WallrunDuration = scoreTimePoint.WallrunDuration;
    }

    public void RecordTimePoints()
    {
        ScoreTimePoint data = new ScoreTimePoint(LiveScore, _killScoring.Multiplier, _wallrunScoring.WallrunDuration);
        _scoreTimePoints.Add(data);
    }

    public void RemoveFrame()
    {
        _scoreTimePoints.RemoveAt(0);
    }
}

public enum ScoreType
{
    Kill, 
    Hitless, 
    Wallrun
}