using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : Weapon
{
    [Space]
    [Header("Shooting")]
    public float baseDamage;
    public float baseFireRate;
    public bool isAutomatic;
    public bool isBurst;
    float timeSinceLastShot = 0;

    [Space]
    [Header("Ammo")]
    public int currentAmmo;
    public int baseMaxAmmo;
    public int totalAmmo;

    [Space]
    [Header("Reloading")]
    public float baseReloadTime;
    public bool isReloading;

    [Space]
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float baseBulletSpeed;

    //[Space]
    //[Header("Modified Values")]

    bool CanShoot() => !isReloading && Time.time > (1 / (baseFireRate / 60)) + timeSinceLastShot;

    public override void Shooting()
    {
        if(CanShoot())
        {
            Debug.Log("Shooting");
            timeSinceLastShot = Time.time;
        }
    }

    public override void Reloading()
    {
        
    }
}
