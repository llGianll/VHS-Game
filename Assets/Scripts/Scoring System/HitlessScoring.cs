using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitlessScoring : MonoBehaviour
{
    [Header("Hitless Computations")]
    [SerializeField] float _hitlessInterval = 10f;
    [SerializeField] float _hitScoreMultiplierIncrease = 1f;
    [SerializeField] float _baseHitlessScore = 100f;
    float _hitlessMultiplier = 0f;
    float _secondsWithoutHit;

    [Header("UI Options")]
    [SerializeField] float _hitlessUIDuration = 2f;
    [SerializeField] Color _textColor;

    public Color TextColor => _textColor;
    public float Multiplier => _hitlessMultiplier;

    private void Start()
    {
        _secondsWithoutHit = 0;
        _hitlessMultiplier = 0;
    }

    private void Update()
    {
        NoHitComputation();
    }

    private void NoHitComputation()
    {
        if (!TimeController.Instance.IsRewinding)
            _secondsWithoutHit += Time.deltaTime;
        else
        {
            _hitlessMultiplier = 0;
            _secondsWithoutHit = 0;
        }

        if (_secondsWithoutHit >= _hitlessInterval)
        {
            _hitlessMultiplier += _hitScoreMultiplierIncrease;
            _secondsWithoutHit = 0;
            StartCoroutine(DisplayHitless());
        }
    }

    IEnumerator DisplayHitless()
    {
        float timeElapsed = 0;
        float hitlessTotal = _baseHitlessScore * _hitlessMultiplier;

        GameObject hitlessUI = PooledObjectManager.Instance.GetPooledObject(ScoreManager.Instance.ScoreUID);
        TextMeshProUGUI hitlessText = hitlessUI.GetComponent<TextMeshProUGUI>();

        hitlessText.text = "+ " + hitlessTotal + "pts (Hitless) = " + _baseHitlessScore + " * " + _hitlessMultiplier;
        hitlessUI.transform.SetParent(ScoreManager.Instance.ScoreBreakdownParent);
        hitlessUI.SetActive(true);

        while (timeElapsed < _hitlessUIDuration)
        {
            float textAlpha = Mathf.Lerp(1, 0, timeElapsed / _hitlessUIDuration);
            timeElapsed += Time.deltaTime;

            hitlessText.color = new Color(_textColor.r, _textColor.g, _textColor.b, textAlpha);
            yield return null;
        }

        ScoreManager.Instance.LiveScore += hitlessTotal;
        hitlessUI.SetActive(false);
    }
}
