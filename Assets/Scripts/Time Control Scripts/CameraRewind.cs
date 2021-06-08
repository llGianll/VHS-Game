using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRewind : MonoBehaviour, IRewindable
{
    #region CameraTimePoint nested class

    public class CameraTimePoint
    {
        public Quaternion CameraRotation;
        public float YRotation;

        public CameraTimePoint(Quaternion cameraRotation, float yRotation)
        {
            CameraRotation = cameraRotation;
            YRotation = yRotation;
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
        CameraTimePoint timePoint = new CameraTimePoint(_cameraHolder_T.rotation, _mouseLook.yRotation);
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
        _mouseLook.yRotation = timePoint.YRotation;
    }
}
