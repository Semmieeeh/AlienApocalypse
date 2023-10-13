using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Chest : MonoBehaviourPunCallbacks, IInteractable
{
    public float firearmChance;
    public float firearmAbilityChance;
    public float nothingChance;

    public Transform goToPoint;

    public GameObject[] firearmDatas;
    public FirearmAbility[] firearmAbilities;
    public GameObject abilityHolder;

    public void Interact(WeaponInputHandler handler)
    {
        photonView.RPC(nameof(OpenChest), RpcTarget.All);
    }

    [PunRPC]
    void OpenChest()
    {
        float value = Random.value;

        if(value >= ((100 - firearmChance) / 100))
        {
            int n = Random.Range(0, firearmDatas.Length);

            for(int i = 0; i < firearmDatas.Length; i++)
            {
                if(n == i)
                {
                    PhotonNetwork.Instantiate(firearmDatas[i].name, goToPoint.position, Quaternion.identity);
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
                    }
                }
            }
        }
        else if(value >= ((100 - nothingChance) / 100))
        {
            return;
        }
    }
}
