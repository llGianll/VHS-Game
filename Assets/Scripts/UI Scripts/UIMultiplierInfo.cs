using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMultiplierInfo : MonoBehaviour
{
    Image _multiplierImage;
    TextMeshProUGUI _multiplierText;
    [SerializeField] ScoreType _scoreType;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.Instance.ScoreBreakdownParent = transform.parent.parent.GetChild(2);
        _multiplierImage = transform.GetChild(0).GetComponent<Image>();
        _multiplierText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _multiplierImage.color = ScoreManager.Instance.GetMultiplierUIColor(_scoreType);
    }

    // Update is called once per frame
    void Update()
    {
        _multiplierText.text = (ScoreManager.Instance.GetMultiplier(_scoreType) + 1f).ToString() + "x";
    }
}
