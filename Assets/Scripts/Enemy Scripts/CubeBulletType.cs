using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBulletType : CubeBase
{
    [SerializeField] float _fireRate = 0.5f;
    [SerializeField] float _projectileSpeed = 5f;
    [SerializeField] GameObject _bullet;

    public bool AtActiveSector { get; set; }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        InitializeStart();
    }

    // Update is called once per frame
    void Update()
    {
        RotateCube();
        this.hasfired = this.currentTime >= _fireRate ? true : false; //check has fired even with optimization code 
        if (SectorOptimization.Instance == null)
        {
            //for enemies to still function while the optimization is not applied to the level
            Shooting();
        }
        else if (SectorOptimization.Instance != null)
        {
            //if optimization is already applied to the level and player is at the same sector as the enemy 
            if (AtActiveSector)
                Shooting();
        }
        RewindUpdate();
    }

    void Shooting()
    {
        if (!TimeController.Instance.IsRewinding)
        {
            if (hasfired)
            {
                foreach (Transform barrelT in bulletSpawnPoints)
                {
                    GameObject bullet = PooledObjectManager.Instance.GetPooledObject("Bullet");
                    bullet.transform.position = barrelT.position;
                    bullet.GetComponent<Rigidbody>().velocity = (barrelT.position - this.transform.position).normalized * _projectileSpeed;
                    bullet.SetActive(true);
                }
                currentTime = 0;
            }
        }
    }
}
