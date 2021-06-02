using UnityEngine;

public class CharacterControllerMouseLook : MonoBehaviour
{
    [SerializeField]
    Transform playerObject;

    Camera playerCam;

    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    float mouseX;
    float mouseY;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public float yAxisClampNegative = -90f;
    public float yAxisClampPositive = 90f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * mouseSensitivityX;
        xRotation -= mouseY * mouseSensitivityY;

        xRotation = Mathf.Clamp(xRotation, yAxisClampNegative, yAxisClampPositive);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        playerObject.Rotate(Vector3.up * mouseX);
    }
}
