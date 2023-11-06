using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Chest : MonoBehaviourPunCallbacks, IInteractable
{
    public ChestManager chestManager;

    [Space]
    [Header("Spawn Chances")]
    public float minFirearmChance;
    public float maxFirearmChance;
    public float minFirearmAbilityChance;
    public float maxFirearmAbilityChance;
    public float minBombChance;
    public float maxBombChance;

    [Header("Prefabs")]
    public GameObject[] firearmDatas;
    public GameObject[] firearmAbilities;

    public GameObject bomb;
    public GameObject chestBeacon;
    public Transform goToPoint;
    public float force;
    public bool opened = false;

    [Header("Chest")]
    public float rotSpeed;
    public Vector3 targetRot;
    public Transform chestLid;

    public void Interact(WeaponInputHandler handler)
    {
        if(!opened)
        {
            photonView.RPC("OpenChest", RpcTarget.All);
        }
    }

    [PunRPC]
    void OpenChest()
    {
        chestBeacon.SetActive(false);
        chestManager.New();

        GetComponent<Animator>().SetTrigger("Open");
        float value = Random.value;

        if(value >= minFirearmChance / 100 && value <= maxFirearmChance / 100)
        {
            int n = Random.Range(0, firearmDatas.Length - 1);

            for(int i = 0; i < firearmDatas.Length; i++)
            {
                if(n == i)
                {
                    GameObject gun = PhotonNetwork.Instantiate(firearmDatas[i].name, goToPoint.position, Quaternion.identity);

                    Rigidbody rb = gun.GetComponent<Rigidbody>();
                    rb.AddForce(goToPoint.forward * force, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(0, 0), Random.Range(0, 0), Random.Range(-2f, 2f));
                    rb.AddTorque(torque * 5);

                    opened = true;

                    return;
                }
            }
        }
        else if(value >= minFirearmAbilityChance / 100 && value <= maxFirearmAbilityChance / 100)
        {
            int n = Random.Range(0, firearmAbilities.Length - 1);

            for(int i = 0; i < firearmAbilities.Length; i++)
            {
                if(n == i)
                {
                    GameObject holder = PhotonNetwork.Instantiate(firearmAbilities[n].name, goToPoint.position, Quaternion.identity);

                    Rigidbody rb = holder.GetComponent<Rigidbody>();
                    rb.AddForce(goToPoint.forward * force, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(0, 0), Random.Range(0, 0), Random.Range(-2f, 2f));
                    rb.AddTorque(torque * 5);

                    opened = true;

                    return;
                    
                }
            }
        }
        else if(value >= minBombChance / 100 && value <= maxBombChance / 100)
        {
            opened = true;

            return;
        }
    }
}
