using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TimeBrackets
{
    [Header("Cleared within:")]
    [SerializeField] [Range(0, 30)] int _minutes;
    [SerializeField] [Range(0, 59)] int _seconds;

    [Header("Score Multiplier")]
    [SerializeField] [Range(1, 100)] int _multiplier;

    //getters 
    public int Minutes => _minutes;
    public int Seconds => _seconds;
    public int Multiplier => _multiplier;
}

[CreateAssetMenu(menuName = "Time Bracket Data")]
public class TimeBracketData : ScriptableObject
{
    [SerializeField] List<TimeBrackets> _timeBrackets = new List<TimeBrackets>();

    public List<TimeBrackets> GetTimeBrackets => _timeBrackets;

}
