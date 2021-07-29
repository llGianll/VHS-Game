using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    [SerializeField] LevelScoreData levelScoreData;
    [SerializeField] TextMeshProUGUI _scoreText;

    void Start()
    {
        levelScoreData.LoadHighScore();
        _scoreText.text = "High Score: " + levelScoreData.GetHighScore();
    }

}
