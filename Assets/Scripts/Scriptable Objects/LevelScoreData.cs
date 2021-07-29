using UnityEngine;

[CreateAssetMenu(menuName = "Level Score Data")]
public class LevelScoreData : ScriptableObject
{
    public string levelKey;
    private float _score;

    public float GetHighScore()
    {
        return _score;
    }

    public void LoadHighScore()
    {
        _score = PlayerPrefs.GetFloat(levelKey, 0);
    }

    public void SaveHighScore(float score)
    {
        if(score > PlayerPrefs.GetFloat(levelKey, 0))
        {
            _score = score;
            PlayerPrefs.SetFloat(levelKey, score);
        }
    }
}
