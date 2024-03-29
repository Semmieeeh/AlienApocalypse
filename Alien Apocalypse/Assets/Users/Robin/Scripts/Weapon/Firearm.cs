using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Firearm : Weapon
{
    [Space]
    [Header("General")]
    public FirearmData firearmData;
    public DataHolder dataHolder;
    public bool isShooting;
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
    [Header("Projectile")]
    bool isProjectile;
    bool canProjectile = true;
    Vector3 raycastHitPoint;

    [Space]
    [Header("Shotgun")]
    public bool isShotgun;
    public bool canShotgun = true;

    [Space]
    [Header("Ammo")]
    public int maxAmmo;
    public int currentAmmo;

    [Space]
    [Header("Reloading")]
    public float reloadTime;
    public bool isReloading;

    [Space]
    [Header("Raycast")]
    RaycastHit hit;
    float bulletForce;

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
    public Animator animator;
    public int weaponInt;

    [Space]
    [Header("Audio")]
    public AudioSource source;

    [Space]
    [Header("Firearm Events")]
    public FirearmEvents events;

    public override void StartWeapon()
    {
        firearmCurrentPosition = firearmData.localPlacmentPos;
        transform.GetChild(0).GetComponent<Children>().enabled = true;
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
            weaponInt = firearmData.weaponInt;
            burstAmount = firearmData.baseBurstAmount;
            fireRate = firearmData.baseFireRate;
            maxAmmo = firearmData.baseMaxAmmo;
            bulletForce = firearmData.bulletForce;
            if(firearmData.anim != null)
            {
                reloadTime = firearmData.anim.length;
            }
            source.spatialBlend = 0;
            currentAmmo = maxAmmo;
            AmmoCounter.Instance.SetWeaponData(firearmData, this);
    }

    public void ModifyWeaponData(float damage, float cooldown, int burstAmount, float fireRate, int maxAmmo, float reloadTime)
    {
        // Damage increase in Percentage
        this.damage *= 1 + (damage);
        // Cooldown decrease in Percentage
        this.cooldown *= 1 - (cooldown);
        // Burst Amount added by Adding
        this.burstAmount += burstAmount;
        // Fire Rate increased in Percentage
        this.fireRate *= 1 + (fireRate);
        // Max Ammo increase by Adding
        this.maxAmmo += maxAmmo;
        // Reload Time decreased in Percentage
        this.reloadTime *= 1 - (reloadTime);
    }

    bool CanShoot() => !isReloading && currentAmmo > 0;

    public override void Shooting()
    {
        if (CanShoot())
        {
            isShooting = true;
            switch (firearmData.fireType)
            {
                case FirearmData.Firetype.singleShot:
                    {
                        if (CanShootSingleShot())
                            StartCoroutine(SingleShot());
                        break;
                    }
                case FirearmData.Firetype.burst:
                    {
                        if (CanShootBurst())
                            StartCoroutine(BurstMode());
                        break;
                    }
                case FirearmData.Firetype.automatic:
                    {
                        if (CanShootAutomatic())
                            AutomaticMode();
                        break;
                    }
                case FirearmData.Firetype.projectile:
                    {
                        if (CanShootProjectile())
                            StartCoroutine(ProjectileMode());
                        break;
                    }
                    //case FirearmData.Firetype.shotgun:
                    //{
                    //    if(CanShootShotgun())
                    //    {
                    //        StartCoroutine(ShotgunMode());
                    //    }
                    //    break;
                    //}
            }
        }
    }

    public override void OnButtonUp()
    {
        canBurst = true;
        canSingleShoot = true;
        canProjectile = true;
        canShotgun = true;
        isShooting = false;
        switch (firearmData.firearmType)
        {
            case FirearmData.FirearmType.gatlingGun:
                source.pitch = 1;
                break;
        }
    }

    bool CanShootSingleShot() => !isSingleShoting && canSingleShoot;

    IEnumerator SingleShot()
    {
        isSingleShoting = true;
        canSingleShoot = false;



        Shoot();
        Recoil();

        events.onSingleShot?.Invoke();
        events.onShooting?.Invoke();

        yield return new WaitForSeconds(cooldown);
        isSingleShoting = false;
    }

    bool CanShootBurst() => !isBursting && canBurst;

    IEnumerator BurstMode()
    {
        isBursting = true;
        canBurst = false;

        for (int i = 0; i < burstAmount; i++)
        {
            if (currentAmmo <= 0)
                break;

            Recoil();
            Shoot();

            events.onShooting?.Invoke();

            events.onBurst?.Invoke();


            yield return new WaitForSeconds(firearmData.baseTimeBetweenBurst);
        }

        yield return new WaitForSeconds(cooldown);
        isBursting = false;
    }

    bool CanShootAutomatic() => Time.time > (1 / (fireRate / 60)) + timeSinceLastShot;

    void AutomaticMode()
    {
        Shoot();
        Recoil();

        events.onAutomatic?.Invoke();

        events.onShooting?.Invoke();

        timeSinceLastShot = Time.time;
    }

    bool CanShootProjectile() => !isProjectile && canProjectile;

    IEnumerator ProjectileMode()
    {
        isProjectile = true;
        canProjectile = false;



        currentAmmo--;

        events.onShooting?.Invoke();

        Projectile();

        Recoil();

        yield return new WaitForSeconds(cooldown);
        isProjectile = false;
    }

    bool CanShootShotgun() => !isShotgun && canShotgun;

    //IEnumerator ShotgunMode()
    //{
    //    if(photonView.IsMine)
    //    {
    //        isShotgun = true;
    //        canShotgun = false;

    //        events.onShooting?.Invoke();

    //        mainEnd.Clear();
    //        hitToEnd.Clear();
    //        mainBegin.Clear();
    //        hitToBegin.Clear();

    //        for(int i = 0; i < firearmData.shotgunBulletsAmount; i++)
    //        {
    //            Vector3 direction = mainCam.transform.forward;
    //            Vector3 spread = mainCam.transform.position;
    //            spread += mainCam.transform.up * Random.Range(-firearmData.ySpread, firearmData.ySpread);
    //            spread += mainCam.transform.right * Random.Range(-firearmData.xSpread, firearmData.xSpread);

    //            direction += spread.normalized * Random.Range(0, 02f);

    //            Vector3 shotgunHit = mainCam.transform.forward * firearmData.raycastDistance;
    //            if(Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit shotgunOut, firearmData.raycastDistance))
    //            {
    //                shotgunHit = shotgunOut.point;
    //            }
    //            else
    //            {
    //                shotgunHit = mainCam.transform.forward * firearmData.raycastDistance;
    //            }

    //            mainBegin.Add(mainCam.transform.position);
    //            hitToBegin.Add(shotgunHit);

    //            Vector3 spread = mainCam.transform.forward * firearmData.raycastDistance;
    //            spread.x += Random.Range(-firearmData.xSpread, firearmData.xSpread);
    //            spread.y += Random.Range(-firearmData.ySpread, firearmData.ySpread);
    //            shotgunHit += spread.normalized;

    //            mainEnd.Add(mainCam.transform.position);
    //            hitToEnd.Add(shotgunHit);

    //            Vector3 hitPoint = new Vector3(shotgunHit.x, shotgunHit.y, shotgunHit.z);

    //            Vector3 particlePoint;
    //            bool enemyHit = false;


    //            if(Physics.Linecast(mainCam.transform.position, hitPoint, out hit))
    //            {
    //                if(hit.transform.TryGetComponent(out IDamagable damagable))
    //                {
    //                    damagable.Damagable(damage, events.onKillEnemy, events.onHitEnemy, bulletForce, Camera.main.transform.forward);
    //                    enemyHit = true;
    //                }

    //                particlePoint = hit.point;
    //            }
    //            else
    //            {
    //                particlePoint = hitPoint;
    //            }

    //            if(dataHolder.shootEffect != null)
    //            {
    //                if(photonView.IsMine)
    //                {
    //                    photonView.RPC("RPCBulletTracers", RpcTarget.All, enemyHit, particlePoint);
    //                }
    //            }

    //            mainEnd.Add(mainCam.transform.position);
    //            hitToEnd.Add(particlePoint);

    //        }

    //        Recoil();
    //        currentAmmo--;


    //        yield return new WaitForSeconds(cooldown);
    //        isShotgun = false;
    //    }
    //}

    //public List<Vector3> mainEnd;
    //public List<Vector3> hitToEnd;
    //public List<Vector3> mainBegin;
    //public List<Vector3> hitToBegin;

    //public Vector3 mainForwatd;
    //public Vector3 mainPos;

    //private void Update()
    //{
    //    if(mainEnd.Count > 0)
    //    {
    //        for(int i = 0; i < firearmData.shotgunBulletsAmount; i++)
    //        {
    //            Debug.DrawLine(mainEnd[i], hitToEnd[i], Color.red);
    //        }
    //    }

    //    if(mainBegin.Count > 0)
    //    {
    //        for(int i = 0; i < firearmData.shotgunBulletsAmount; i++)
    //        {
    //            Debug.DrawLine(mainBegin[i], hitToBegin[i], Color.green);
    //        }
    //    }

    //    Debug.DrawRay(mainCam.transform.position, mainCam.transform.forward * firearmData.raycastDistance, Color.blue);
    //}

    public bool IsReload() => isReloading; 

    public override IEnumerator Reloading()
    {
        events.onStartReloading?.Invoke();
        isReloading = true;

        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }

        source.clip = firearmData.reloadSound;
        PlaySound();

        yield return new WaitForSeconds(reloadTime / 2);
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

    void Shoot()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        GunType();
        source.clip = firearmData.shootSound;
        PlaySound();

        Vector3 particlePoint;
        bool enemyHit = false;

        Vector3 mainForward;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit mainHit, firearmData.raycastDistance))
        {
            mainForward = mainHit.point;
        }
        else
        {
            mainForward = mainCam.transform.forward * firearmData.raycastDistance;
        }

        //Vector3 gunForward;
        //if(Physics.Raycast(dataHolder.muzzle.position, dataHolder.muzzle.forward, out RaycastHit gunHit, firearmData.raycastDistance))
        //{
        //    gunForward = gunHit.point;
        //}
        //else
        //{
        //    gunForward = dataHolder.muzzle.forward * firearmData.raycastDistance;
        //}

        Vector3 hitPoint = new Vector3(mainForward.x, mainForward.y, mainForward.z);

        if (Physics.Linecast(mainCam.transform.position, hitPoint, out hit))
        {
            if (hit.transform.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damagable(damage, events.onKillEnemy, events.onHitEnemy, bulletForce, Camera.main.transform.forward);
                enemyHit = true;
            }

            particlePoint = hit.point;
        }
        else
        {
            particlePoint = hitPoint;
        }

        if (dataHolder.shootEffect != null)
        {
            RPCBulletTracers(enemyHit, particlePoint);
        }

        currentAmmo--;
    }

    void RPCBulletTracers(bool enemyHit, Vector3 particlePoint)
    {
        dataHolder.shootEffect.Activate(enemyHit, hit.normal, particlePoint);

    }

    void Projectile()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        Debug.Log("Instantiate");

        source.clip = firearmData.shootSound;
        PlaySound();

        GameObject projectile = Instantiate(firearmData.projectilePrefab, transform.GetChild(0).transform.GetChild(0).position, transform.rotation);

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit otherHit, firearmData.raycastDistance))
        {
            raycastHitPoint = otherHit.point;
        }
        else
        {
            raycastHitPoint = mainCam.transform.position + mainCam.transform.forward * firearmData.raycastDistance;
        }

        if (projectile.TryGetComponent<Projectile>(out Projectile pro))
        {
            pro.InitializeProjectile(damage, firearmData.projectileSpeed, firearmData.radius, transform.position, raycastHitPoint);
            pro.InitialzieEvent(events.onHitEnemy, events.onKillEnemy);
        }
    }

    public void PlaySound()
    {
        if(source != null)
        {
            source.PlayOneShot(source.clip);
        }
    }

    void GunType()
    {
        switch(firearmData.firearmType)
        {
            case FirearmData.FirearmType.handgun:
            {
                break;
            }
            case FirearmData.FirearmType.shotgun:
            {
                break;
            }
            case FirearmData.FirearmType.assaultRifle:
            {
                break;
            }
            case FirearmData.FirearmType.rocketLauncher:
            {
                break;
            }
            case FirearmData.FirearmType.gatlingGun:
            {
                dataHolder.gatlingBarrel.localEulerAngles = new Vector3(dataHolder.gatlingBarrel.localEulerAngles.x, dataHolder.gatlingBarrel.localEulerAngles.y, dataHolder.gatlingBarrel.localEulerAngles.z + firearmData.rotationAmount);
                if(source.pitch < 1.3f)
                {
                    source.pitch += 0.005f;
                }

                break;
            }
        }
    }
}