using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAmmoCount : MonoBehaviour
{
    public Firearm fireArm;
    public Animator anim;
    public Animator anim2;
    public float animSpeed;

    // Update is called once per frame
    private void Start()
    {
        animSpeed = 1.0f;
    }
    void Update()
    {
        if (fireArm == null)
        {
            fireArm = transform.parent.transform.parent.GetComponent<Firearm>();
            return;
        }
        if (fireArm != null)
        {
            anim.SetFloat("AmmoCount", fireArm.currentAmmo);
            if (anim2 != null)
            {
                if (fireArm.isShooting == true)
                {

                    if (animSpeed < 5)
                    {
                        animSpeed += Time.deltaTime * 20;
                    }
                }
                else
                {
                    if (animSpeed > 1)
                    {
                        animSpeed-=Time.deltaTime * 8;
                    }
                    
                }
                anim2.SetFloat("Speed",animSpeed);
            }
        }
    }
}
