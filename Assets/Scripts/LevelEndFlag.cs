using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndFlag : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RigidbodyPlayerMovement>())
        {
            GameManager.Instance.ReachedEndOfLevel();
        }
    }
}
