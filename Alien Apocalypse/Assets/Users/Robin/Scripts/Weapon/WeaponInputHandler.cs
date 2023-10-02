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
    public List<FirearmAbility> weaponAbilities;

    [Header("Weapon")]
    public GameObject weaponHolder;
    public Weapon selectedWeapon;
    public List<Weapon> weaponSlots;
    public float scrollNum = 0;
    public float oldScrollNum = 0;

    [Space]
    [Header("Firearm Events")]
    public FirearmEvents events;

    void Start()
    {
        SetWeapon();
    }

    void Update()
    {
        SelectWeapon();

        if(selectedWeapon != null)
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
            if (selectedWeapon.GetComponent<Firearm>().currentAmmo < selectedWeapon.GetComponent<Firearm>().maxAmmo && selectedWeapon.GetComponent<Firearm>().isReloading == false)
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
                if(selectedWeapon.transform.TryGetComponent<Firearm>(out Firearm currentFirearm))
                {
                    if(currentFirearm.firearmData != null)
                    {
                        selectedWeapon.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }

            if(weaponSlots[Mathf.Abs((int)scrollNum)].transform.TryGetComponent<Firearm>(out Firearm nextFirearm))
            {
                if(nextFirearm.firearmData != null)
                {
                    selectedWeapon = weaponSlots[Mathf.Abs((int)scrollNum)];

                    selectedWeapon.transform.GetChild(0).gameObject.SetActive(true);

                    selectedWeapon.mainCam = mainCam;
                    selectedWeapon.recoilObject = recoil;
                }
                else if(nextFirearm.firearmData == null)
                {
                    selectedWeapon.mainCam = null;
                    selectedWeapon.recoilObject = null;

                    selectedWeapon = null;                    
                }
            }

            oldScrollNum = scrollNum;
        }
    }

    public bool CanAddWeapon()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].TryGetComponent<Firearm>(out Firearm firearm))
            {
                if(firearm.firearmData == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void AddWeapon(FirearmData firearmData)
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].TryGetComponent<Firearm>(out Firearm firearm))
            {
                if(firearm.firearmData == null)
                {
                    firearm.firearmData = firearmData;
                    SetWeapon();
                }
            }            
        }
    }

    void SetWeapon()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].transform.childCount == 0)
            {
                if(weaponSlots[i].TryGetComponent<Firearm>(out Firearm firearm))
                {
                    if(firearm.firearmData != null)
                    {
                        GameObject weapon = PhotonNetwork.Instantiate(firearm.firearmData.prefab.name, transform.position, transform.rotation);

                        weapon.transform.parent = weaponSlots[i].transform;

                        weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                        weapon.transform.localScale = new Vector3(1, 1, 1);

                        firearm.events = events;

                        if(weapon.TryGetComponent<Animator>(out Animator animator))
                        {
                            firearm.animator = animator;
                        }

                        if(weapon.TryGetComponent<AudioSource>(out AudioSource audioSource))
                        {
                            firearm.source = audioSource;
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
    }

    public void AddAbility(FirearmAbility firearmAbility)
    {
        weaponAbilities.Add(firearmAbility);
        SetAbility();
    }

    void SetAbility()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].transform.childCount > 0)
            {
                if(weaponSlots[i].TryGetComponent<Firearm>(out Firearm firearm))
                {
                    for(int j = 0; j < weaponAbilities.Count; j++)
                    {
                        firearm.ModifyWeaponData(weaponAbilities[j].damage, weaponAbilities[j].cooldown, weaponAbilities[j].burstAmount, weaponAbilities[j].fireRate, weaponAbilities[j].maxAmmo, weaponAbilities[j].reloadTime); ;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class FirearmEvents
{
    public UnityEvent onShooting;
    public UnityEvent onSingleShot;
    public UnityEvent onBurst;
    public UnityEvent onAutomatic;
    public UnityEvent onHitEnemy;
    public UnityEvent onKillEnemy;
    public UnityEvent onStartReloading;
    public UnityEvent onEndReloading;
}