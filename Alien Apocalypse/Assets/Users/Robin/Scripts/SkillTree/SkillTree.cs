using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public WeaponInputHandler weaponInputHandler;

    public Grappling grappling;

    [Header("Other")]
    public KeyCode input;
    public GameObject holder;

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
    public float healthModifier;

    public int reassembling;
    public float regenModifier;

    public int weaponHandeling;
    public float damageModifier;

    [Header("Ability Data")]
    public int abilityPoints;

    [Header("UI")]
    public TMP_Text currentLevelText;
    public TMP_Text currentExpText;
    public TMP_Text expNeededText;

    private void Start()
    {
        currentLevelText.text = $"Lvl: {currentLevel}";
        currentExpText.text = $"Exp: {currentExperince}";
        expNeededText.text = $"Cost: {0}";
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

    public void AddExp(float exp)
    {
        currentExperince += exp;
        currentExpText.text = $"Exp: {currentExpText}";
    }

    public void ApplyLevelUp()
    {
        if(currentExperince >= expNeededForLevelUp)
        {
            Debug.Log("Level Up");

            currentLevel += levelUps;
            currentExperince -= expNeededForLevelUp;
            
            armourPlating += potentialLevelArmour;
            playerHealth.maxHealth *= (healthModifier + 1);

            reassembling += potentialLevelReassembly;
            playerHealth.healthRegenAmount *= regenModifier + 1;

            weaponHandeling += potentialLevelWeaponHandeling;
            weaponInputHandler.SetAbility(damageModifier, weaponHandeling);

            levelUps = 0;
            expNeededForLevelUp = 0;

            potentialLevelArmour = 0;
            potentialLevelReassembly = 0;
            potentialLevelWeaponHandeling = 0;

            currentLevelText.text = $"Lvl: {currentLevel}";
            currentExpText.text = $"Exp: {currentExperince}";
            expNeededText.text = $"Cost: {0}";
        }
    }

    public void ArmourLevel(int num)
    {
        levelUps += num;

        if(levelUps <= 0)
        {
            levelUps = 0;
        }

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }

        potentialLevelArmour += num;

        if(potentialLevelArmour < 0)
        {
            potentialLevelArmour = 0;
        }

        currentLevelText.text = $"Lvl: {currentLevel} > {currentLevel + levelUps}";
        expNeededText.text = $"Cost: {expNeededForLevelUp}";
    }

    public void RegenLevel(int num)
    {
        levelUps += num;

        if(levelUps <= 0)
        {
            levelUps = 0;
        }

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }

        potentialLevelReassembly += num;

        if(potentialLevelReassembly < 0)
        {
            potentialLevelReassembly = 0;
        }

        currentLevelText.text = $"Lvl: {currentLevel} > {currentLevel + levelUps}";
        expNeededText.text = $"Cost: {expNeededForLevelUp}";
    }

    public void HandelingLevel(int num)
    {
        levelUps += num;

        if(levelUps <= 0)
        {
            levelUps = 0;
        }

        expNeededForLevelUp = 0;
        for(int i = 0; i < levelUps; i++)
        {
            expNeededForLevelUp += (standardExpNeeded * ((currentLevel + levelUps) * expModifier + 1));
        }

        potentialLevelWeaponHandeling += num;

        if(potentialLevelWeaponHandeling < 0)
        {
            potentialLevelWeaponHandeling = 0;
        }

        currentLevelText.text = $"Lvl: {currentLevel} > {currentLevel + levelUps}";
        expNeededText.text = $"Cost: {expNeededForLevelUp}";
    }
}