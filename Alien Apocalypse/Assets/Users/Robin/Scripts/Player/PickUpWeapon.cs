using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpWeapon : MonoBehaviour, IInteractable
{
    public FirearmData firearmData;

    public void Interact(WeaponInputHandler handler)
    {
        if(handler.CanAddWeapon())
        {
            if(firearmData != null)
            {
                handler.AddWeapon(firearmData);
                
                Destroy(gameObject);
            }
        }
    }
}
