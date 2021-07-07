using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillScoring : MonoBehaviour
{
    //Maybe create a base class?
    [Header("Enemy kills")]
    [SerializeField] float _enemyKillMultiplierIncrease = 1f;

    [Header("UI Options")]
    [SerializeField] float _UIDuration = 2f;
    [SerializeField] Color _textColor;

    public Color TextColor => _textColor;

    public float Multiplier { get; set; }

    private void Start()
    {
        Multiplier = 0;
    }

    public void AddKillScore(float score)
    {
        StartCoroutine(DisplayKill(score));
    }

    IEnumerator DisplayKill(float score)
    {
        float timeElapsed = 0;
        IncreaseMultiplierOnKill();
        float killTotal = score * Multiplier;

        GameObject _killUI = PooledObjectManager.Instance.GetPooledObject(ScoreManager.Instance.ScoreUID);
        TextMeshProUGUI _killText = _killUI.GetComponent<TextMeshProUGUI>(); 

        _killText.text = "+ " + killTotal + "pts (Kill) = " + score + " * " + Multiplier;
        _killUI.transform.SetParent(ScoreManager.Instance.ScoreBreakdownParent);
        _killUI.SetActive(true);

        while (timeElapsed < _UIDuration)
        {
            float textAlpha = Mathf.Lerp(1, 0, timeElapsed / _UIDuration);
            timeElapsed += Time.deltaTime;

            _killText.color = new Color(_textColor.r, _textColor.g, _textColor.b, textAlpha);
            yield return null;
        }

        ScoreManager.Instance.LiveScore += killTotal;
        _killUI.SetActive(false);
    }

    void IncreaseMultiplierOnKill()
    {
        Multiplier += _enemyKillMultiplierIncrease;
    }
}
