using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizeCameraEffectsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RigidbodyPlayerMovement>())
        {

            other.gameObject.GetComponent<RigidbodyWallrun>().NormalizeCameraEffects();
            Debug.Log("wall run stop trigger activated");
        }
    }
}
