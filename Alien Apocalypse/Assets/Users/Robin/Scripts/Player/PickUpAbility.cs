using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAbility : MonoBehaviour, IInteractable
{
    public FirearmAbility firearmAbility;
    
    public void Interact(WeaponInputHandler handler)
    {
        //if(firearmAbility != null)
        //{
        //    handler.AddAbility(firearmAbility);
        //    photonView.RPC("Destroy",RpcTarget.All);
        //    UIPowerupManager.Instance.OnPickupPowerup (firearmAbility);
        //}
    }

    //[PunRPC]
    //void Destroy()
    //{
    //    PhotonNetwork.Destroy(gameObject);
    //}

}
