using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimarySlot : MonoBehaviour
{
    [SerializeField]
    private Weapon primaryWeapon;

    public Weapon PrimaryWeapon
    {
        get
        {
            return primaryWeapon;
        }

        private set
        {
            primaryWeapon = value;
        }
    }

    public void SetPrimaryWeapon(Weapon weapon)
    {
        PrimaryWeapon = weapon;
    }
}
