using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public enum AbilityType
    {
        player,
        firearm,
        melee,
    }
    public AbilityType abilityType;
}
