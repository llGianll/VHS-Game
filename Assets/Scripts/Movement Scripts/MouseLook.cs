using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    Transform playerObject;

    public float mouseSensitivity = 100f;

    float mouseX;
    float mouseY;

    private float xRotation = 0f;

    public float yAxisClampNegative = -90f;
    public float yAxisClampPositive = 90f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, yAxisClampNegative, yAxisClampPositive);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerObject.Rotate(Vector3.up * mouseX);
    }
}
