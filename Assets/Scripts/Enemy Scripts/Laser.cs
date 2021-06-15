using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lineR;
    public Vector3 laserVector;

    void Start()
    {
        lineR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        lineR.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, laserVector, out hit))
        {
            if (hit.collider)
            {
                lineR.SetPosition(1, hit.point);
            }
            if (hit.transform.gameObject.GetComponent<RigidbodyPlayerMovement>())
                TimeController.Instance.Rewind();
        }
        else lineR.SetPosition(1, laserVector * 100);
    }
}
