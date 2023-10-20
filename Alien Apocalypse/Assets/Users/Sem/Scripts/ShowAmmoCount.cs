using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShowAmmoCount : MonoBehaviourPunCallbacks
{
    public Firearm fireArm;

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (fireArm == null)
            {
                fireArm = transform.parent.transform.parent.GetComponent<Firearm>();
                return;
            }
            if (fireArm != null)
            {
                GetComponent<Animator>().SetFloat("AmmoCount", fireArm.currentAmmo);
            }
        }
    }
}
