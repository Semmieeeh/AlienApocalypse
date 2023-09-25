using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInputHandler : MonoBehaviour
{
    public Camera mainCam;
    public GameObject recoil;
    public Weapon selectedWeapon;


    void Start()
    {
        selectedWeapon.transform.localPosition = selectedWeapon.GetLocalPlacmentPos();
        SetWeapon();
    }

    void Update()
    {
        if(Input.GetButton("Fire1"))
            selectedWeapon.Shooting();
        else if(Input.GetButtonUp("Fire1"))
            selectedWeapon.OnButtonUp();

        if(Input.GetKeyDown(KeyCode.R))
            selectedWeapon.StartCoroutine(selectedWeapon.Reloading());
        //weapon1.GetComponent<Animator>().SetTrigger("Reload");

        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        transform.localPosition = selectedWeapon.Sway(input, transform.localPosition);

        selectedWeapon.UpdateWeapon(input);
    }

    void SetWeapon()
    {
        selectedWeapon.StartWeapon();
        selectedWeapon.mainCam = mainCam;
        selectedWeapon.recoilObject = recoil;
    }
}
