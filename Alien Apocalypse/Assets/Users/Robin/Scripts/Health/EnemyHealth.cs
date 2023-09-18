using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class EnemyHealth : MonoBehaviourPunCallbacks, IDamagable
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

    
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit)
    {
        photonView.RPC(nameof(Test), RpcTarget.All, damage);

        if(health <= 0)
        {
            onKill.Invoke();
        }
        else
        {
            onHit.Invoke();
        }
    }

    [PunRPC]
    void Test(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
