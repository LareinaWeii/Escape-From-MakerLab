using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Gun equippedGun;
    private Transform weaponHold;
    public Gun startingGun;
    public int type;

    void Start()
    {
        if (type == 0)
        {
            weaponHold = transform.GetChild(0);
        }
        else
            weaponHold = transform.GetChild(0).GetChild(0);
        EquipGun(startingGun);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.parent = weaponHold.transform;
    }

    public void Shoot()
    {
        if (equippedGun != null)
        {
            equippedGun.shoot();
        }
    }
}
