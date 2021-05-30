using UnityEngine;

public class RigidbodyPlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidBody;

    [SerializeField]
    Transform orientation;

    [SerializeField]
    LayerMask groundMask;
    public float groundDistance = 0.4f;
    bool isGrounded;
    

    float playerHeight;

    float xMovement;
    float zMovement;


    Vector3 movementDirection;

    public float playerSpeed = 12f;
    public float playerSpeedMultiplier = 10f;
    public float playerAirMultiplier = 0.4f; 

    public float playerJumpHeight = 3f;
    public float playerGroundDrag = 12f;
    public float playerAirDrag = 3f;



    // Start is called before the first frame update
    void Start()
    {
        rigidBody.freezeRotation = true;
        playerHeight = GetComponent<CapsuleCollider>().height;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);

        xMovement = Input.GetAxisRaw("Horizontal");
        zMovement = Input.GetAxisRaw("Vertical");

        movementDirection = orientation.right * xMovement + orientation.forward * zMovement;

        DragControl();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerJump();
        }

    }

    private void FixedUpdate()
    {
        PlayerMove();
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
        if (isGrounded)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rigidBody.AddForce(movementDirection.normalized * playerSpeed * playerSpeedMultiplier * playerAirMultiplier, ForceMode.Acceleration);
        }
        
    }

    void playerJump()
    {
        rigidBody.AddForce(transform.up * playerJumpHeight, ForceMode.Impulse);
    }
}
