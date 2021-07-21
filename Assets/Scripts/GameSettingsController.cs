using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameSettingsController : Singleton<GameSettingsController>
{
    [SerializeField] GameObject _gameSettingsPanel;
    [SerializeField] GameSettings _clientSettings;
    [SerializeField] GameSettings _defaultSettings;
    [SerializeField] UISliderValue _mouseSenseX;
    [SerializeField] UISliderValue _mouseSenseY;

    //public bool gamePaused;
    // Start is called before the first frame update
    void Start()
    {
        InitialClientSettings();
        _gameSettingsPanel.SetActive(false);
        UIEventInit();
        //gamePaused = false;
    }

    private void UIEventInit()
    {
        _mouseSenseX.OnSliderChanged += ModifyMouseSensitivityX;
        _mouseSenseY.OnSliderChanged += ModifyMouseSensitivityY;

        _mouseSenseX.OnInputValueChanged += ModifyMouseSensitivityX;
        _mouseSenseY.OnInputValueChanged += ModifyMouseSensitivityY;
    }

    private void InitialClientSettings()
    {
        _mouseSenseX.InitializeUIValues(_clientSettings.mouseSensitivityX);
        _mouseSenseY.InitializeUIValues(_clientSettings.mouseSensitivityY);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isPaused)
            _gameSettingsPanel.SetActive(true);
        else
            _gameSettingsPanel.SetActive(false);

    }

    public void ModifyMouseSensitivityX(float value) { _clientSettings.mouseSensitivityX = value; }
    public void ModifyMouseSensitivityY(float value) { _clientSettings.mouseSensitivityY = value; }

}
