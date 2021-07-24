using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class ConfirmWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _windowText;
    [SerializeField] GameObject _pauseOptionPanel;
    PauseAction _pauseAction;

    private void OnEnable()
    {
        _pauseOptionPanel.SetActive(false);        
    }

    private void OnDisable()
    {
        _pauseOptionPanel.SetActive(true);
    }

    public void WindowSetup(PauseAction pauseAction , string windowText)
    {
        _pauseAction = pauseAction;
        _windowText.text = windowText;
    }

    public void YesButton()
    {
        _pauseAction.ExecutePauseAction();
    }

    public void NoButton()
    {
        //_pauseOptionPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
