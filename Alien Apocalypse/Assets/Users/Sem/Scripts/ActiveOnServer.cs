using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnServer : MonoBehaviour
{
    public GameObject[] otherWeapons;
    public GameObject targetWeapon;
    private void Update()
    {
        WeaponOnServer();
    }

    void WeaponOnServer()
    {
        gameObject.SetActive(true);
        foreach (GameObject weapon in otherWeapons)
        {
            weapon.SetActive(false);
        }
    }
}
