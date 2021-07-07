using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] int _maxRewinds = 6;
    int _rewindsLeft = 0;

    public int RewindsLeft => _rewindsLeft;

    private void Start()
    {
        _rewindsLeft = _maxRewinds;
    }

    public void DecreaseRewind()
    {
        _rewindsLeft--;

        if(_rewindsLeft <= 0)
        {
            //no rewinds left, update ui, press r to restart
            LevelManager.Instance.RestartLevel();
        }
            
    }

    public void ReachedEndOfLevel()
    {
        ScoreManager.Instance.DisplayEndScreen();
        Time.timeScale = 0;
    }
}
