﻿using System.Collections;
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

    [SerializeField]
    MoveCamera moveCamera;

    public float groundDistance = 0.2f;
    public bool isGrounded;

    float xMovement;
    float zMovement;
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

    [Header("Air Movement")]
    public float playerJumpHeight = 3f;
    public float playerAirMultiplier = 0.4f;
    public float playerAirDrag = 1f;
    public float playerGravity = -20f;
    public bool canJump = true;

    [Header("Crouching and Sliding")]
    public float slideForce = 40f;
    public float slideDrag = 0.5f;
    [SerializeField]
    Vector3 crouchSize;
    [SerializeField]
    Vector3 standSize;
    [SerializeField]
    Vector3 crouchScale;
    Vector3 normalScale = new Vector3 (1, 1, 1);
    public bool crouchButtonPressed = false;
    public bool isCrouching = false;
    public bool isSliding = false;
    bool crouchCooldown = true;
    public float crouchFallDistance;
    public float aboveHeadDistance;
    public bool detectedItemAboveHeadDuringCrouch;

    Vector3 ToSetSize = Vector3.one;



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
        rigidBody.AddForce(Vector3.down * -playerGravity, ForceMode.Force);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        detectedItemAboveHeadDuringCrouch = Physics.CheckSphere(aboveHeadCheck.position, aboveHeadDistance, groundMask);

        xMovement = Input.GetAxisRaw("Horizontal");
        zMovement = Input.GetAxisRaw("Vertical");

        movementDirection = orientation.right * xMovement + orientation.forward * zMovement;

        slopeMovementDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);

        if (isSliding && (Input.GetKeyDown(KeyCode.W)) || (Input.GetKeyDown(KeyCode.S)) || (Input.GetKeyDown(KeyCode.D)) || (Input.GetKeyDown(KeyCode.A)))
        {
            StopSlide();
        }

        if (isSliding == true && rigidBody.velocity.magnitude < 0.5f)
        {
            StopSlide();
        }

        DragControl();
        SpeedControl();

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
            if(ToSetSize == crouchSize && !isSliding)
            {
                StopCrouch();
            }

            if(isSliding)
            {
                StopSlide();
                StopCrouch();
            }
            
        }

        crouchButtonPressed = Input.GetKey(KeyCode.C) && isGrounded && crouchCooldown;

        
        if(crouchButtonPressed && Input.GetButtonDown("Jump"))
        {
            StopCrouch();
            if (isSliding)
            {
                StopSlide();
            }
        }

        if(!Input.GetKey(KeyCode.C) && isCrouching && !detectedItemAboveHeadDuringCrouch)
        {
            transform.localScale = normalScale;
            isCrouching = false;
            canJump = true;
            canSprint = true;
        }
        

    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    //Basic Player Movement
    void SpeedControl()
    {
        if(Input.GetKey(KeyCode.LeftShift) && isGrounded && isMoving() && !crouchButtonPressed && !isSliding && canSprint)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, playerRunSpeed, playerAcceleration * Time.deltaTime);

        }
        else if ((crouchButtonPressed || isSliding) && isGrounded)
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
        if (isGrounded && !isSliding)
        {
            rigidBody.drag = playerGroundDrag;
        }
        else if(!isGrounded && !isSliding)
        {
            rigidBody.drag = playerAirDrag;
        }
        else if(isGrounded && isSliding)
        {
            rigidBody.drag = slideDrag;
        }
    }

    void PlayerMove()
    {
        if (isGrounded && !OnSlope() && !isSliding)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier * playerAirMultiplier, ForceMode.Acceleration);
        }
        else if(isGrounded && OnSlope() && !isSliding)
        {
            rigidBody.AddForce(slopeMovementDirection.normalized * playerSpeed * playerSpeedMultiplier, ForceMode.Acceleration);
        }
        
        
    }

    void PlayerJump()
    {
        if (isSliding)
        {
            StopSlide();
        }

        if (isGrounded && canJump)
        {
            //Debug.Log("everybody jump");
            //Vector3 jumpDirection = new Vector3 (mainCameraHolder.forward.x, transform.up.y, mainCameraHolder.forward.z);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
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
            if (!isSliding)
            {
                canSprint = false;
                canJump = false;
                ToSetSize = crouchSize;
                transform.position = new Vector3(transform.position.x, transform.position.y - crouchFallDistance/2, transform.position.z);
                transform.localScale = crouchScale;
                isCrouching = true;
                Debug.Log("crouched");
            }
            if (Input.GetKey(KeyCode.LeftShift) && !isSliding && !crouchButtonPressed)
            {
                StartSlide();
                return;
            }

            
        }
    }
    void StopCrouch()
    {
        
        //basic uncrouch
        if (isSliding || !crouchButtonPressed || !isGrounded || detectedItemAboveHeadDuringCrouch) 
        {
            return;
        }
        if (crouchButtonPressed)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (ToSetSize.y - transform.localScale.y), transform.position.z);
            ToSetSize = standSize;
            transform.localScale = normalScale;
            Debug.Log("uncrouched");
            canSprint = true;
            canJump = true;
            isCrouching = false;
        }
        crouchCooldown = false;
        StartCoroutine(resetSlideCooldown());
    }

    void StartSlide()
    {
        //StartingOurSlide by checking how much force to add (relative to current speed + basic add)
        float currentForce = rigidBody.velocity.magnitude * 0.45f;
        isSliding = true;
        if (OnSlope())
        {
            rigidBody.AddForce(slopeMovementDirection.normalized * slideForce * currentForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else if (!OnSlope())
        {
            rigidBody.AddForce(movementDirection.normalized * slideForce * currentForce * Time.deltaTime, ForceMode.Acceleration);
        }


    }

    void StopSlide()
    {

        isSliding = false;
        if (!crouchButtonPressed)
        {
            StopCrouch();
        }
        StartCoroutine(resetSlideCooldown());


        rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);

        //Stopping all of our movement when we stop the slide + uncrouching if we are uncrouched
    }

    private IEnumerator resetSlideCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        crouchCooldown = true;

        //Stopping crouch and slide spam
    }

    void SetSize()
    {
        if (!isSliding)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ToSetSize, 5 * Time.deltaTime);
        }
        if (isSliding)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ToSetSize, 8 * Time.deltaTime);
        }

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
