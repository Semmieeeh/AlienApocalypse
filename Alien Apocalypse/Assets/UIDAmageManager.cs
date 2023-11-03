using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDAmageManager : MonoBehaviour
{
    public static UIDAmageManager instance;
    public Animator damageAnimator;

    private void Awake ( )
    {
        instance = this;
    }

    public void Damage ( )
    {
        damageAnimator.SetTrigger ("Damage");
    }
}
