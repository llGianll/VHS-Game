using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject _menuPanel;
    [SerializeField] GameObject _levelSelectPanel;

    [Header("Settings")]
    [SerializeField] GameSettings _gameSettings;

    // Start is called before the first frame update
    void Start()
    {
        _gameSettings.LoadGameSettings();
        _menuPanel.SetActive(true);
        _levelSelectPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        _menuPanel.SetActive(false);
        _levelSelectPanel.SetActive(true);
    }

}
