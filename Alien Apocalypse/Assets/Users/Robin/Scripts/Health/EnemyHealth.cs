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

    UnityEvent onKill;
    UnityEvent onHit;

    private void Start()
    {
        health = maxHealth;
        PhotonNetwork.SerializationRate = 10;
    }

    
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit)
    {
        this.onKill = onKill;
        this.onHit = onHit;

        photonView.RPC(nameof(SyncDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    void SyncDamage(float damage)
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
