using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float minHealth;
    public enum PlayerState
    {
        alive,
        downed,
        dead
    }
    public PlayerState state;
    void Start()
    {
        minHealth = 0;
        maxHealth = 100;
        health = maxHealth;
        state = PlayerState.alive;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PlayerState.alive:

                break; 
            case PlayerState.downed:

                break;
            case PlayerState.dead:

                break;
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        //update health text object
        health -= damage;
        //Debug.Log(health);
    }
    public void Die()
    {

    }
}
