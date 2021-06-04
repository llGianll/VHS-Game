using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyWallrun : MonoBehaviour
{
    [SerializeField]
    Transform orientation;

    public float wallDistance;
    public float minimumJumpHeight;

    [SerializeField]
    private float wallRunGravity;
    [SerializeField]
    private float wallRunJumpForce;

    bool wallLeft = false;
    bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rigidBody;

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        WallCheck();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
                Debug.Log("wall hit left");
            }
            else if(wallRight)
            {
                StartWallRun();
                Debug.Log("wall hit right");
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void WallCheck()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    void StartWallRun()
    {
        rigidBody.useGravity = false;

        rigidBody.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if (Input.GetButtonDown("Jump"))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal; //check if normalize or not works better
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal; //check if normalize or not works better
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
                rigidBody.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }

    void StopWallRun()
    {
        rigidBody.useGravity = true;
    }
}
