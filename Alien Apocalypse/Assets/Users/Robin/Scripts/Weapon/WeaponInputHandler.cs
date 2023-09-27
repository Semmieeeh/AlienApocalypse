using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class WeaponInputHandler : MonoBehaviourPunCallbacks
{
    [Header("Recoil")]
    public Camera mainCam;
    public GameObject recoil;

    [Header("Ability")]
    public List<Ability> weaponAbilities;

    [Header("Weapon")]
    public Weapon selectedWeapon;
    public List<Weapon> weaponSlots;
    public float scrollNum;
    public float oldScrollNum = 1;

    void Start()
    {

    }

    void Update()
    {
        SelectWeapon();

        if(selectedWeapon.firearmData != null)
            InputWeapon();
    }

    void InputWeapon()
    {
        if(Input.GetButton("Fire1"))
            selectedWeapon.Shooting();
        else if(Input.GetButtonUp("Fire1"))
            selectedWeapon.OnButtonUp();

        if(Input.GetKeyDown(KeyCode.R))
            selectedWeapon.StartCoroutine(selectedWeapon.Reloading());

        transform.localPosition = selectedWeapon.Sway(transform.localPosition);
        selectedWeapon.UpdateWeapon();
    }

    [PunRPC]
    void SelectWeapon()
    {
        scrollNum += Input.mouseScrollDelta.y;
        scrollNum = Mathf.Clamp(scrollNum, -weaponSlots.Count + 1, 0);

        if(oldScrollNum != scrollNum)
        {
            if(selectedWeapon.transform.childCount > 0)
            {
                PhotonNetwork.Destroy(selectedWeapon.transform.GetChild(0).gameObject);
            }

            if(weaponSlots[Mathf.Abs((int)scrollNum)] != null)
            {
                selectedWeapon = weaponSlots[Mathf.Abs((int)scrollNum)];
                
                if(selectedWeapon.firearmData != null)
                {
                    GameObject weapon = PhotonNetwork.Instantiate(selectedWeapon.firearmData.prefab.name, selectedWeapon.transform.position, selectedWeapon.transform.rotation);

                    weapon.transform.parent = selectedWeapon.transform;

                    weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    weapon.transform.localScale = new Vector3(1, 1, 1);

                    selectedWeapon.mainCam = mainCam;
                    selectedWeapon.recoilObject = recoil;
                }
            }

            oldScrollNum = scrollNum;
        }
    }
}
