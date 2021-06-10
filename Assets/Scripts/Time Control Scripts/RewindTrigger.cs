using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RigidbodyPlayerMovement>())
        {
            TimeController.Instance.Rewind();
        }
    }
}
