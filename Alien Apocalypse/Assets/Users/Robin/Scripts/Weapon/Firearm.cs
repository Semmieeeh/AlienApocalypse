using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class Firearm : Weapon
{
    [Space]
    [Header("Type")]
    public FirearmType firearmType;
    public Firetype fireType;
    public enum Firetype
    {
        singleShot,
        burst,
        automatic,
    }

    [Space]
    [Header("Shooting")]
    public float baseDamage;

    [Space]
    [Header("Single Shot")]
    public float baseSingleShotCooldown;
    bool isSingleShoting;
    bool canSingleShoot = true;

    [Space]
    [Header("Burst")]
    public int baseBurstAmount;
    public float baseTimeBetweenBurst;
    public float baseBurstCooldown;
    bool isBursting;
    bool canBurst = true;

    [Space]
    [Header("Automatic")]
    public float baseFireRate;
    float baseTimeSinceLastShot;

    [Space]
    [Header("Ammo")]
    public int currentAmmo;
    public int baseMaxAmmo;
    public int totalAmmo;

    [Space]
    [Header("Reloading")]
    public float baseReloadTime;
    bool isReloading;

    //[Space]
    //[Header("Projectile")]
    //public GameObject projectilePrefab;
    //public Transform muzzle;
    //public float baseProjectileSpeed;

    [Space]
    [Header("Raycast")]
    public float raycastDistance;
    //public Vector3 raycastHitPoint;
    RaycastHit hit;

    [Space]
    [Header("Sway")]
    public float swayClamp;
    public float smoothing;

    [Space]
    [Header("Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float snappiness;
    public float returnSpeed;
    Vector3 currentRotation;
    Vector3 targetRotation;

    [Space]
    [Header("Firearm Events")]
    public FireArmEvents events;

    public override void StartWeapon()
    {
        
    }

    public override void UpdateWeapon()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);

        recoil.transform.localRotation = Quaternion.Euler(currentRotation);
    }

    bool CanShoot() => !isReloading && currentAmmo > 0;

    public override void Shooting()
    {
        if(CanShoot())
        {            
            //raycastHitPoint = hit.point;

            switch(fireType)
            {
                case Firetype.singleShot:
                {
                    if(CanShootSingleShot())
                        StartCoroutine(SingleShot());
                    break;
                }
                case Firetype.burst:
                {
                    if(CanShootBurst())
                        StartCoroutine(BurstMode());
                    break;
                }
                case Firetype.automatic:
                {
                    if(CanShootAutomatic())
                        AutomaticMode();
                    break;
                }
            }
        }
    }

    public override void OnButtonUp()
    {
        canBurst = true;
        canSingleShoot = true;
    }

    bool CanShootSingleShot() => !isSingleShoting && canSingleShoot;

    IEnumerator SingleShot()
    {
        isSingleShoting = true;
        canSingleShoot = false;

        // OnShooting will always be called if CanShoot is true and doesn't regard the FireType
        events.onShooting?.Invoke();

        // OnSingleShot will be called every time a projectile is fired; FireType has to be SingelShot
        events.onSingleShot?.Invoke();

        Recoil();
        Raycast();

        yield return new WaitForSeconds(baseSingleShotCooldown);
        isSingleShoting = false;
    }

    bool CanShootBurst() => !isBursting && canBurst;


    IEnumerator BurstMode()
    {
        isBursting = true;
        canBurst = false;

        for(int i = 0; i < baseBurstAmount; i++)
        {
            if(currentAmmo <= 0)
                break;

            // OnShooting will always be called if CanShoot is true and doesn't regard the FireType
            events.onShooting?.Invoke();

            // OnBurst will be called every time a projectile is fired; FireType has to be Burst
            events.onBurst?.Invoke();

            Recoil();
            Raycast();

            yield return new WaitForSeconds(baseTimeBetweenBurst);
        }

        yield return new WaitForSeconds(baseBurstCooldown);
        isBursting = false;        
    }

    bool CanShootAutomatic() => Time.time > (1 / (baseFireRate / 60)) + baseTimeSinceLastShot;

    void AutomaticMode()
    {
        // OnShooting will always be called if CanShoot is true and doesn't regard the FireType
        events.onShooting?.Invoke();

        // OnAutomatic will be called every time a projectile is fired; The FireType has to be Automatic
        events.onAutomatic?.Invoke();

        Recoil();
        Raycast();

        baseTimeSinceLastShot = Time.time;        
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

    public override void Sway(Vector2 mouseInput)
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        input.x = Mathf.Clamp(input.x, -swayClamp, swayClamp);
        input.y = Mathf.Clamp(input.y, -swayClamp, swayClamp);

        Vector3 target = new Vector3(input.x, input.y, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, target + localPlacmentPos, Time.deltaTime * smoothing);
    }

    void Recoil()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void Raycast()
    {
        if(Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, raycastDistance))
        {
            if(hit.transform.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damagable(baseDamage, events.onKillEnemy, events.onHitEnemy);               
            }
        }

        currentAmmo--;
    }
}

[System.Serializable]
public class FireArmEvents
{
    public UnityEvent onShooting;
    public UnityEvent onSingleShot;
    public UnityEvent onBurst;
    public UnityEvent onAutomatic;
    public UnityEvent onHitEnemy;
    public UnityEvent onKillEnemy;
    public UnityEvent onStartReloading;
    public UnityEvent onEndReloading;
}
