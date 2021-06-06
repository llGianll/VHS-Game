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



    [SerializeField]
    private float wallRunGravity;
    [SerializeField]
    private float wallRunJumpForce;
    [SerializeField]
    private float wallRunTimer = 2f;

    bool wallLeft = false;
    bool wallRight = false;
    bool wallBack = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    RaycastHit backWallHit;

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
        //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), mainCameraHolder.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (!rigidbodyPlayerMovement.isGrounded)
        {
            WallCheck();

            if (CanWallRun())
            {
                if (wallLeft || wallRight || wallBack)
                {
                    StartWallRun();
                    Debug.Log("wall hit");
                }
                else
                {
                    StopWallRun();
                }
            }
        }

    }

    void WallCheck()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
        wallBack = Physics.Raycast(transform.position, -orientation.forward, out backWallHit, wallDistance);

    }

    void StartWallRun()
    {
        rigidbodyPlayerMovement.GravityOff();

        rigidBody.AddForce(Vector3.down * -wallRunGravity, ForceMode.Force);


        StartCoroutine("WallStayCountdown");

        if (Input.GetButtonDown("Jump"))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = mainCameraHolder.forward; //+ leftWallHit.normal; //check if normalized vector3 or not works better
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
                Debug.Log("wall jumped from left");
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = mainCameraHolder.forward; //+ rightWallHit.normal; //check if normalized vector3 or not works better
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
                Debug.Log("wall jumped from right");
            }
            else if (wallBack)
            {
                Vector3 wallRunJumpDirection = mainCameraHolder.forward; //+ backWallHit.normal; //check if normalized vector3 or not works better
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
                Debug.Log("wall jumped from back");
            }
            //StopCoroutine("WallStayCountdown");
        }


    }

    void StopWallRun()
    {
        rigidbodyPlayerMovement.GravityOn();
        
    }

    void PlayerFall()
    {
        StopWallRun();
        Debug.Log("wall stay coroutine stopped");
        

        if (!rigidbodyPlayerMovement.isGrounded) {
            rigidBody.AddForce(Vector3.down * rigidBody.position.y, ForceMode.Impulse);
            Debug.Log("they all fall down");
        }
        else
        {
            StopCoroutine("WallStayCountdown");
        }
    }
    

    private IEnumerator WallStayCountdown()
    {
        Debug.Log("wall timer started");
            yield return new WaitForSeconds(wallRunTimer);
            Debug.Log("wall timer up");
            PlayerFall();
    }
}
