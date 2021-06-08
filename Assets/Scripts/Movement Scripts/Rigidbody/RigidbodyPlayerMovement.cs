﻿using UnityEngine;

public class RigidbodyPlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidBody;

    [SerializeField]
    Transform orientation;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    LayerMask groundMask;
    public float groundDistance = 0.2f;
    public bool isGrounded;

    Vector3 movementDirection;
    Vector3 slopeMovementDirection;

    public float playerSpeed = 12f;
    public float playerWalkSpeed = 6f;
    public float playerRunSpeed = 12f;
    public float playerAcceleration = 10f;

    public float playerSpeedMultiplier = 10f;
    public float playerAirMultiplier = 0.4f;

    public float playerJumpHeight = 3f;
    public float playerGroundDrag = 12f;
    public float playerAirDrag = 1f;

    public float playerGravity = -20f;

    RaycastHit slopeHit;

    Transform mainCameraHolder;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    float playerHeight;

    float xMovement;
    float zMovement;

    float storedGravityVal;




    // Start is called before the first frame update
    void Start()
    {
        rigidBody.freezeRotation = true;
        playerHeight = GetComponent<CapsuleCollider>().height;
        rigidBody.useGravity = false;
        storedGravityVal = playerGravity;
        mainCameraHolder = GameObject.Find("RigidbodyCameraHolder").transform;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.AddForce(Vector3.down * -playerGravity, ForceMode.Force);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        xMovement = Input.GetAxisRaw("Horizontal");
        zMovement = Input.GetAxisRaw("Vertical");

        movementDirection = orientation.right * xMovement + orientation.forward * zMovement;

        DragControl();
        SpeedControl();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            PlayerJump();
        }

        slopeMovementDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);

    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    void SpeedControl()
    {
        if(Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, playerRunSpeed, playerAcceleration * Time.deltaTime);
        }
        else
        {
            playerSpeed = Mathf.Lerp(playerSpeed, playerWalkSpeed, playerAcceleration * Time.deltaTime);
        }
    }

    void DragControl()
    {
        if (isGrounded)
        {
            rigidBody.drag = playerGroundDrag;
        }
        else
        {
            rigidBody.drag = playerAirDrag;
        }
    }

    void PlayerMove()
    {
        if (isGrounded && !OnSlope())
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier, ForceMode.Acceleration);
        }
        else if(isGrounded && OnSlope())
        {
            rigidBody.AddForce(slopeMovementDirection.normalized * playerSpeed * playerSpeedMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier * playerAirMultiplier, ForceMode.Acceleration);
        }
        
    }

    void PlayerJump()
    {
        if (isGrounded)
        {
            //Debug.Log("everybody jump");
            //Vector3 jumpDirection = new Vector3 (mainCameraHolder.forward.x, transform.up.y, mainCameraHolder.forward.z);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            //rigidBody.AddForce(jumpDirection * playerJumpHeight, ForceMode.Impulse);
            rigidBody.AddForce(transform.up * playerJumpHeight, ForceMode.Impulse);
        }
    }

    public void GravityOff()
    {
        playerGravity = 0f;
    }
    public void GravityOn()
    {
        playerGravity = storedGravityVal;
    }
}
