using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSector : MonoBehaviour
{
    [SerializeField] Color _gizmoColor;
    [SerializeField] LayerMask _enemyLayerMask;
    Collider _collider;
    Collider[] _enemyColliders;

    TurretBase _turretBase;
    CubeBulletType _cubeBulletType;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        //SectorOptimization.Instance.OnActiveSectorChange += ActiveSectorChanged;
    }

    private void OnDisable()
    {
        //SectorOptimization.Instance.OnActiveSectorChange -= ActiveSectorChanged;
    }

    private void Start()
    {
        SectorOptimization.Instance.OnActiveSectorChange += ActiveSectorChanged;
        _enemyColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation, _enemyLayerMask);
    }

    private void OnDrawGizmos()
    {
        Vector3 size = Vector3.one;
        Gizmos.color = _gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RigidbodyPlayerMovement>())
        {
            SectorOptimization.Instance.SetActiveSector(this);
        }
    }

    private void ActiveSectorChanged()
    {
        if (SectorOptimization.Instance.ActiveSector == this)
        {
            //activate enemies inside this sector
            Debug.Log("Activating enemies at: "+gameObject.name);
            EnemyActivation(true);
        }
        else
        {
            //deactivate enemies inside this sector
            Debug.Log("Deactivating enemies at: " + gameObject.name);
            EnemyActivation(false);
        }
    }

    private void EnemyActivation(bool activate)
    {
        foreach (var col in _enemyColliders)
        {
            if(col.GetComponent<TurretBase>() != null)
            {
                col.GetComponent<TurretBase>().AtActiveSector = activate;
            }
            else if(col.GetComponent<CubeBulletType>() != null)
            {
                col.GetComponent<CubeBulletType>().AtActiveSector = activate;
            }
        }
    }
}
