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
    public float scrollNum = 0;
    public float oldScrollNum = 0;

    void Start()
    {
        InitializeWeaponSlots();
    }

    void InitializeWeaponSlots()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null)
            {
                SetWeapon();
            }
        }
    }

    void Update()
    {
        SelectWeapon();

        InputWeapon();
    }

    void InputWeapon()
    {
        if(Input.GetButton("Fire1"))
            selectedWeapon.Shooting();
        else if(Input.GetButtonUp("Fire1"))
            selectedWeapon.OnButtonUp();

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (selectedWeapon.GetComponent<Firearm>().currentAmmo < selectedWeapon.GetComponent<Firearm>().maxAmmo)
            {
                selectedWeapon.StartCoroutine(selectedWeapon.Reloading());
            }
        }
            

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
            if(selectedWeapon != null)
            {
                selectedWeapon.transform.GetChild(0).gameObject.SetActive(false);
            }

            if(weaponSlots[Mathf.Abs((int)scrollNum)] != null)
            {
                selectedWeapon = weaponSlots[Mathf.Abs((int)scrollNum)];

                selectedWeapon.transform.GetChild(0).gameObject.SetActive(true);

                selectedWeapon.mainCam = mainCam;
                selectedWeapon.recoilObject = recoil;
            }
            else
            {
                selectedWeapon.mainCam = null;
                selectedWeapon.recoilObject = null;

                selectedWeapon = null;
            }

            oldScrollNum = scrollNum;
        }
    }

    public void AddWeapon(Firearm firearm)
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] == null)
            {
                weaponSlots[i] = firearm;
                SetWeapon();
            }
        }
    }

    void SetWeapon()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].transform.childCount == 0)
            {
                GameObject weapon = PhotonNetwork.Instantiate(weaponSlots[i].firearmData.prefab.name, transform.position, transform.rotation);

                weapon.transform.parent = weaponSlots[i].transform;

                weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                weapon.transform.localScale = new Vector3(1, 1, 1);


                if(weapon.TryGetComponent<Animator>(out Animator animator))
                {
                    weaponSlots[i].GetComponent<Firearm>().anim = animator;
                }

                weaponSlots[i].StartWeapon();

                if(Mathf.Abs((int)scrollNum) == i)
                {
                    selectedWeapon = weaponSlots[i];

                    selectedWeapon.mainCam = mainCam;
                    selectedWeapon.recoilObject = recoil;
                }
                else
                {
                    weaponSlots[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
}