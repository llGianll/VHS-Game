using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WallrunScoring : MonoBehaviour
{
    [Header("Wallrun score")]
    [SerializeField] float _wallrunBaseScore = 50f;
    public float WallrunDuration { get; set; }

    [Header("UI Options")]
    [SerializeField] float _UIDuration = 2f;
    [SerializeField] Color _textColor;

    private RigidbodyWallrun _rbWallRun;

    GameObject _wallrunUI;
    TextMeshProUGUI _wallrunText;

    bool _isScoringActive = false;
    float _elapsedTime;

    private void Awake()
    {
        _rbWallRun = FindObjectOfType<RigidbodyWallrun>();
    }

    // Start is called before the first frame update
    void Start()
    {
        WallrunDuration = 0f;
        _elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        WallrunComputation();
    }

    private void WallrunComputation()
    {
        if (_rbWallRun.isWallRunning)
        {
            if (_wallrunUI == null)
            {
                _wallrunUI = PooledObjectManager.Instance.GetPooledObject(ScoreManager.Instance.ScoreUID);
                _wallrunText = _wallrunUI.GetComponent<TextMeshProUGUI>();
            }

            _isScoringActive = true;

            WallrunDuration += Time.deltaTime;
            _wallrunText.text = "+ " + Mathf.RoundToInt(_wallrunBaseScore * WallrunDuration) + "pts (Wallrun) = "
                + _wallrunBaseScore + " * " + WallrunDuration.ToString("0.00")+" secs";

            _wallrunText.color = new Color(_textColor.r, _textColor.g, _textColor.b, 1);
            _wallrunUI.transform.SetParent(ScoreManager.Instance.ScoreBreakdownParent);
            _wallrunUI.SetActive(true);
            _elapsedTime = 0;
        }
        else
        {
            if (_isScoringActive)
            {
                _wallrunText.color = new Color(_textColor.r, _textColor.g, _textColor.b, 1 - (_elapsedTime/_UIDuration));
                _elapsedTime += Time.deltaTime;
                if(_elapsedTime >= _UIDuration)
                {
                    ScoreManager.Instance.LiveScore += Mathf.RoundToInt(_wallrunBaseScore * WallrunDuration);
                    _isScoringActive = false;
                    _wallrunUI.SetActive(false);
                    WallrunDuration = 0;
                    _elapsedTime = 0;
                    _wallrunUI = null;
                    _wallrunText = null;
                }
            }
        }
        
    }
}
