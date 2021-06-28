using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyWallrun : MonoBehaviour
{
    [SerializeField]
    Transform orientation;

    [SerializeField]
    RigidbodyPlayerMovement rigidbodyPlayerMovement;

    public float wallDistance;
    public float minimumJumpHeight;
    //public float forwardWallJumpVectorDistance = 0.5f;
    public float playerWallRunSpeed = 15;

    public Vector3 wallLeftNormal;
    public Vector3 wallRightNormal;

    [SerializeField]
    private float wallRunGravity;
    [SerializeField]
    private float horizontalWallRunJumpForce;
    [SerializeField]
    private float verticalWallRunJumpForce;
    [SerializeField]
    private float angledWallRunJumpForce;

    [SerializeField]
    LayerMask wallMask;

    public bool isWallRunning;

    [Header("Camera Effects")]
    [SerializeField] Camera mainCamera;
    [SerializeField] private float normalFov;
    [SerializeField] private float wallRunFov;
    [SerializeField] private float fovToWallRunFovTransitionTime;
    [SerializeField] private float wallRunCameraTilt;
    [SerializeField] private float wallRunCameraTiltTime;
    [SerializeField] private float wallRunCameraRotation;
    [SerializeField] private float wallRunCameraRotationTime;

    public float tilt { get; private set; }

    public float cameraRotation { get; private set; }
    

    bool wallLeft = false;
    bool wallRight = false;

    public bool isLeftWallHit;
    public bool isRightWallHit;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rigidBody;

    Transform mainCameraHolder;

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);

    }

    

    // Start is called before the first frame update
    void Start()
    {
        mainCameraHolder = GameObject.Find("RigidbodyCameraHolder").transform;
        rigidBody = GetComponent<Rigidbody>();

        mainCamera = Camera.main;
        normalFov = mainCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rigidbodyPlayerMovement.isGrounded)
        {
            WallCheck();

            if (CanWallRun())
            {
                if (wallLeft || wallRight)
                {
                    StartWallRun();
                }
                else
                {
                    NormalizeCameraEffects();
                    StopWallRun();
                }
            }
        }
        else
        {
            isWallRunning = false;
        }

        if(isWallRunning && rigidbodyPlayerMovement.isGrounded)
        {
            StopWallRun();
            NormalizeCameraEffects();
        }

        isLeftWallHit = wallLeft;
        isRightWallHit = wallRight;
    }

    void WallCheck()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, wallMask);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, wallMask);

    }

    void StartWallRun()
    {
        rigidbodyPlayerMovement.GravityOff();
        isWallRunning = true;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, wallRunFov, fovToWallRunFovTransitionTime * Time.deltaTime);
        
        //rigidBody.AddForce(Vector3.down * -wallRunGravity, ForceMode.Acceleration);
        
        rigidbodyPlayerMovement.playerSpeed = Mathf.Lerp(rigidbodyPlayerMovement.playerSpeed, playerWallRunSpeed, rigidbodyPlayerMovement.playerAcceleration * Time.deltaTime);

        if (wallLeft)
        {
            wallLeftNormal = leftWallHit.normal;
            tilt = Mathf.Lerp(tilt, -wallRunCameraTilt, wallRunCameraTiltTime * Time.deltaTime);
            cameraRotation = Mathf.Lerp(cameraRotation, wallRunCameraRotation, wallRunCameraRotationTime * Time.deltaTime);

        }
        else if (wallRight)
        {
            wallRightNormal = rightWallHit.normal;
            tilt = Mathf.Lerp(tilt, wallRunCameraTilt, wallRunCameraTiltTime * Time.deltaTime);
            cameraRotation = Mathf.Lerp(cameraRotation, -wallRunCameraRotation, wallRunCameraRotationTime * Time.deltaTime);
        }


        if (Input.GetButtonDown("Jump"))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = mainCameraHolder.forward; // + leftWallHit.normal; //check if normalized vector3 or not works better
                Vector3 angledWallRunJumpDirection = mainCameraHolder.forward + (Quaternion.Euler(135, 0, 0) * leftWallHit.normal);
                //Vector3 forwardWallJumpVector = transform.forward + (leftWallHit.normal * forwardWallJumpVectorDistance);
                //Vector3 wallRunJumpReflectDirection = Vector3.Reflect(forwardWallJumpVector, wallRunJumpDirection);
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection* horizontalWallRunJumpForce * 100, ForceMode.Force);
                rigidBody.AddForce(angledWallRunJumpDirection * angledWallRunJumpForce * 100, ForceMode.Force);
                rigidBody.AddForce(transform.up * verticalWallRunJumpForce * 100, ForceMode.Force);
                //rigidBody.AddForce(-wallRunJumpReflectDirection * wallRunJumpForce * 100, ForceMode.Force);
                Debug.Log("wall jumped from left");
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = mainCameraHolder.forward; // + rightWallHit.normal; //check if normalized vector3 or not works better
                Vector3 angledWallRunJumpDirection = mainCameraHolder.forward + (Quaternion.Euler(45,0,0) * rightWallHit.normal);
                //Vector3 forwardWallJumpVector = (transform.forward + new Vector3(forwardWallJumpVectorDistance,0,forwardWallJumpVectorDistance) ) + (rightWallHit.normal);
                //float wallRunJumpReflectDirection = Vector3.Angle(forwardWallJumpVector, wallRunJumpDirection);
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection * horizontalWallRunJumpForce * 100, ForceMode.Force);
                rigidBody.AddForce(angledWallRunJumpDirection * angledWallRunJumpForce * 100, ForceMode.Force);
                rigidBody.AddForce(transform.up * verticalWallRunJumpForce * 100, ForceMode.Force);
                //rigidBody.AddForce(wallRunJumpDirection + new Vector3(wallRunJumpReflectDirection, 0, wallRunJumpReflectDirection) * wallRunJumpForce * 100, ForceMode.Force);
                Debug.Log("wall jumped from right");
            }
            isWallRunning = false;

        }


    }

    public void StopWallRun()
    {
        isWallRunning = false;
        rigidbodyPlayerMovement.GravityOn();
        NormalizeCameraEffects();
    }

    
    public void NormalizeCameraEffects()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFov, fovToWallRunFovTransitionTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, wallRunCameraTiltTime * Time.deltaTime);
        cameraRotation = Mathf.Lerp(cameraRotation, 0, wallRunCameraRotationTime * Time.deltaTime);
    }


}
