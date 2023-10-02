using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirearmAbility", menuName = "Firearm Ability", order = 2)]
public class FirearmAbility : Ability
{
    [Header("Increase in %")]
    public float damage;
    [Header("Decrease in %")]
    public float cooldown;
    [Header("Increase by Adding")]
    public int burstAmount;
    [Header("Increase in %")]
    public int fireRate;
    [Header("Increase by Adding")]
    public int maxAmmo;
    [Header("Decrease in %")]
    public float reloadTime;
}
