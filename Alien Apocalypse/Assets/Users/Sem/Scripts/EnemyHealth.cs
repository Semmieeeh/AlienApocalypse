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
            Destroy(gameObject);
            
        }
    }
}
