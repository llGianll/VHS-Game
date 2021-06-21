using UnityEngine;

public class RigidbodyMouseLook : MonoBehaviour
{

    [SerializeField]
    Transform playerCamHolder;

    [SerializeField]
    Transform orientation;

    [SerializeField] RigidbodyWallrun wallrun;

    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    float mouseX;
    float mouseY;

    //need x and y rotation to be recorded and manipulated by camera rewind script
    [HideInInspector]
    public float xRotation = 0f; 

    [HideInInspector]
    public float yRotation = 0f; 

    public float yAxisClampNegative = -90f;
    public float yAxisClampPositive = 90f;

    // Start is called before the first frame update
    void Start()
    {
        playerCamHolder = GameObject.Find("RigidbodyCameraHolder").transform;
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

        playerCamHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation + wallrun.cameraRotation, wallrun.tilt);
        if (!wallrun.isWallRunning)
        {
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);//the piece of code rotating player's body
        }


    }
}
