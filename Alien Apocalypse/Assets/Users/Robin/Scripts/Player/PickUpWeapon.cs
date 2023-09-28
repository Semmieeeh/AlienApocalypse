using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviourPunCallbacks, IInteractable
{
    public FirearmData firearmData;

    public void Interact(WeaponInputHandler handler)
    {
        if(firearmData != null)
        {
            if(handler.CanAddWeapon())
            {
                handler.AddWeapon(firearmData);

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
