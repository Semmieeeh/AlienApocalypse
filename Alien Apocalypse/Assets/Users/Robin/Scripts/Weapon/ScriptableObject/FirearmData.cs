using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Firetype fireType;
    public enum Firetype
    {
        singleShot,
        burst,
        automatic,
    }
}
