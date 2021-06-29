using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : Singleton<SFXPlayer>
{
    [System.Serializable]
    private class SFXDictionary
    {
        public SFXPresets sfxName;
        public AudioSource audioSource;
    }

    [SerializeField] List<SFXDictionary> _soundEffects = new List<SFXDictionary>();

    public void Play(SFXPresets sfxName)
    {
        int sfxIndex = SFXIndexSearch(sfxName);
        if (IsSFXPlayable(sfxIndex))
        {
            _soundEffects[sfxIndex].audioSource.Play();
        }
    }

    public void Stop(SFXPresets sfxName)
    {
        int sfxIndex = SFXIndexSearch(sfxName);
        if (IsSFXPlayable(sfxIndex))
        {
            _soundEffects[sfxIndex].audioSource.Stop();
        }
    }

    public void PlayClipAtPoint(SFXPresets sfxName, Vector3 sfxPos)
    {
        int sfxIndex = SFXIndexSearch(sfxName);
        if (IsSFXPlayable(sfxIndex))
        {
            AudioSource.PlayClipAtPoint(_soundEffects[sfxIndex].audioSource.clip, sfxPos);
        }
    }

    private int SFXIndexSearch(SFXPresets sfxName)
    {
        int index = 0;
        foreach (var sfx in _soundEffects)
        {
            if (sfxName == sfx.sfxName)
                return index;

            index++;
        }


        return -1;
    }

    private bool IsSFXPlayable(int sfxIndex)
    {
        return sfxIndex >= 0 && _soundEffects[sfxIndex].audioSource != null;
    }


}

public enum SFXPresets
{
    Shoot, 
    Reload, 
    Jump,
    PlayerHit, 
    TurretHit, 
    Footstep_1,
    Footstep_2, 
    Explosion_1
}