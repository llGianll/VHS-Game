using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyPlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidBody;

    [SerializeField]
    Transform orientation;

    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    Transform aboveHeadCheck;

    [SerializeField]
    LayerMask groundMask;


    public float groundDistance = 0.2f;
    public bool isGrounded;

    public float xMovement;
    public float zMovement;
    Vector3 movementDirection;
    Vector3 slopeMovementDirection;

    [Header("Ground Movement")]
    public float playerSpeed = 12f;
    public float playerWalkSpeed = 6f;
    public float playerRunSpeed = 12f;
    public float playerCrouchSpeed = 3f;
    public float playerAcceleration = 10f;
    public float playerSpeedMultiplier = 10f;
    public float playerGroundDrag = 12f;
    public bool canSprint = true;
    public bool isRunning = false;

    [Header("Air Movement")]
    public float playerJumpHeight = 3f;
    public float playerAirMultiplier = 0.4f;
    public float playerAirDrag = 1f;
    public float playerGravity = -20f;
    public float playerJumpVelocityLimiter = 2f;
    public bool canJump = true;

    [Header("Crouching and Sliding")]
    public float slideForce = 40f;
    public float slideDrag = 0.5f;
    public float runningSlideForce = 60f;
    [SerializeField]
    Vector3 crouchSize;
    [SerializeField]
    Vector3 standSize;
    [SerializeField]
    Vector3 crouchScale;
    Vector3 normalScale = new Vector3 (1, 1, 1);
    public bool crouchButtonPressed = false;
    public bool isCrouching = false;
    bool crouchCooldown = true;
    public float crouchFallDistance;
    public float aboveHeadDistance;
    public bool detectedItemAboveHeadDuringCrouch;

    Vector3 ToSetSize = Vector3.one;

    //[PSEUDO FOOTSTEPS IMPLEMENTATION]
    float _currentStepTime = 0;
    SFXPresets _currentStepSFX = SFXPresets.Footstep_1;

    RaycastHit slopeHit;

    Transform mainCameraHolder;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
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

    bool isMoving()
    {
        if (xMovement == 0 && zMovement == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    float playerHeight;

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

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        detectedItemAboveHeadDuringCrouch = Physics.CheckSphere(aboveHeadCheck.position, aboveHeadDistance, groundMask);

        PlayerInput();

        DragControl();
        SpeedControl();
        FootStepSFX();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            PlayerJump();
        }

        if (Input.GetKeyDown(KeyCode.C) && isGrounded)
        {
            StartCrouch();
        }


        if (Input.GetKeyUp(KeyCode.C) && isGrounded)
        {
            if(ToSetSize == crouchSize)
            {
                StopCrouch();
            }
            
        }

        crouchButtonPressed = Input.GetKey(KeyCode.C) && isGrounded && crouchCooldown;

        
        if(crouchButtonPressed && Input.GetButtonDown("Jump"))
        {
            StopCrouch();
        }

        if(!Input.GetKey(KeyCode.C) && isCrouching && !detectedItemAboveHeadDuringCrouch)
        {
            //to automatically leave crouch as soon as there's nothing above without pressing c again 
            transform.localScale = normalScale;
            isCrouching = false;
            canJump = true;
            canSprint = true;
        }
        

    }

    private void FootStepSFX()
    {
        //[PSEUDO FOOTSTEPS IMPLEMENTATION]

        if (!isMoving() || !isGrounded)
        {
            _currentStepTime = 0;
            return;
        }

        float stepInterval = (isRunning) ? 0.25f : 0.4f;

        if(_currentStepTime == 0)
        {
            SFXPlayer.Instance.Play(_currentStepSFX);
            _currentStepSFX = (_currentStepSFX == SFXPresets.Footstep_1) ? SFXPresets.Footstep_2 : SFXPresets.Footstep_1;
        }

        _currentStepTime += Time.deltaTime;

        if (_currentStepTime >= stepInterval)
        {
            _currentStepTime = 0;
        }

    }

    private void FixedUpdate()
    {
        rigidBody.AddForce(Vector3.down * -playerGravity, ForceMode.Force);
        PlayerMove();
    }

    //Basic Player Movement

    void PlayerInput()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        zMovement = Input.GetAxisRaw("Vertical");

        movementDirection = orientation.forward * zMovement + orientation.right * xMovement;
    }
    void SpeedControl()
    {
        if(Input.GetKey(KeyCode.LeftShift) && isGrounded && isMoving() && !crouchButtonPressed && canSprint)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, playerRunSpeed, playerAcceleration * Time.deltaTime);
            isRunning = true;

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
        else if (crouchButtonPressed && isGrounded)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, playerCrouchSpeed, playerAcceleration * Time.deltaTime);

        }
        else if(!Input.GetKey(KeyCode.LeftShift) && isGrounded && !crouchButtonPressed)
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
        else if(!isGrounded)
        {
            rigidBody.drag = playerAirDrag;
        }

    }

    void PlayerMove()
    {
        if (isGrounded)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier * playerAirMultiplier, ForceMode.Acceleration);
        }
        
        
    }

    void PlayerJump()
    {
        if (isGrounded && canJump)
        {
            Debug.Log("everybody jump");
            SFXPlayer.Instance.Play(SFXPresets.Jump);
            //Vector3 jumpDirection = new Vector3 (mainCameraHolder.forward.x, transform.up.y, mainCameraHolder.forward.z);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x / playerJumpVelocityLimiter, 0 , rigidBody.velocity.z / playerJumpVelocityLimiter);
            //rigidBody.angularVelocity = new Vector3(rigidBody.velocity.x / playerJumpVelocityLimiter, 0, rigidBody.velocity.z / playerJumpVelocityLimiter);
            //rigidBody.AddForce(jumpDirection * playerJumpHeight, ForceMode.Impulse);
            rigidBody.AddForce(transform.up * playerJumpHeight, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        var layer = other.gameObject.layer;
        if (groundMask != (groundMask | (1 << layer))) return;

        Vector3 normal = other.GetContact(0).normal;

        if (Vector3.Angle(Vector3.up, normal) < 45)
        {
            isGrounded = true;
        }

        //getting the normal of any object we collide with
    }

    //Crouch and Sliding
    void StartCrouch()
    {
        //basic crouch
        if (crouchCooldown && !detectedItemAboveHeadDuringCrouch)
        {
            if (crouchButtonPressed || !isGrounded)
            {
                return;
            }

                canSprint = false;
                canJump = false;
                ToSetSize = crouchSize;
                transform.position = new Vector3(transform.position.x, transform.position.y - crouchFallDistance/2, transform.position.z);
                transform.localScale = crouchScale;
                isCrouching = true;
                if (isRunning)
                {
                    float currentForce = rigidBody.velocity.magnitude * 0.45f;
                    rigidBody.AddForce(movementDirection.normalized * runningSlideForce * currentForce * Time.deltaTime, ForceMode.Impulse);
                    Debug.Log("boom we slid");
                }

            
        }
    }
    void StopCrouch()
    {
        
        //basic uncrouch
        if (!crouchButtonPressed || !isGrounded || detectedItemAboveHeadDuringCrouch) 
        {
            return;
        }
        if (crouchButtonPressed)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (ToSetSize.y - transform.localScale.y), transform.position.z);
            ToSetSize = standSize;
            transform.localScale = normalScale;
            //Debug.Log("uncrouched");
            canSprint = true;
            canJump = true;
            isCrouching = false;
        }
        crouchCooldown = false;
        StartCoroutine(resetSlideCooldown());
    }


    private IEnumerator resetSlideCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        crouchCooldown = true;

        //Stopping crouch and slide spam
    }


    //Gravity
    public void GravityOff()
    {
        playerGravity = 0f;
    }
    public void GravityOn()
    {
        playerGravity = storedGravityVal;
    }

}
