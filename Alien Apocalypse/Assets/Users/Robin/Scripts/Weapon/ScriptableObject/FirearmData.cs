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
        assaultRifle
    }

    public enum Firetype
    {
        singleShot,
        burst,
        automatic,
    }
    public Firetype fireType;

    [Space]
    [Header("Shooting")]
    public float baseDamage;

    [Space]
    [Header("Single Shot")]
    public float baseSingleShotCooldown;

    [Space]
    [Header("Burst")]
    public int baseBurstAmount;
    public float baseTimeBetweenBurst;
    public float baseBurstCooldown;

    [Space]
    [Header("Automatic")]
    public float baseFireRate;

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
}
