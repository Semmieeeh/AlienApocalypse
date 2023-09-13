using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Firearm : Weapon
{
    [Space]
    [Header("Type")]
    public FirearmType firearmType;

    [Space]
    [Header("Shooting")]
    public float baseDamage;
    public float baseFireRate;
    public bool isAutomatic;
    public bool isBurst;
    float timeSinceLastShot;

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
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform muzzle;
    public float baseProjectileSpeed;

    [Space]
    [Header("Raycast")]
    public Transform playerCamera;
    public float raycastDistance;
    public Vector3 raycastHitPoint;
    RaycastHit hit;

    [Space]
    [Header("Firearm Events")]
    public FireArmEvents events;


    bool CanShoot() => !isReloading && currentAmmo > 0 && Time.time > (1 / (baseFireRate / 60)) + timeSinceLastShot;

    public override void Shooting()
    {
        if(CanShoot())
        {
            events.onShooting?.Invoke();

            if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, raycastDistance))
                raycastHitPoint = hit.point;
            else
                raycastHitPoint = playerCamera.position + playerCamera.forward * raycastDistance;

            //Debug.Log(raycastHitPoint);

            GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

            if(projectile.TryGetComponent<Projectile>(out Projectile projectileScript))
            {
                projectileScript.InitializeProjectile(baseDamage, baseProjectileSpeed, projectile.transform.position, raycastHitPoint);
                projectileScript.onHit = events.onHitEnemy;
            }

            currentAmmo--;
            timeSinceLastShot = Time.time;
        }
    }

    public override IEnumerator Reloading()
    {
        events.onStartReloading?.Invoke();

        isReloading = true;

        yield return new WaitForSeconds(baseReloadTime);

        currentAmmo = baseMaxAmmo;
        isReloading = false;

        events.onEndReloading?.Invoke();
    }
}

[System.Serializable]
public class FireArmEvents
{
    public UnityEvent onShooting;
    public UnityEvent onHitEnemy;
    public UnityEvent onKillEnemy;
    public UnityEvent onStartReloading;
    public UnityEvent onEndReloading;
}
