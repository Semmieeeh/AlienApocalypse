using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerup : MonoBehaviour
{
    [SerializeField]
    Image powerupImage;

    [SerializeField]
    FirearmAbility currentAbility;
    
    public void Initialize(FirearmAbility ability)
    {
        powerupImage.sprite = ability.uiSprite;
    }
}
