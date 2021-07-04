using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    [SerializeField] Transform cameraPosition;

    private void Start()
    {
        //cameraPosition = GameObject.Find("CameraPosition").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
