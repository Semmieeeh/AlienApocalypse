using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;

    public Grappling grappling;

    [Header("Level Data")]
    public int currentLevel;
    public float currentExperince;

    public int standardExpNeeded;
    public float expModifier;

    [Header("Dont Touch This Motherfucker!")]
    public float expNeededForLevelUp;
    public int levelUps = 0;
    public int potentialLevelArmour = 0;
    public int potentialLevelReassembly = 0;
    public int potentialLevelWeaponHandeling = 0;

    [Header("Levels Stuff")]
    public int armourPlating;
    public int reassembling;
    public int weaponHandeling;

    [Header("Ability Data")]
    public int abilityPoints;

    public void ApplyLevelUp()
    {
        if(currentExperince >= expNeededForLevelUp)
        {
            Debug.Log("Level Up");

            currentLevel += levelUps;
            currentExperince -= expNeededForLevelUp;
            
            armourPlating += potentialLevelArmour;
            reassembling += potentialLevelReassembly;
            weaponHandeling += potentialLevelWeaponHandeling;

            levelUps = 0;
            expNeededForLevelUp = 0;

            potentialLevelArmour = 0;
            potentialLevelReassembly = 0;
            potentialLevelWeaponHandeling = 0;
        }
    }

    public void LevelUpButton()
    {
        levelUps++;

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }
    }

    public void LevelDownButton()
    {
        levelUps--;

        if(levelUps <= 0)
        {
            levelUps = 0;
        }

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }
    }

    public void ArmourLevel(int num)
    {
        potentialLevelArmour += num;

        if(potentialLevelArmour < 0)
        {
            potentialLevelArmour = 0;
        }
    }

}