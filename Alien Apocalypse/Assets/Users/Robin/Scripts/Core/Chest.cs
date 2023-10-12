using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Chest : MonoBehaviourPunCallbacks, IInteractable
{
    public FirearmData[] firearmDatas;
    public Transform goToPoint;

    public void Interact(WeaponInputHandler handler)
    {

    }

    [PunRPC]
    void OpenChest()
    {
        //PhotonNetwork.Instantiate(new GameObject, transform.position, new Ve)
    }
}
