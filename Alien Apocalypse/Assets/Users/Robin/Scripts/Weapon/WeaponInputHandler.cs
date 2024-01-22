using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInputHandler : MonoBehaviour
{
    [Header("Recoil")]
    public Camera mainCam;
    public GameObject recoil;

    [Header("Weapon")]
    public GameObject weaponHolder;
    public Weapon selectedWeapon;
    public List<Weapon> weaponSlots;
    public float scrollNum = 0;
    public float oldScrollNum = 0;

    [Space]
    [Header("Firearm Events")]
    public FirearmEvents events;

    [Header("Arm Animations")]
    public Animator anim;
    private GameObject previousWeapon;

    [Header("Modifier")]
    public float damageModifier;
    public int damageLevel;
    public float switchTime = 0.5f;

    void Start()
    {
        SetWeapon();
    }

    void Update()
    {
        SelectWeapon();
        switchTime -=Time.deltaTime;
        InputWeapon();
    }

    
    void InputWeapon()
    {
        if(selectedWeapon != null)
        {
            if(Input.GetButton("Fire1"))
                selectedWeapon.Shooting();
            else if(Input.GetButtonUp("Fire1"))
                selectedWeapon.OnButtonUp();

            if(Input.GetKeyDown(KeyCode.R))
            {
                if(selectedWeapon.GetComponent<Firearm>().currentAmmo < selectedWeapon.GetComponent<Firearm>().maxAmmo && selectedWeapon.GetComponent<Firearm>().isReloading == false)
                {
                    selectedWeapon.StartCoroutine(selectedWeapon.Reloading());
                }
            }

            transform.localPosition = selectedWeapon.Sway(transform.localPosition);
            selectedWeapon.UpdateWeapon();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
        }
    }

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
                    if(currentFirearm.IsReload() == true || switchTime >0f)
                    {
                        scrollNum = oldScrollNum;
                        return;
                    }
                    else
                    {
                        if(currentFirearm.firearmData != null)
                        {
                            if(selectedWeapon.transform.childCount > 0)
                            {
                                selectedWeapon.transform.GetChild(0).gameObject.SetActive(false);
                                //photonView.RPC("UpdateAnimations", RpcTarget.All);
                                switchTime = 0.5f;
                            }
                        }
                    }
                }
            }

            if(weaponSlots[Mathf.Abs((int)scrollNum)].transform.TryGetComponent<Firearm>(out Firearm nextFirearm))
            {
                if(nextFirearm.firearmData != null)
                {
                    selectedWeapon = weaponSlots[Mathf.Abs((int)scrollNum)];

                    if(selectedWeapon.transform.GetChild(0) != null)
                    {
                        selectedWeapon.transform.GetChild(0).gameObject.SetActive(true);
                        //photonView.RPC("UpdateAnimations", RpcTarget.All);
                    }

                    AmmoCounter.Instance.SetWeaponData(nextFirearm.firearmData,nextFirearm );


                    selectedWeapon.mainCam = mainCam;
                    selectedWeapon.recoilObject = recoil;
                }
                else if(nextFirearm.firearmData == null)
                {
                    selectedWeapon.mainCam = null;
                    selectedWeapon.recoilObject = null;
                    
                    selectedWeapon = null;

                    AmmoCounter.Instance.SetNoWeapon ( );

                    //photonView.RPC("UpdateAnimations", RpcTarget.All);
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

                    return;
                }
            }
        }
    }

    public void SetWeapon()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].transform.childCount == 0)
            {
                if(weaponSlots[i].TryGetComponent<Firearm>(out Firearm firearm))
                {
                    if(firearm.firearmData != null)
                    {
                        GameObject weapon = Instantiate(firearm.firearmData.prefab, transform.position, transform.rotation);
                        

                        weapon.transform.parent = weaponSlots[i].transform;

                        weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                        weapon.transform.localScale = new Vector3(1, 1, 1);

                        for(int j = 0; i < damageLevel; j++)
                        {
                            firearm.ModifyWeaponData(damageModifier, 0, 0, 0, 0, 0);
                        }

                        firearm.events = events;

                        if(weapon.TryGetComponent<DataHolder>(out DataHolder dataHolder))
                        {
                            firearm.dataHolder = dataHolder;
                        }

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

    public void SetAbility(float damage, int damageLevel)
    {
        damageModifier = damage;
        this.damageLevel = damageLevel;

        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i].transform.childCount > 0)
            {
                if(weaponSlots[i].TryGetComponent<Firearm>(out Firearm firearm))
                {
                    firearm.ModifyWeaponData(damage, 0, 0, 0, 0, 0);
                }
            }
        }
    }

    void DropWeapon()
    {
        if(selectedWeapon != null)
        {
            if(selectedWeapon.transform.TryGetComponent<Firearm>(out Firearm currentFirearm))
            {
                if(currentFirearm.firearmData != null)
                {
                    Destroy(selectedWeapon.transform.GetChild(0).gameObject);
                    GameObject gun = Instantiate(currentFirearm.firearmData.dropPrefab, transform.position, Camera.main.transform.rotation);
                    
                    Rigidbody rb = gun.GetComponent<Rigidbody>();
                    rb.AddForce(Camera.main.transform.forward * 5,ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(0,0), Random.Range(0,0), Random.Range(-2f,2f));
                    rb.AddTorque(torque * 5); 
                    

                    selectedWeapon.mainCam = null;
                    selectedWeapon.recoilObject = null;

                    currentFirearm.firearmData = null;
                    selectedWeapon = null;
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