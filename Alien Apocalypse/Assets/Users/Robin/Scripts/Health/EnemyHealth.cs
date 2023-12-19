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
        //photonView.RPC("Shader", RpcTarget.All);

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
            dead = true;


            onKill?.Invoke();
            
            
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
                hitLimb.AddForce(blastDirection * knockBack, ForceMode.Impulse);
                
            }
            else
            {
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
    bool died;
    public GameObject explosion;
    public GameObject explodeObj;
    bool exploded = false;
    GameObject exp;
    public void Explode()
    {
        photonView.RPC("ExplodeRPC", RpcTarget.All);
    }
    [PunRPC]
    public void ExplodeRPC()
    {
        health = 0;
        SyncDamage(10);
    }
    public Animator anim;
    public IEnumerator Die()
    {
        PhotonNetwork.Destroy(gun);
        
        
        if (exploded == false && canExplode)
        {
            
            exp = PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
            
            exp.transform.parent = transform;
            exploded = true;
        }
        yield return new WaitForSeconds(10);
        died = true;
        anim.SetBool("Dead", died);
        yield return new WaitForSeconds(1);
        if (canExplode == true)
        {
            
            photonView.RPC("DieWithExplosion", RpcTarget.All);
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
    void DieWithExplosion()
    {
        if (exp != null)
        {
            PhotonNetwork.Destroy(exp);
            
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
