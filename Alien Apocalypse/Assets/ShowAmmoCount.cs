using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAmmoCount : MonoBehaviour
{
    public Firearm fireArm;

    // Update is called once per frame
    void Update()
    {
        if(fireArm == null)
        {
            fireArm = transform.parent.transform.parent.GetComponent<Firearm>();
            return;
        }
        if(fireArm != null)
        {
            GetComponent<Animator>().SetFloat("AmmoCount",fireArm.currentAmmo);
        }
    }
}
