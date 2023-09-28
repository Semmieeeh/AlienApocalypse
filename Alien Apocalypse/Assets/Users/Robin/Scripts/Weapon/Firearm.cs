using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Firearm : Weapon
{
    [Space]
    [Header("General")]
    public FirearmData firearmData;
    public float damage;
    public float cooldown;

    [Space]
    [Header("Single Shot")]
    bool isSingleShoting;
    bool canSingleShoot = true;

    [Space]
    [Header("Burst")]
    public float burstAmount;
    bool isBursting;
    bool canBurst = true;

    [Space]
    [Header("Automatic")]
    public float fireRate;
    float timeSinceLastShot;

    [Space]
    [Header("Ammo")]
    public int maxAmmo;
    public int currentAmmo;

    [Space]
    [Header("Reloading")]
    public float reloadTime;
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
    [Header("Animation")]
    public Animator anim;

    [Space]
    [Header("Firearm Events")]
    [HideInInspector]
    public FirearmEvents events;

    public override void StartWeapon()
    {
        firearmCurrentPosition = firearmData.localPlacmentPos;
        SetWeaponData();
    }

    public override void UpdateWeapon()
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

    void SetWeaponData()
    {
        damage = firearmData.baseDamage;
        cooldown = firearmData.baseCooldown;

        burstAmount = firearmData.baseBurstAmount;
        fireRate = firearmData.baseFireRate;

        maxAmmo = firearmData.baseMaxAmmo;
        reloadTime = firearmData.anim.length;

        currentAmmo = maxAmmo;
    }

    public void ModifyWeaponData(float damage, float cooldown, int burstAmount, int fireRate, int maxAmmo, float reloadTime)
    {
        // Damage increase in Percentage
        this.damage *= 1 + (damage / 100);
        // Cooldown decrease in Percentage
        this.cooldown *= 1 - (cooldown / 100);
        // Burst Amount added by Adding
        this.burstAmount += burstAmount;
        // Fire Rate increased in Percentage
        this.fireRate *= 1 - (fireRate / 100);
        // Max Ammo increase by Adding
        this.maxAmmo += maxAmmo;
        // Reload Time decreased in Percentage
        this.reloadTime *= 1 - (reloadTime / 100);
    }

    bool CanShoot() => !isReloading && currentAmmo > 0;

    public override void Shooting()
    {
        if(CanShoot())
        {
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

        yield return new WaitForSeconds(cooldown);
        isSingleShoting = false;
    }

    bool CanShootBurst() => !isBursting && canBurst;

    IEnumerator BurstMode()
    {
        isBursting = true;
        canBurst = false;

        for(int i = 0; i < burstAmount; i++)
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

        yield return new WaitForSeconds(cooldown);
        isBursting = false;        
    }

    bool CanShootAutomatic() => Time.time > (1 / (fireRate / 60)) + timeSinceLastShot;

    void AutomaticMode()
    {
        // OnShooting will always be called if CanShoot is true and doesn't regard the FireType
        events.onShooting?.Invoke();

        // OnAutomatic will be called every time a projectile is fired; The FireType has to be Automatic
        events.onAutomatic?.Invoke();

        Recoil();
        Raycast();
 
        timeSinceLastShot = Time.time;        
    }

    public override IEnumerator Reloading()
    {
        events.onStartReloading?.Invoke();
        isReloading = true;

        if(anim != null)
        {
            anim.SetTrigger("Reload");
        }
        yield return new WaitForSeconds(reloadTime/2);
        currentAmmo = maxAmmo;
        yield return new WaitForSeconds(reloadTime / 2);

        isReloading = false;

        events.onEndReloading?.Invoke();
    }

    public override Vector3 Sway(Vector3 pos)
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
        if(anim != null)
        {
            anim.SetTrigger("Shoot");
        }

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