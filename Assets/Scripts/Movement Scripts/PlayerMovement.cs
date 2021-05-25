using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    CharacterController characterController;

    [SerializeField]
    Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    float xMovement;
    float zMovement;

    public float playerSpeed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -9.8f;
    
    private float setSlopeLimit;

    Vector3 velocity;
    private Vector3 _netVelocity;

    public Vector3 NetVelocity
    {
        // getter/setter for CharacterControllerRewind script
        get { return _netVelocity; }
        set { _netVelocity = value; }
    } 

    private void Awake()
    {
        setSlopeLimit = characterController.slopeLimit;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(isGrounded && velocity.y < 0)
        {
            characterController.slopeLimit = setSlopeLimit;
            velocity.y = -2f;
        }

        xMovement = Input.GetAxisRaw("Horizontal");
        zMovement = Input.GetAxisRaw("Vertical");

        Vector3 movement = Vector3.Normalize(transform.right * xMovement + transform.forward * zMovement);

        characterController.Move(movement * playerSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            characterController.slopeLimit = 100.0f;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        _netVelocity = velocity * Time.deltaTime;

        characterController.Move(_netVelocity);
    }
}
