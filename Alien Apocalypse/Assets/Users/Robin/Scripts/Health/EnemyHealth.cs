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
    public Renderer r;
    public Vector3 blastDirection;
    UnityEvent onKill;
    UnityEvent onHit;

    public bool canExplode;
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
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit, float b, Vector3 blastDir)
    {
        knockBack = b;
        this.onKill = onKill;
        this.onHit = onHit;
        blastDirection = blastDir;
        photonView.RPC(nameof(SyncDamage), RpcTarget.All, damage);
    }
    bool dead;
    float time;
    private void Update()
    {
        photonView.RPC("Shader", RpcTarget.All);

    }
    [PunRPC]
    public void Shader()
    {
        if (died == true)
        {
            time += 1* Time.deltaTime;
            r.material.SetFloat("_Dead", time);
        }
    }
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
            
            gameObject.layer = 0;
            foreach(var rigidBody in rigidBodies)
            {

                rigidBody.isKinematic = false;
                rigidBody.gameObject.layer = 0;
            }
            var enemyTest = GetComponent<EnemyAiTest>();
            enemyTest.anim.enabled = false;
            enemyTest.armAnim.enabled = false;
            enemyTest.enabled = false;

            if (hitLimb != null)
            {
                photonView.RPC(nameof(ReleaseGun), RpcTarget.All);
                hitLimb.AddForce(blastDirection * knockBack, ForceMode.Impulse);
                
            }
            else
            {
                photonView.RPC(nameof(ReleaseGun), RpcTarget.All);
                foreach (var rigidBody in rigidBodies)
                {
                    rigidBody.AddForce(blastDirection * knockBack, ForceMode.Impulse);          
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
        yield return new WaitForSeconds(0);
        if (gun != null)
        {
            PhotonNetwork.Destroy(gun);
        }
        if(explodeObj!= null)
        {
            PhotonNetwork.Destroy(explodeObj);
        }
  
    }
    bool died;
    public GameObject explosion;
    public GameObject explodeObj;
    bool exploded = false;
    GameObject exp;
    IEnumerator Die()
    {
        if (exploded == false && canExplode)
        {
            
            exp = PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
            exp.transform.parent = transform;
            exploded = true;
        }
        yield return new WaitForSeconds(10);
        died = true;
        yield return new WaitForSeconds(1);
        if (canExplode == true)
        {
            
            photonView.RPC("DieWithExplosion", RpcTarget.All, exp);
        }
        else
        {
            photonView.RPC("DieVoid", RpcTarget.All);
        }
        
    }
    [PunRPC]
    void DieVoid()
    {
    
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    void DieWithExplosion(GameObject expl)
    {
        if (expl != null)
        {
            PhotonNetwork.Destroy(exp);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
