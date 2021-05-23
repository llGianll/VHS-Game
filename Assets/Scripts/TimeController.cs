using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    public bool IsRewinding { get; private set; }
    public float Timer { get; private set; }
    public Action OnRewind = delegate { };
    public Action OnResume = delegate { };

    [Header("Time variables")]
    [SerializeField] float _rewindTime = 5f;
    [SerializeField] float _rewindSpeedMult = 5;

    public float RewindSpeedMult => _rewindSpeedMult;

    [Header("Ui variables")]
    [SerializeField] Image _statusImg;
    [SerializeField] Sprite _playIcon;
    [SerializeField] Sprite _rewindIcon;
    [SerializeField] Text _timerText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //IsRewinding = true;
            //StartCoroutine(RewindTimer());
            //OnRewind();
            //Debug.Log("------Rewind------");
        }

        //if (Input.GetKeyUp(KeyCode.Return))
        //{
        //    IsRewinding = false;
        //    OnResume();
        //    Debug.Log("------Resume------");
        //}

        if (IsRewinding)
        {
            Timer -= Time.deltaTime * _rewindSpeedMult;
            Timer = Mathf.Clamp(Timer, 0, Mathf.Infinity);

            if (Timer <= 0)
                IsRewinding = false;

        }
        else
            Timer += Time.deltaTime;


        UpdateStatusUI();
    }

    public void Rewind()
    {
        IsRewinding = true;
        StartCoroutine(RewindTimer());
    }

    private IEnumerator RewindTimer()
    {
        OnRewind();
        Debug.Log("------Rewind------");
        yield return new WaitForSeconds(_rewindTime/_rewindSpeedMult); //potential time inaccuracy problem 
        IsRewinding = false;
        OnResume();
        Debug.Log("------Resume------");
    }

    private void UpdateStatusUI()
    {
        if (IsRewinding)
            _statusImg.sprite = _rewindIcon;
        else
            _statusImg.sprite = _playIcon;

        _timerText.text = string.Format("{0:#.00} secs", Timer);
    }
}
