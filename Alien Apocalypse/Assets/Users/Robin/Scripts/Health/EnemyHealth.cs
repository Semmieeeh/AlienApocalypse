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
    public Rigidbody[] rigidBodies;


    UnityEvent onKill;
    UnityEvent onHit;

    private void Start()
    {
        maxHealth = maxHealth * multiplier;
        health = maxHealth;
        
    }

    public Rigidbody hitLimb;
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
            
            foreach(var rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = false;               
            }
            GetComponent<EnemyAiTest>().anim.enabled = false;
            GetComponent<EnemyAiTest>().armAnim.enabled = false;
            GetComponent<EnemyAiTest>().enabled = false;
            if (hitLimb != null)
            {
                hitLimb.AddForce(Camera.main.transform.forward * 10,ForceMode.Impulse);
            }
            else
            {
                foreach(var rigidBody in rigidBodies)
                {
                    rigidBody.AddForce(Camera.main.gameObject.transform.forward * 15, ForceMode.Impulse);
                }

            }
            StartCoroutine(nameof(Die));
        }
        else
        {
            onHit?.Invoke();
        }
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(10);
        photonView.RPC("DieVoid", RpcTarget.All);
    }
    [PunRPC]
    void DieVoid()
    {
        Destroy(gameObject);
    }
}
