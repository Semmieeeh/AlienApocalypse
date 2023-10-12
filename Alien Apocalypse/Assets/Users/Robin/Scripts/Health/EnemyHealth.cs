using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class EnemyHealth : MonoBehaviourPunCallbacks, IDamagable
{
    public EnemyManager instance;
    public float health;
    public float maxHealth;
    public GameObject gettingShotBy;
    public float xpAmount;
    public float multiplier;

    UnityEvent onKill;
    UnityEvent onHit;

    private void Start()
    {
        maxHealth = maxHealth * multiplier;
        health = maxHealth;
        
    }

    
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit)
    {
        this.onKill = onKill;
        this.onHit = onHit;

        photonView.RPC(nameof(SyncDamage), RpcTarget.All, damage);
        Debug.Log(damage);
    }

    [PunRPC]
    public void SyncDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            onKill?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            onHit?.Invoke();
        }
    }
}
