using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    float _moveSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movementVector = new Vector3(horizontal, 0, vertical);


        if (!TimeController.Instance.IsRewinding)
            transform.position += movementVector * _moveSpeed * Time.deltaTime;
    }
}
