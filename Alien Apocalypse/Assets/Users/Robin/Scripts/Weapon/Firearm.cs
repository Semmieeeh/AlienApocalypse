using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Firearm : Weapon
{
    public FirearmData firearmData;

    [Space]
    [Header("Shooting")]

    [Space]
    [Header("Single Shot")]
    bool isSingleShoting;
    bool canSingleShoot = true;

    [Space]
    [Header("Burst")]

    bool isBursting;
    bool canBurst = true;

    [Space]
    float baseTimeSinceLastShot;

    [Space]
    [Header("Ammo")]
    public int currentAmmo;
    public int totalAmmo;

    [Space]
    [Header("Reloading")]
    bool isReloading;

    [Space]
    [Header("Raycast")]
    RaycastHit hit;

    [Space]
    [Header("Recoil Camera Rotation")]
    Vector3 camCurrentRotation;
    Vector3 camTargetRotation;

    [Header("Recoil Firearm Rotation")]
    Vector3 firearmCurrentRotation;
    Vector3 firearmTargetRotation;

    [Header("Recoil Firearm Position")]
    Vector3 firearmCurrentPosition;
    Vector3 firearmTargetPosition;

    [Space]
    [Header("Firearm Events")]
    public FireArmEvents events;

    public override void StartWeapon()
    {
        firearmCurrentPosition = firearmData.localPlacmentPos;
    }

    public override void UpdateWeapon(Vector2 mouseInput)
    {
        camTargetRotation = Vector3.Lerp(camTargetRotation, Vector3.zero, firearmData.camReturnSpeed * Time.deltaTime);
        camCurrentRotation = Vector3.Slerp(camCurrentRotation, camTargetRotation, firearmData.camSnappiness * Time.fixedDeltaTime);

        firearmTargetRotation = Vector3.Lerp(firearmTargetRotation, Vector3.zero, firearmData.firearmReturnSpeed * Time.deltaTime);
        firearmCurrentRotation = Vector3.Lerp(firearmCurrentRotation, firearmTargetRotation, firearmData.firearmSnappiness * Time.fixedDeltaTime);

        firearmTargetPosition = Vector3.Lerp(firearmTargetPosition, firearmData.localPlacmentPos, firearmData.backUpReturnSpeed * Time.deltaTime);
        firearmCurrentPosition = Vector3.Lerp(firearmCurrentPosition, firearmTargetPosition, firearmData.backUpSnappiness * Time.deltaTime);

        recoilObject.transform.localRotation = Quaternion.Euler(camCurrentRotation);
        transform.localRotation = Quaternion.Euler(firearmCurrentRotation);
        transform.localPosition = firearmCurrentPosition;
    }

    bool CanShoot() => !isReloading && currentAmmo > 0;

    public override void Shooting()
    {
        if(CanShoot())
        {            
            //raycastHitPoint = hit.point;

            switch(firearmData.fireType)
            {
                case FirearmData.Firetype.singleShot:
                {
                    if(CanShootSingleShot())
                        StartCoroutine(SingleShot());
                    break;
                }
                case FirearmData.Firetype.burst:
                {
                    if(CanShootBurst())
                        StartCoroutine(BurstMode());
                    break;
                }
                case FirearmData.Firetype.automatic:
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

        yield return new WaitForSeconds(firearmData.baseSingleShotCooldown);
        isSingleShoting = false;
    }

    bool CanShootBurst() => !isBursting && canBurst;


    IEnumerator BurstMode()
    {
        isBursting = true;
        canBurst = false;

        for(int i = 0; i < firearmData.baseBurstAmount; i++)
        {
            if(currentAmmo <= 0)
                break;

            // OnShooting will always be called if CanShoot is true and doesn't regard the FireType
            events.onShooting?.Invoke();

            // OnBurst will be called every time a projectile is fired; FireType has to be Burst
            events.onBurst?.Invoke();

            Recoil();
            Raycast();

            yield return new WaitForSeconds(firearmData.baseTimeBetweenBurst);
        }

        yield return new WaitForSeconds(firearmData.baseBurstCooldown);
        isBursting = false;        
    }

    bool CanShootAutomatic() => Time.time > (1 / (firearmData.baseFireRate / 60)) + baseTimeSinceLastShot;

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

        yield return new WaitForSeconds(firearmData.baseReloadTime);

        currentAmmo = firearmData.baseMaxAmmo;
        isReloading = false;

        events.onEndReloading?.Invoke();
    }

    public override Vector3 Sway(Vector2 mouseInput, Vector3 pos)
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        input.x = Mathf.Clamp(input.x, -firearmData.swayClamp, firearmData.swayClamp);
        input.y = Mathf.Clamp(input.y, -firearmData.swayClamp, firearmData.swayClamp);

        Vector3 target = new Vector3(input.x, input.y, 0);

        Vector3 newPos = Vector3.Lerp(pos, target + Vector3.zero, Time.deltaTime * firearmData.smoothing);

        return newPos;
    }

    void Recoil()
    {
        camTargetRotation += new Vector3(firearmData.camRecoilX, Random.Range(-firearmData.camRecoilY, firearmData.camRecoilY), Random.Range(-firearmData.camRecoilZ, firearmData.camRecoilZ));
        firearmTargetRotation += new Vector3(firearmData.firearmRecoilX, Random.Range(-firearmData.firearmRecoilY, firearmData.firearmRecoilY), Random.Range(-firearmData.firearmRecoilZ, 0));
        firearmTargetPosition = new Vector3(firearmData.localPlacmentPos.x, firearmData.localPlacmentPos.y, firearmData.firearmRecoilBackUp + firearmData.localPlacmentPos.z);
    }

    void Raycast()
    {
        if(Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, firearmData.raycastDistance))
        {
            if(hit.transform.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damagable(firearmData.baseDamage, events.onKillEnemy, events.onHitEnemy);               
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
