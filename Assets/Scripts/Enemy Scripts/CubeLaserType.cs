using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLaserType : CubeBase
{
    List<Laser> _lasers = new List<Laser>();

    private void Awake()
    {
        Initialize();
        _lasers = new List<Laser>(transform.GetComponentsInChildren<Laser>(false));
    }

    // Update is called once per frame
    void Update()
    {
        RotateCube();
        ShootLaser();
        RewindUpdate();
    }



    void ShootLaser()
    {
        if (!TimeController.Instance.IsRewinding)
        {
            foreach (Laser laser in _lasers)
            {
                laser.laserVector = (laser.transform.position - this.transform.position).normalized;
            }
        }
    }
}
