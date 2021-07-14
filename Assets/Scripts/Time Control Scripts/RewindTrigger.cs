using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTrigger : MonoBehaviour
{
    [SerializeField] Color _gizmoColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RigidbodyPlayerMovement>())
        {
            TimeController.Instance.Rewind();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 size = Vector3.one;
        Gizmos.color = _gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
