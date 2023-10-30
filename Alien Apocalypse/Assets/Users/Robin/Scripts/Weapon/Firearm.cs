using Photon.Pun;
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
    [HideInInspector]
    public FirearmEvents events;

    public override void StartWeapon()
    {
        if (photonView.IsMine)
        {
            firearmCurrentPosition = firearmData.localPlacmentPos;
            transform.GetChild(0).GetComponent<Children>().enabled = true;
            SetWeaponData();
        }
    }

    public override void UpdateWeapon()
    {
        if (photonView.IsMine)
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
    }

    void SetWeaponData()
    {
        if (photonView.IsMine)
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
        }
    }

    public void ModifyWeaponData(float damage, float cooldown, int burstAmount, float fireRate, int maxAmmo, float reloadTime)
    {
        if (photonView.IsMine)
        {
            // Damage increase in Percentage
            this.damage *= 1 + (damage / 100);
            // Cooldown decrease in Percentage
            this.cooldown *= 1 - (cooldown / 100);
            // Burst Amount added by Adding
            this.burstAmount += burstAmount;
            // Fire Rate increased in Percentage
            this.fireRate *= 1 + (fireRate / 100);
            // Max Ammo increase by Adding
            this.maxAmmo += maxAmmo;
            // Reload Time decreased in Percentage
            this.reloadTime *= 1 - (reloadTime / 100);
        }
    }

    bool CanShoot() => !isReloading && currentAmmo > 0;

    public override void Shooting()
    {
        if (photonView.IsMine)
        {
            if (CanShoot())
            {
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
                        if(CanShootProjectile())
                            StartCoroutine(ProjectileMode());
                        break;
                    }
                }
            }
        }
    }

    public override void OnButtonUp()
    {
        if (photonView.IsMine)
        {
            canBurst = true;
            canSingleShoot = true;
            canProjectile = true;

            source.pitch = 1;            
        }
    }

    bool CanShootSingleShot() => !isSingleShoting && canSingleShoot;

    IEnumerator SingleShot()
    {
        if (photonView.IsMine)
        {
            isSingleShoting = true;
            canSingleShoot = false;

            events.onShooting?.Invoke();

            events.onSingleShot?.Invoke();

            Shoot();
            Recoil();

            yield return new WaitForSeconds(cooldown);
            isSingleShoting = false;
        }
    }

    bool CanShootBurst() => !isBursting && canBurst;

    IEnumerator BurstMode()
    {
        if (photonView.IsMine)
        {
            isBursting = true;
            canBurst = false;

            for (int i = 0; i < burstAmount; i++)
            {
                if (currentAmmo <= 0)
                    break;

                events.onShooting?.Invoke();

                events.onBurst?.Invoke();

                Shoot();
                Recoil();

                yield return new WaitForSeconds(firearmData.baseTimeBetweenBurst);
            }

            yield return new WaitForSeconds(cooldown);
            isBursting = false;
        }    
    }

    bool CanShootAutomatic() => Time.time > (1 / (fireRate / 60)) + timeSinceLastShot;

    void AutomaticMode()
    {
        if (photonView.IsMine)
        {           
            // OnShooting will always be called if CanShoot is true and doesn't regard the FireType
            events.onShooting?.Invoke();

            // OnAutomatic will be called every time a projectile is fired; The FireType has to be Automatic
            events.onAutomatic?.Invoke();

            Shoot();
            Recoil();

            timeSinceLastShot = Time.time;
            
        }        
    }

    bool CanShootProjectile() => !isProjectile && canProjectile;

    IEnumerator ProjectileMode()
    {
        if(photonView.IsMine)
        {
            isProjectile = true;
            canProjectile = false;

            events.onShooting?.Invoke();

            photonView.RPC(nameof(Projectile), RpcTarget.All);
            Recoil();
        }

        yield return new WaitForSeconds(cooldown);
        isProjectile = false;
    }

    public bool IsReload() => isReloading; 

    public override IEnumerator Reloading()
    {
        if (photonView.IsMine)
        {
            events.onStartReloading?.Invoke();
            isReloading = true;

            if (animator != null)
            {
                animator.SetTrigger("Reload");
            }

            source.clip = firearmData.reloadSound;
            photonView.RPC("PlaySound", RpcTarget.All);

            yield return new WaitForSeconds(reloadTime / 2);
            currentAmmo = maxAmmo;
            yield return new WaitForSeconds(reloadTime / 2);

            isReloading = false;

            events.onEndReloading?.Invoke();
        }
    }

    public override Vector3 Sway(Vector3 pos)
    {
        if(photonView.IsMine)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            input.x = Mathf.Clamp(input.x, -firearmData.swayClamp, firearmData.swayClamp);
            input.y = Mathf.Clamp(input.y, -firearmData.swayClamp, firearmData.swayClamp);

            Vector3 target = new Vector3(input.x, input.y, 0);

            Vector3 newPos = Vector3.Lerp(pos, target + Vector3.zero, Time.deltaTime * firearmData.smoothing);

            return newPos;
        }
        else
        {
            return Vector3.zero;
        }
    }

    void Recoil()
    {
        if (photonView.IsMine)
        {
            camTargetRotation += new Vector3(firearmData.camRecoilX, Random.Range(-firearmData.camRecoilY, firearmData.camRecoilY), Random.Range(-firearmData.camRecoilZ, firearmData.camRecoilZ));
            firearmTargetRotation += new Vector3(firearmData.firearmRecoilX, Random.Range(-firearmData.firearmRecoilY, firearmData.firearmRecoilY), Random.Range(-firearmData.firearmRecoilZ, 0));
            firearmTargetPosition = new Vector3(firearmData.localPlacmentPos.x, firearmData.localPlacmentPos.y, firearmData.firearmRecoilBackUp + firearmData.localPlacmentPos.z);
        }
    }

    void Shoot()
    {
        if (photonView.IsMine)
        {
            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            GunType();
            source.clip = firearmData.shootSound;
            photonView.RPC("PlaySound", RpcTarget.All);

            Vector3 particlePoint;
            bool enemyHit = false;

            Vector3 mainForward = mainCam.transform.forward * firearmData.raycastDistance;
            Vector3 gunForward = dataHolder.muzzle.forward * firearmData.raycastDistance;

            Vector3 hitPoint = new Vector3(mainForward.x, gunForward.y, mainForward.z);

            if(Physics.Linecast(mainCam.transform.position, hitPoint, out hit))
            {
                if(hit.transform.TryGetComponent(out IDamagable damagable))
                {
                    damagable.Damagable(damage, events.onKillEnemy, events.onHitEnemy,bulletForce);
                    enemyHit = true;
                }

                particlePoint = hit.point;
            }
            else
            {
                particlePoint = hitPoint;
            }

            if(dataHolder.shootEffect != null)
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("RPCBulletTracers", RpcTarget.All, enemyHit, particlePoint);
                }
            }

            currentAmmo--;
        }
    }
    [PunRPC]
    void RPCBulletTracers(bool enemyHit, Vector3 particlePoint)
    {
        dataHolder.shootEffect.Activate(enemyHit, hit.normal, particlePoint);

    }

    [PunRPC]
    void Projectile()
    {
        if(photonView.IsMine)
        {
            if(animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            Debug.Log("Instantiate");

            source.clip = firearmData.shootSound;
            photonView.RPC("PlaySound", RpcTarget.All);

            GameObject projectile = PhotonNetwork.Instantiate(firearmData.projectilePrefab.name, transform.GetChild(0).transform.GetChild(0).position, transform.rotation);

            if(Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit otherHit, firearmData.raycastDistance))
            {
                raycastHitPoint = otherHit.point;
            }
            else
            {
                raycastHitPoint = mainCam.transform.position + mainCam.transform.forward * firearmData.raycastDistance;
            }

            if(projectile.TryGetComponent<Projectile>(out Projectile pro))
            {
                pro.InitializeProjectile(damage, firearmData.projectileSpeed, firearmData.radius, transform.position, raycastHitPoint);
                pro.InitialzieEvent(events.onHitEnemy, events.onKillEnemy);
            }

            //if(dataHolder.shootEffect != null)
            //{
            //    dataHolder.shootEffect.Activate(otherHit);
            //}



            currentAmmo--;
        }
    }

    [PunRPC]
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