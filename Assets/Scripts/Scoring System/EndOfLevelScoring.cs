using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndOfLevelScoring : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject _endScorePanel;
    [SerializeField] TextMeshProUGUI _accumulatedText;
    [SerializeField] TextMeshProUGUI _multiplierText;
    [SerializeField] TextMeshProUGUI _totalScoreText;
    [SerializeField] TextMeshProUGUI _scoreTableText;
    [SerializeField] TextMeshProUGUI _clearTime;
    [SerializeField] TextMeshProUGUI _rankText;

    [Header("Scriptable Objects")]
    [SerializeField] TimeBracketData _timeBracketData;
    [SerializeField] ScoreBracketData _scoreBracketData;

    int _endMultiplier;
    int _totalScore;
    string _rank;
    TextMeshProUGUI _realTimeText;

    // Start is called before the first frame update
    void Start()
    {
        _realTimeText = GameObject.Find("Rewind UI Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _endScorePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _realTimeText.text = ((int)(TimeController.Instance.RealTime / 60)).ToString() + ":"
            + ((int)(TimeController.Instance.RealTime % 60)).ToString("00");
    }

    public void MultiplierTimeEquivalent()
    {
        int clearTimeInSeconds = (int)(TimeController.Instance.RealTime);

        //only works when entries are in ascending order
        if (_timeBracketData == null)
            return;

        foreach (var time in _timeBracketData.GetTimeBrackets)
        {
            int bracketTimeInSeconds = (time.Minutes * 60) + time.Seconds; 
            if (clearTimeInSeconds <= bracketTimeInSeconds)
            {
                _endMultiplier = time.Multiplier;
                Debug.Log(_endMultiplier);
                return;
            }
        }

        _endMultiplier = 1; //default 
        Debug.Log(_endMultiplier);
    }

    public void ScoreAndRank()
    {
        _totalScore = (int)ScoreManager.Instance.LiveScore * _endMultiplier;
        _rank = null;

        if (_scoreBracketData == null)
            return;

        foreach (var score in _scoreBracketData.GetScoreBrackets)
        {
            if (_totalScore >= score.Points)
            {
                _rank = score.Rank;
                break;
            }
        }

        if(_rank == null)
        {
            _rank = _scoreBracketData.GetScoreBrackets[_scoreBracketData.GetScoreBrackets.Count - 1].ToString();
        }
    }

    public void SetInfoToUI()
    {
        _accumulatedText.text = ((int)ScoreManager.Instance.LiveScore).ToString();
        _multiplierText.text = _endMultiplier.ToString();
        _totalScoreText.text = _totalScore.ToString();

        SetScoreBracketTable();

        _clearTime.text = ((int)(TimeController.Instance.RealTime / 60)).ToString() + ":"
            + ((int)(TimeController.Instance.RealTime % 60)).ToString("00");
        _rankText.text = _rank;

        _endScorePanel.SetActive(true);
    }

    private void SetScoreBracketTable()
    {
        string rankTable = "";
        foreach (var score in _scoreBracketData.GetScoreBrackets)
        {
            rankTable += score.Points + "+ " + score.Rank + "\n";
        }

        _scoreTableText.text = rankTable;
    }
}
