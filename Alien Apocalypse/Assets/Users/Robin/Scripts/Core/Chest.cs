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
    public float firearmChance;
    public float firearmAbilityChance;
    public float nothingChance;

    [Header("Prefabs")]
    public GameObject[] firearmDatas;
    public FirearmAbility[] firearmAbilities;
    public GameObject abilityHolder;

    public Transform goToPoint;
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
            chestManager.New();
        }
    }

    [PunRPC]
    void OpenChest()
    {
        GetComponent<Animator>().SetTrigger("Open");
        float value = Random.value;

        if(value >= ((100 - firearmChance) / 100))
        {
            int n = Random.Range(0, firearmDatas.Length);

            for(int i = 0; i < firearmDatas.Length; i++)
            {
                if(n == i)
                {
                    PhotonNetwork.Instantiate(firearmDatas[i].name, goToPoint.position, Quaternion.identity);
                    opened = true;

                    return;
                }
            }
        }
        else if(value >= ((100 - firearmAbilityChance) / 100))
        {
            int n = Random.Range(0, firearmAbilities.Length);

            for(int i = 0; i < firearmAbilities.Length; i++)
            {
                if(n == i)
                {
                    GameObject holder = PhotonNetwork.Instantiate(abilityHolder.name, goToPoint.position, Quaternion.identity);

                    if(holder.TryGetComponent<PickUpAbility>(out PickUpAbility pickUpAbility))
                    {
                        pickUpAbility.firearmAbility = firearmAbilities[i];
                        opened = true;

                        return;
                    }
                }
            }
        }
        else if(value >= ((100 - nothingChance) / 100))
        {
            opened = true;

            return;
        }
    }
}
