using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseAction : MonoBehaviour
{
    [SerializeField] ConfirmWindow _confirmWindow;
    [SerializeField] GameObject _pausePanel;
    [SerializeField] UnityEvent _pauseAction;

    [TextArea(5, 6)]
    [SerializeField] string windowText; 

    public void ShowConfirmWindow()
    {
        _confirmWindow.WindowSetup(this, windowText);
        _confirmWindow.gameObject.SetActive(true);
    }

    public void ExecutePauseAction()
    {
        _pauseAction.Invoke();
    }
}
