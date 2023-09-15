using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInputHandler : MonoBehaviour
{
    public Weapon weapon1;
    public Camera mainCam;

    void Start()
    {
        weapon1.transform.localPosition = weapon1.GetLocalPlacmentPos();
        SetWeapon();
    }

    void Update()
    {
        if(Input.GetButton("Fire1"))
            weapon1.Shooting();
        else if(Input.GetButtonUp("Fire1"))
            weapon1.OnButtonUp();

        if(Input.GetKeyDown(KeyCode.R))
            weapon1.StartCoroutine(weapon1.Reloading());

        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        weapon1?.Sway(input);

        weapon1.UpdateWeapon();
    }

    void SetWeapon()
    {
        weapon1.mainCam = mainCam;
    }
}
