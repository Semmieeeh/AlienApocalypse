using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirearmData", menuName = "Firearm", order = 0)]
public class FirearmData : WeaponData
{
    [Space]
    [Header("Type")]
    public FirearmType firearmType;
    public enum FirearmType
    {
        handgun,
        shotgun,
        assaultRifle,
        rocketLauncher,
        gatlingGun,
    }

    public enum Firetype
    {
        singleShot,
        burst,
        automatic,
        projectile,
        shotgun,
    }
    public Firetype fireType;

    [Space]
    [Header("General")]
    public float baseDamage;
    public float baseCooldown;
    public float rotationAmount;
    public float bulletForce;

    [Space]
    [Header("Burst")]
    public int baseBurstAmount;
    public float baseTimeBetweenBurst;

    [Space]
    [Header("Automatic")]
    public float baseFireRate;

    [Space]
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float radius;

    [Space]
    [Header("Shotgun")]
    public int shotgunBulletsAmount;
    public float xSpread;
    public float ySpread;

    [Space]
    [Header("Ammo")]
    public int baseMaxAmmo;

    [Space]
    [Header("Reloading")]
    public float baseReloadTime;

    [Space]
    [Header("Raycast")]
    public float raycastDistance;

    [Space]
    [Header("Sway")]
    public float swayClamp;
    public float smoothing;

    [Space]
    [Header("Recoil Camera Rotation")]
    public float camRecoilX;
    public float camRecoilY;
    public float camRecoilZ;
    public float camSnappiness;
    public float camReturnSpeed;

    [Header("Recoil Firearm Rotation")]
    public float firearmRecoilX;
    public float firearmRecoilY;
    public float firearmRecoilZ;
    public float firearmSnappiness;
    public float firearmReturnSpeed;

    [Header("Recoil Firearm Position")]
    public float firearmRecoilBackUp;
    public float backUpSnappiness;
    public float backUpReturnSpeed;

    [Header("Animation Data")]
    public AnimationClip anim;
    public int weaponInt;

    [Header("Audio Data")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header ("UI")]
    public Sprite weaponSprite;

}
