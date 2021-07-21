using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] int _maxRewinds = 6;
    int _rewindsLeft = 0;

    public int RewindsLeft => _rewindsLeft;
    public static bool isPaused;

    private void Start()
    {
        _rewindsLeft = _maxRewinds;
        isPaused = false;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            PauseGame();
        }

    }

    private void PauseGame()
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            Cursor.visible = false;
        }
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
