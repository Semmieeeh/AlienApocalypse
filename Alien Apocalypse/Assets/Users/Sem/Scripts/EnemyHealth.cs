using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyManager instance;
    public float health;
    public float maxHealth = 100;
    public float minHealth = 0;
    public GameObject gettingShotBy;
    public float xpAmount;
    public string unlocksSkill;

    private void Start()
    {
        health = maxHealth;
    }
    


    [PunRPC]
    public void EnemyTakeDamage(float value)
    {
        health -= value;
        Debug.Log("Took damage!");
        if (health <= minHealth)
        {
            if(gettingShotBy != null)
            {
                if (unlocksSkill == "Wallrunning" && gettingShotBy != null)
                {
                    gettingShotBy.GetComponent<WallRunning>().unlockedSkill = true;
                }

                if (unlocksSkill == "Dash" && gettingShotBy != null)
                {
                    gettingShotBy.GetComponent<DashAbility>().unlockedSkill = true;
                }

                if (unlocksSkill == "GrapplingHook" && gettingShotBy != null)
                {
                    gettingShotBy.GetComponent<Grappling>().unlockedSkill = true;
                }
            }

            Destroy(gameObject);
            
        }
    }
}
