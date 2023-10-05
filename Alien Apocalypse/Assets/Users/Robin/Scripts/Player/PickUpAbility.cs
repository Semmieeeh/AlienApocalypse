using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class PickUpAbility : MonoBehaviourPunCallbacks, IInteractable
{
    public FirearmAbility firearmAbility;

    
    public void Interact(WeaponInputHandler handler)
    {
        if(firearmAbility != null)
        {
            handler.AddAbility(firearmAbility);
            photonView.RPC("Destroy",RpcTarget.All);
        }
    }
    [PunRPC]
    void Destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

}
