using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PickUpWeapon : MonoBehaviourPunCallbacks, IInteractable
{
    public FirearmData firearmData;

    public void Interact(WeaponInputHandler handler)
    {
        if(handler.CanAddWeapon())
        {
            if(firearmData != null)
            {
                handler.AddWeapon(firearmData);
                
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
