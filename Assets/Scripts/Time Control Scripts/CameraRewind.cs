using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRewind : MonoBehaviour, IRewindable
{
    #region CameraTimePoint nested class

    [System.Serializable]
    public class CameraTimePoint
    {
        public Quaternion CameraRotation;
        public float YRotation;
        public float XRotation;

        public CameraTimePoint(Quaternion cameraRotation, float xRotation, float yRotation)
        {
            CameraRotation = cameraRotation;
            YRotation = yRotation;
            XRotation = xRotation;
        }
    }

    #endregion

    List<CameraTimePoint> _cameraTimePoints = new List<CameraTimePoint>();
    RigidbodyMouseLook _mouseLook;
    [SerializeField] Transform _cameraHolder_T;


    private void Awake()
    {
        _mouseLook = GetComponent<RigidbodyMouseLook>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeControllerEventsInit();
    }

    private void TimeControllerEventsInit()
    {
        TimeController.Instance.OnRewindBegin += Rewind;
        TimeController.Instance.OnRewindEnd += StopRewind;
        TimeController.Instance.OnRewindUpdate += RewindTimePoints;
        TimeController.Instance.OnResumeUpdate += RecordTimePoints;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RewindTimePoints()
    {
        if (_cameraTimePoints.Count > 0)
        {
            int timeIndex = (int)TimeController.Instance.RewindSpeedMult;

            if (_cameraTimePoints.Count < timeIndex)
            {
                SetCameraTimePoint(_cameraTimePoints[0]);
                _cameraTimePoints.Clear();
            }
            else
            {
                SetCameraTimePoint(_cameraTimePoints[_cameraTimePoints.Count - timeIndex]);
                for (int i = 0; i < timeIndex; i++)
                {
                    _cameraTimePoints.RemoveAt(_cameraTimePoints.Count - 1);
                }
            }

        }
        else
            StopRewind();
    }

    public void RecordTimePoints()
    {
        CameraTimePoint timePoint = new CameraTimePoint(_cameraHolder_T.rotation, _mouseLook.xRotation , _mouseLook.yRotation);
        _cameraTimePoints.Add(timePoint);
    }

    public void Rewind()
    {
        _mouseLook.enabled = false;
    }

    public void StopRewind()
    {
        _mouseLook.enabled = true;
    }

    private void SetCameraTimePoint(CameraTimePoint timePoint)
    {
        _cameraHolder_T.rotation = timePoint.CameraRotation;
        _mouseLook.xRotation = timePoint.XRotation;
        _mouseLook.yRotation = timePoint.YRotation;
    }
}
