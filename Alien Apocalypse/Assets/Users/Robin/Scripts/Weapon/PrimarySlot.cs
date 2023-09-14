using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimarySlot : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;

    private void Update()
    {
        //if(Input.GetButton("Fire1"))
        //    weapon.Shooting(1);
        //else if(Input.GetButtonUp("Fire1"))
        //    weapon.Shooting(0);

        //if(Input.GetKeyDown(KeyCode.R))
        //    StartCoroutine(weapon.Reloading());
    }
}
