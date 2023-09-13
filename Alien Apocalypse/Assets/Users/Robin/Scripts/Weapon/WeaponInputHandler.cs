using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInputHandler : MonoBehaviour
{
    public Weapon weapon1;


    void Start()
    {
        weapon1.transform.localPosition = weapon1.GetLocalPlacmentPos();
    }

    void Update()
    {
        if(Input.GetButton("Fire1"))
            weapon1.Shooting();
        
        if(Input.GetKeyDown(KeyCode.R))
            weapon1.StartCoroutine(weapon1.Reloading());
    }
}
