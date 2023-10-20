using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActiveOnServer : MonoBehaviourPunCallbacks
{
    public GameObject[] otherWeapons;
    public GameObject targetWeapon;
    private void Update()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("WeaponOnServer", RpcTarget.All);
        }
    }

    [PunRPC]
    void WeaponOnServer()
    {
        if (photonView.IsMine)
        {
            gameObject.SetActive(true);
            foreach (GameObject weapon in otherWeapons)
            {
                weapon.SetActive(false);
            }
        }
    }
}
