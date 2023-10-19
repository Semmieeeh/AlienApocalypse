using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPunCallbacks, IDamagable
{
    public EnemyManager instance;
    public float health;
    public float maxHealth;
    public GameObject gettingShotBy;
    public float xpAmount;
    public float multiplier;
    public Rigidbody[] rigidBodies;
    public float knockBack;
    public GameObject gun;


    UnityEvent onKill;
    UnityEvent onHit;

    private void Start()
    {
        maxHealth = maxHealth * multiplier;
        health = maxHealth;
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<EnemyAiTest>().enabled = true;
        }
        
    }

    public Rigidbody hitLimb;
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit, float b)
    {
        knockBack = b;
        this.onKill = onKill;
        this.onHit = onHit;

        photonView.RPC(nameof(SyncDamage), RpcTarget.All, damage);
        Debug.Log(damage);
    }
    bool dead;
    [PunRPC]
    public void SyncDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            if (!dead)
            {
                onKill?.Invoke();
            }
            dead = true;
            
            
            foreach(var rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = false;               
            }
            var enemyTest = GetComponent<EnemyAiTest>();
            enemyTest.anim.enabled = false;
            enemyTest.armAnim.enabled = false;
            enemyTest.enabled = false;

            if (hitLimb != null)
            {
                photonView.RPC(nameof(ReleaseGun), RpcTarget.All);
                hitLimb.AddForce(Camera.main.transform.forward * knockBack, ForceMode.Impulse);
                
            }
            else
            {
                photonView.RPC(nameof(ReleaseGun), RpcTarget.All);
                foreach (var rigidBody in rigidBodies)
                {
                    rigidBody.AddForce(Camera.main.gameObject.transform.forward * knockBack, ForceMode.Impulse);          
                }
                
            }
            StartCoroutine(nameof(Die));
        }
        else
        {
            if (!dead)
            {
                onHit?.Invoke();
            }
        }
    }
    [PunRPC]
    private void ReleaseGun()
    {
        StartCoroutine(nameof(GunRPC));
    }
    [PunRPC]
    IEnumerator GunRPC()
    {
        yield return new WaitForSeconds(0f);
        gun.transform.parent = null;
        Rigidbody gunRb = gun.GetComponent<Rigidbody>();
        gunRb.isKinematic = false;
        Vector3 rand = new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
        gunRb.AddForce(rand * 10,ForceMode.Impulse);
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(10);
        photonView.RPC("DieVoid", RpcTarget.All);
    }
    [PunRPC]
    void DieVoid()
    {
        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Destroy(gun);
    }
}
