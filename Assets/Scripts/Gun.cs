using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform muzzlePoint;
    public Projectile projectile;
    public float fireRate = 100; // in ms
    public float muzzleVelocity = 35;

    float nextShotTime;

    public void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + fireRate / 1000;

            Projectile bullet = Instantiate(projectile, muzzlePoint.position, muzzlePoint.rotation) as Projectile;
            bullet.SetSpeed(muzzleVelocity);
        }
    }


}
