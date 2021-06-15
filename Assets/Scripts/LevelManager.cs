using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public void RestartLevel()
    {
        SceneManager.LoadScene("Lvl1");
    }
}
