using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ScoreBrackets
{
    [SerializeField] string _rank;
    [SerializeField] int _points;

    public int Points => _points;
    public string Rank => _rank;
}

[CreateAssetMenu(menuName = "Score Bracket Data")]
public class ScoreBracketData : ScriptableObject
{
    [SerializeField] List<ScoreBrackets> _scoreBrackets = new List<ScoreBrackets>();
    public List<ScoreBrackets> GetScoreBrackets => _scoreBrackets;
}
