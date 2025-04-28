using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Greengrape greengrape;
    public Golf golf; 
    public float muzzleVelocity = 2;
    public float fireRate = 100;//开火间隔 时间
    float nextFireTime;//开火间隔
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void shoot() {         
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate / 1000;
            if(greengrape != null)
            {
                Greengrape newGreengrape = Instantiate(greengrape, muzzle.position, muzzle.rotation);
                newGreengrape.SetSpeed(muzzleVelocity);
            }
            else if(golf != null)
            {
                Golf newGolf = Instantiate(golf, muzzle.position, muzzle.rotation);
                newGolf.SetSpeed(muzzleVelocity);
            }
        }
    }
}
