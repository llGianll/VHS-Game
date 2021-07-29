using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettingsController : Singleton<GameSettingsController>
{
    [SerializeField] GameObject _gameSettingsPanel;
    [SerializeField] GameObject _pausePanel;
    [SerializeField] GameObject _confirmWindow;
    [SerializeField] GameSettings _clientSettings;
    [SerializeField] GameSettings _defaultSettings;
    [SerializeField] UISliderValue _mouseSenseX;
    [SerializeField] UISliderValue _mouseSenseY;
    [SerializeField] UISliderValue _bgmSlider;
    [SerializeField] UISliderValue _sfxSlider;

    [SerializeField] AudioMixer _audioMixer;

    //public bool gamePaused;
    // Start is called before the first frame update
    void Start()
    {
        InitialClientSettings();
        _gameSettingsPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _confirmWindow.SetActive(false);
        UIEventInit();
        //gamePaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.isPaused)
        //    _gameSettingsPanel.SetActive(true);
        //else
        //    _gameSettingsPanel.SetActive(false);

        if (GameManager.isPaused)
        {
            if(!_confirmWindow.activeSelf)
                _pausePanel.SetActive(true);

        }
        else
        {
            _pausePanel.SetActive(false);
            _gameSettingsPanel.SetActive(false);
            _confirmWindow.SetActive(false);
        }

    }

    private void InitialClientSettings()
    {
        _mouseSenseX.InitializeUIValues(_clientSettings.mouseSensitivityX);
        _mouseSenseY.InitializeUIValues(_clientSettings.mouseSensitivityY);

        //audio mixer entries
        _bgmSlider.InitializeUIValues(_clientSettings.bgmVolume);
        _sfxSlider.InitializeUIValues(_clientSettings.sfxVolume);
        _audioMixer.SetFloat("BGMVolume", Mathf.Log10(_clientSettings.bgmVolume / 100) * 20);
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(_clientSettings.sfxVolume / 100) * 20);
    }

    private void UIEventInit()
    {
        _mouseSenseX.OnSliderChanged += ModifyMouseSensitivityX;
        _mouseSenseY.OnSliderChanged += ModifyMouseSensitivityY;

        _mouseSenseX.OnInputValueChanged += ModifyMouseSensitivityX;
        _mouseSenseY.OnInputValueChanged += ModifyMouseSensitivityY;

        _bgmSlider.OnSliderChanged += ModifyBGMVolume;
        _sfxSlider.OnSliderChanged += ModifySFXVolume;

        _bgmSlider.OnInputValueChanged += ModifyBGMVolume;
        _sfxSlider.OnInputValueChanged += ModifySFXVolume;
    }

    private void ModifyBGMVolume(float volume)
    {
        _clientSettings.bgmVolume = volume;
        _audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume / 100) * 20);

        _clientSettings.SaveGameSettings();
    }

    private void ModifySFXVolume(float volume)
    {
        _clientSettings.sfxVolume = volume;
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume / 100) * 20);

        _clientSettings.SaveGameSettings();
    }

    public void ModifyMouseSensitivityX(float value)
    {
        _clientSettings.mouseSensitivityX = value;

        _clientSettings.SaveGameSettings();
    }
    public void ModifyMouseSensitivityY(float value)
    {
        _clientSettings.mouseSensitivityY = value;

        _clientSettings.SaveGameSettings();
    }

    public void ShowSettingsWindow()
    {
        _pausePanel.SetActive(false);
        _gameSettingsPanel.SetActive(true);
    }

    public void ShowPausePanel()
    {
        _gameSettingsPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void RestartLevelAction()
    {
        LevelManager.Instance.RestartLevel();
    }

    public void ReturnToMainMenu()
    {
        LevelManager.Instance.ReturnToMainMenu();
    }

    public void ExitGame()
    {
        LevelManager.Instance.ExitGame();
    }
}
