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
    public int identity;

    private void Start()
    {
        health = maxHealth;
        instance = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }

    

    [PunRPC]
    public void EnemyTakeDamage(float value)
    {
        health -= value;
        Debug.Log("Took damage!");
        if (health <= minHealth)
        {
            instance.enemies.Remove(instance.enemies[identity]);
            instance.ReassignIdentities();
            PhotonNetwork.Destroy(gameObject);
            
        }
    }
}
