using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyManager instance;
    public float health;
    public float maxHealth = 100;
    public GameObject gettingShotBy;
    public float xpAmount;

    private void Start()
    {
        health = maxHealth;
    }  

    [PunRPC]
    public void EnemyTakeDamage(float value)
    {
        health -= value;
        Debug.Log("Took damage!");

        if (health <= 0)
        {


            Destroy(gameObject);
            
        }
    }
}
