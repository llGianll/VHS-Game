using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRewind : MonoBehaviour, IRewindable
{
    List<Quaternion> _cameraRotations = new List<Quaternion>();
    CharacterControllerMouseLook _mouseLook;

    private void Awake()
    {
        _mouseLook = GetComponent<CharacterControllerMouseLook>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeController.Instance.OnRewind += Rewind;
        TimeController.Instance.OnResume += StopRewind;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeController.Instance.IsRewinding)
            RewindTimePoints();
        else
            RecordTimePoints();
    }

    public void RewindTimePoints()
    {
        if (_cameraRotations.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_cameraRotations.Count < timeIndex)
            {
                SetRotation(_cameraRotations[0]);
                _cameraRotations.Clear();
            }
            else
            {
                SetRotation(_cameraRotations[_cameraRotations.Count - timeIndex]);
                for (int i = 0; i < timeIndex; i++)
                {
                    _cameraRotations.RemoveAt(_cameraRotations.Count - 1);
                }
            }

        }
        else
            StopRewind();
    }

    public void RecordTimePoints()
    {
        _cameraRotations.Add(transform.rotation);
    }

    public void Rewind()
    {
        _mouseLook.enabled = false;
    }

    public void StopRewind()
    {
        _mouseLook.enabled = true;
    }

    private void SetRotation(Quaternion cameraRotation)
    {
        transform.rotation = cameraRotation;
    }
}
