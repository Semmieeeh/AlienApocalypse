using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillTree skillTree;

    public DashAbility dash;
    public WallRunning wall;
    public SlidingAbility slide;
    public Grappling grappling;

    public bool unlocked;
    public Skill nextSkill;

    public void UnlockSkill()
    {
        if(unlocked && skillTree.abilityPoints >= 1)
        {
            if(dash != null)
            {
                dash.unlockedSkill = true;   
            }
            else if(wall != null)
            {
                wall.unlockedSkill = true;
            }
            else if(slide != null)
            {
                slide.unlockedSkill = true;    
            }
            else if(grappling != null)
            {
                grappling.unlockedSkill = true;
            }

            if(nextSkill != null)
            {
                nextSkill.unlocked = true;
            }
            Debug.Log("Unlocked " + gameObject.name);
            skillTree.abilityPoints--;
            skillTree.abilityPointsText.text = $"{skillTree.abilityPOintsString} {skillTree.abilityPoints}";
        }
    }
}