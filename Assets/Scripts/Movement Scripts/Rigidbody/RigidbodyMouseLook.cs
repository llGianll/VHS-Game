using UnityEngine;

public class RigidbodyMouseLook : MonoBehaviour
{

    [SerializeField]
    Transform playerCamHolder;

    [SerializeField]
    Transform orientation;

    [SerializeField]
    RigidbodyWallrun wallrun;

    [SerializeField]
    RigidbodyPlayerMovement rigidbodyPlayerMovement;

    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    float mouseX;
    float mouseY;

    //need x and y rotation to be recorded and manipulated by camera rewind script
    [HideInInspector]
    public float xRotation = 0f; 

    [HideInInspector]
    public float yRotation = 0f; 
    public float yRotationClamped = 0f; 

    public float yAxisClampNegative = -90f;
    public float yAxisClampPositive = 90f;
    public float xAxisClampNegative = -90f;
    public float xAxisClampPositive = 90f;
    float yRotationNegative = -360;
    float yRotationPositive = 0;
    float yRotationDefaultNegative = -360;
    float yRotationDefaultPositive = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerCamHolder = GameObject.Find("RigidbodyCameraHolder").transform;
        Cursor.lockState = CursorLockMode.Locked;
        yRotationNegative = yRotationDefaultNegative;
        yRotationPositive = yRotationDefaultPositive;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * mouseSensitivityX;
        xRotation -= mouseY * mouseSensitivityY;

        xRotation = Mathf.Clamp(xRotation, yAxisClampNegative, yAxisClampPositive);
        

        playerCamHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallrun.tilt);
        if (!wallrun.isWallRunning)
        {
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);//the piece of code rotating player's body
            yRotationNegative = yRotationDefaultNegative;
            yRotationPositive = yRotationDefaultPositive;
        }
        else if (wallrun.isWallRunning)
        {
            if (wallrun.isLeftWallHit)
            {
                Debug.Log("wallrun clamped left");
                yRotationNegative = -90 + xAxisClampNegative;
                yRotationPositive = -90 + xAxisClampPositive;
                //yRotation = Mathf.Clamp(yRotation, yRotationNegative, yRotationPositive);
            }
            else if (wallrun.isRightWallHit)
            {
                Debug.Log("wallrun clamped right");
                yRotationClamped = Mathf.Clamp(yRotation, xAxisClampNegative, xAxisClampPositive);
                yRotationNegative = xAxisClampNegative;
                yRotationPositive = xAxisClampPositive;
                //yRotation = Mathf.Clamp(yRotation, yRotationNegative, yRotationPositive);
            }
        }


    }
}
