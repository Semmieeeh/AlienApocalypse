using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbility : MonoBehaviour
{
    const string kActive = "Active";

    public float cooldown;

    float currentCooldown;

    [SerializeField]
    Animator animator;

    [SerializeField]
    Image progressImage;

    [SerializeField]
    bool test;

    public bool Active
    {
        get
        {
            return animator.GetBool (kActive );
        }
        set
        {
            animator.SetBool (kActive, value);
        }
    }

    private void Update ( )
    {
        if ( test )
        {
            test = false;

            Activate ( );
        }


        if ( !Active )
            return;

        currentCooldown += Time.deltaTime;

        progressImage.fillAmount = Mathf.InverseLerp (0,cooldown, currentCooldown );

        if(currentCooldown >= cooldown )
        {
            Disable ( );
        }
    }

    void Disable ( )
    {
        Active = false;
    }

    public void Activate ( )
    {
        Active = true;
        currentCooldown = 0;
    }
}
