using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    [Range(0, 10)] public float mouseSensitivityX;
    [Range(0, 10)] public float mouseSensitivityY;
    [Range(0, 100)] public float bgmVolume;
    [Range(0, 100)] public float sfxVolume;

    public void SaveGameSettings()
    {
        PlayerPrefs.SetFloat("mouseX", mouseSensitivityX);
        PlayerPrefs.SetFloat("mouseY", mouseSensitivityY);
        PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    public void LoadGameSettings()
    {
        //call on game start button on main menu
        mouseSensitivityX = PlayerPrefs.GetFloat("mouseX", 5);
        mouseSensitivityY =  PlayerPrefs.GetFloat("mouseY", 5);
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume", 100);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100);
    }
}
