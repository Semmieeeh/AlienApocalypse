using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTree : MonoBehaviour
{
    [Header("References")]
    public static SkillTree instance;
    public PlayerHealth playerHealth;
    public WeaponInputHandler weaponInputHandler;

    public Grappling grappling;

    [Header("Other")]
    public KeyCode input;
    public GameObject holder;

    [Header("Level Data")]
    public int currentLevel;
    public float currentExperience;

    public int standardExpNeeded;
    public float expModifier;

    [Header("Dont Touch This Motherfucker!")]
    public float expNeededForLevelUp;
    public int levelUps = 0;
    public int potentialLevelArmour = 0;
    public int potentialLevelReassembly = 0;
    public int potentialLevelWeaponHandeling = 0;

    [Header("Levels Stuff")]
    public int armourPlatingLvl;
    public float healthModifier;

    public int reassemblingLvl;
    public float regenModifier;

    public int weaponHandelingLvl;
    public float damageModifier;

    [Header("Ability Data")]
    public float abilityPoints;

    [Header("UI")]
    public TMP_Text abilityPointsText;
    public string abilityPOintsString;
    [Space]
    public TMP_Text currentLevelText;
    public string currentLevelString;
    [Space]
    public TMP_Text currentExpText;
    public string currentExpString;
    [Space]
    public TMP_Text expNeededText;
    public string expNeededString;
    [Space]
    public TMP_Text armourLvlText;
    public string armourLvlString;
    [Space]
    public TMP_Text reassemblingLvlText;
    public string reassemblingLvlString;
    [Space]
    public TMP_Text weaponHandelingLvlText;
    public string weaponHandelingLvlString;

    private void Start()
    {
        instance = this;

        abilityPointsText.text = $"{abilityPOintsString} {abilityPoints}";

        currentLevelText.text = $"{currentLevelString} {currentLevel}";
        currentExpText.text = $"{currentExpString} {currentExperience}";
        expNeededText.text = $"{expNeededString} {0}";

        armourLvlText.text = $"{armourLvlString} {armourPlatingLvl}";
        reassemblingLvlText.text = $"{reassemblingLvlString} {reassemblingLvl}";
        weaponHandelingLvlText.text = $"{weaponHandelingLvlString} {weaponHandelingLvl}";
    }

    void Update()
    {
        if(Input.GetKeyDown(input))
        {
            bool active = holder.activeSelf;
            holder.SetActive(!active);

            if(!active)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else if(active)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public static void AddExp(float exp)
    {
        instance.currentExperience += exp;
        instance.currentExpText.text = $"{instance.currentExpString} {instance.currentExpText}";
    }

    public static void AddAbilityPoint(float point)
    {
        instance.abilityPoints += point;
        instance.abilityPointsText.text = $"{instance.abilityPOintsString} {instance.abilityPoints}";
    }

    public void ApplyLevelUp()
    {
        if(currentExperience >= expNeededForLevelUp)
        {
            currentLevel += levelUps;
            currentExperience -= expNeededForLevelUp;
            
            armourPlatingLvl += potentialLevelArmour;
            //playerHealth.maxHealth *= (healthModifier + 1);

            reassemblingLvl += potentialLevelReassembly;
            //playerHealth.healthRegenAmount *= regenModifier + 1;

            weaponHandelingLvl += potentialLevelWeaponHandeling;
            //weaponInputHandler.SetAbility(damageModifier, weaponHandeling);

            levelUps = 0;
            expNeededForLevelUp = 0;

            potentialLevelArmour = 0;
            potentialLevelReassembly = 0;
            potentialLevelWeaponHandeling = 0;

            currentLevelText.text = $"{currentLevelString} {currentLevel}";
            currentExpText.text = $"{currentExpString} {currentExperience}";
            expNeededText.text = $"{expNeededString} {0}";

            armourLvlText.text = $"{armourLvlString} {armourPlatingLvl}";
            reassemblingLvlText.text = $"{reassemblingLvlString} {reassemblingLvl}";
            weaponHandelingLvlText.text = $"{weaponHandelingLvlString} {weaponHandelingLvl}";
        }
    }

    public void ArmourLevel(int num)
    {
        potentialLevelArmour += num;

        if(potentialLevelArmour < 0)
        {
            potentialLevelArmour = 0;
        }
        else
        {
            levelUps += num;

            if(levelUps <= 0)
            {
                levelUps = 0;
            }
        }

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }

        if(levelUps == 0)
        {
            currentLevelText.text = $"{currentLevelString} {currentLevel}";
            currentExpText.text = $"{currentExpString} {currentExperience}";
            expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";

        }
        else
        {
            currentLevelText.text = $"{currentLevelString} {currentLevel} > {currentLevel + levelUps}";
            currentExpText.text = $"{currentExpString} {currentExperience} > {currentExperience - expNeededForLevelUp}";
            expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";
        }

        if(potentialLevelArmour == 0)
        {
            armourLvlText.text = $"{armourLvlString} {armourPlatingLvl}";
        }
        else
        {
            armourLvlText.text = $"{armourLvlString} {armourPlatingLvl} > {armourPlatingLvl + potentialLevelArmour}";
        }
    }

    public void RegenLevel(int num)
    {
        potentialLevelReassembly += num;

        if(potentialLevelReassembly < 0)
        {
            potentialLevelReassembly = 0;
        }
        else
        {
            levelUps += num;

            if(levelUps <= 0)
            {
                levelUps = 0;
            }
        }

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }

        if(levelUps == 0)
        {
            currentLevelText.text = $"{currentLevelString} {currentLevel}";
            currentExpText.text = $"{currentExpString} {currentExperience}";
            expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";

        }
        else
        {
            currentLevelText.text = $"{currentLevelString} {currentLevel} > {currentLevel + levelUps}";
            currentExpText.text = $"{currentExpString} {currentExperience} > {currentExperience - expNeededForLevelUp}";
            expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";
        }

        if(potentialLevelReassembly == 0)
        {
            reassemblingLvlText.text = $"{reassemblingLvlString} {reassemblingLvl}";
        }
        else
        {
            reassemblingLvlText.text = $"{reassemblingLvlString} {reassemblingLvl} > {reassemblingLvl + potentialLevelReassembly}";
        }
    }

    public void HandelingLevel(int num)
    {
        potentialLevelWeaponHandeling += num;

        if(potentialLevelWeaponHandeling < 0)
        {
            potentialLevelWeaponHandeling = 0;
        }
        else
        {
            levelUps += num;

            if(levelUps <= 0)
            {
                levelUps = 0;
            }
        }        

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }

        if(levelUps == 0)
        {
            currentLevelText.text = $"{currentLevelString} {currentLevel}";
            currentExpText.text = $"{currentExpString} {currentExperience}";
            expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";

        }
        else
        {
            currentLevelText.text = $"{currentLevelString} {currentLevel} > {currentLevel + levelUps}";
            currentExpText.text = $"{currentExpString} {currentExperience} > {currentExperience - expNeededForLevelUp}";
            expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";
        }

        if(potentialLevelWeaponHandeling == 0)
        {
            weaponHandelingLvlText.text = $"{weaponHandelingLvlString} {weaponHandelingLvl}";
        }
        else
        {
            weaponHandelingLvlText.text = $"{weaponHandelingLvlString} {weaponHandelingLvl} > {weaponHandelingLvl + potentialLevelWeaponHandeling}";
        }
    }

    public void Ability()
    {

    }
}