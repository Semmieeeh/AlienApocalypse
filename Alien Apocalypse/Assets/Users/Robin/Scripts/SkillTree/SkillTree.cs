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
    public int currentExperince;
    public int skillPoints;
    public int abilityPoints;

    [Header("Levels Stuff")]
    public int maxHealth;
    public int healthRegeneration;
    public int damage;


}