using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerup : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    Image powerupImage;

    [SerializeField]
    public FirearmAbility currentAbility;
    
    public void Initialize(FirearmAbility ability)
    {
        powerupImage.sprite = ability.uiSprite;
    }

    public void Blink ( )
    {
        animator.SetTrigger ("Blink");
    }
}
