using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] GameSettings _gameSettings;

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void PlayButton()
    {
        if(_gameSettings != null)
            _gameSettings.LoadGameSettings();

        SceneManager.LoadScene("Level_Select");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
