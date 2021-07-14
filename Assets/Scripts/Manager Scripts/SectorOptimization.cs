using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorOptimization : MonoBehaviour
{
    public static SectorOptimization Instance;

    LevelSector _activeSector;
    public LevelSector ActiveSector => _activeSector;

    public Action OnActiveSectorChange = delegate { };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetActiveSector(LevelSector levelSector)
    {
        _activeSector = levelSector;
        OnActiveSectorChange?.Invoke();
    }
}
