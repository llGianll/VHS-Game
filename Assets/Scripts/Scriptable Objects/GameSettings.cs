using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    [Range(0, 10)] public float mouseSensitivityX;
    [Range(0, 10)] public float mouseSensitivityY;
}
