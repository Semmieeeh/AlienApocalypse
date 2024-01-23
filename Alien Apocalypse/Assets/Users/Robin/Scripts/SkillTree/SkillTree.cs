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
    public int abilityPoints;

    [Header("UI")]
    public TMP_Text currentLevelText;
    public string currentLevelString;
    [Space]
    public TMP_Text currentExpText;
    public string currentExpString;
    [Space]
    public TMP_Text expNeededText;
    public string expNeededString;

    private void Start()
    {
        instance = this;

        currentLevelText.text = $"{currentLevelString} {currentLevel}";
        currentExpText.text = $"{currentExpString} {currentExperience}";
        expNeededText.text = $"{expNeededForLevelUp} {0}";
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

        currentLevelText.text = $"{currentLevelString} {currentLevel} > {currentLevel + levelUps}";
        expNeededText.text = $"{expNeededString} {expNeededForLevelUp}";
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

        currentLevelText.text = $"{currentLevelString} {currentLevel} > {currentLevel + levelUps}";
        expNeededText.text = $"{expNeededString}  {expNeededForLevelUp}";
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

        currentLevelText.text = $"{currentLevelString} {currentLevel} > {currentLevel + levelUps}";
        expNeededText.text = $"{expNeededString}  {expNeededForLevelUp}";
    }

    public void Ability()
    {

    }
}