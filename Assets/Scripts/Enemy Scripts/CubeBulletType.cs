using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBulletType : CubeBase
{
    [SerializeField] float _fireRate = 0.5f;
    [SerializeField] float _projectileSpeed = 5f;
    [SerializeField] GameObject _bullet;

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
        Shooting();
        RewindUpdate();
    }

    void Shooting()
    {
        this.hasfired = this.currentTime >= _fireRate ? true : false;
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
