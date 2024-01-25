using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    public bool isGiant;
    public bool isEnclave;
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
        if (!isEnclave)
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
        if(TryGetComponent<EnemyAiTest>(out EnemyAiTest e))
        {
            e.enabled = true;
        }
        
    }

    public Rigidbody hitLimb;
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit, float b, Vector3 blastDir)
    {
        knockBack = b;
        this.onKill = onKill;
        this.onHit = onHit;
        blastDirection = blastDir;
        SyncDamage(damage);
    }
    bool dead;
    float time;
    private void Update()
    {
        Shader();

    }
    public void Shader()
    {
        if (died == true)
        {
            time += 1* Time.deltaTime;
            r.material.SetFloat("_Dead", time);
        }
    }
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
            if(TryGetComponent<EnemyAiTest>(out EnemyAiTest enemyTest))
            {
                enemyTest.anim.enabled = false;
                enemyTest.armAnim.enabled = false;
                enemyTest.enabled = false;
            }            

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
        ExplodeRPC();
    }
    public void ExplodeRPC()
    {
        health = 0;
        SyncDamage(10);
    }
    public Animator anim;
    public IEnumerator Die()
    {
        if (!isGiant && !isEnclave)
        {
            Destroy(gun);
            if(transform.GetChild(0).TryGetComponent<Animator>(out Animator anim))
            {
                anim.enabled = false;
            }

            if (exploded == false && canExplode)
            {

                exp = Instantiate(explosion, transform.position, Quaternion.identity);

                exp.transform.parent = transform;
                exploded = true;
            }
            yield return new WaitForSeconds(10);
            died = true;
            anim.SetBool("Dead", died);
            yield return new WaitForSeconds(1);
            if (canExplode == true)
            {
                DieWithExplosion();
            }
            else
            {
                DieVoid();
            }
        }
        else if(isGiant)
        {
            int random = Random.Range(1, 4);
            for(int i = 0; i <random; i++)
            {
                GameObject blob = Instantiate(enemyBlobs, spawnPos.position, spawnPos.rotation);
                Vector3 torque = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
                Rigidbody rb = blob.GetComponent<Rigidbody>();
                rb.AddTorque(torque * 2, ForceMode.Impulse);
                rb.AddForce(transform.up * 10, ForceMode.Impulse);
                rb.AddForce(transform.forward * 4, ForceMode.Impulse);

                Quaternion rot = spawnPos.rotation;
                rot.y += 90;
                spawnPos.rotation = rot;
            }
            

            Destroy(gameObject);
        }
        else if(isEnclave)
        {
            Destroy(gameObject);
        }
        
    }
    public Transform spawnPos;

    public GameObject enemyBlobs;
    void DieVoid()
    {
        SkillTree.AddExp(xpAmount);
        Destroy(gameObject);
        
    }
    void DieWithExplosion()
    {
        if (exp != null)
        {
            Destroy(exp);
            
        }
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isEnclave && collision.gameObject.tag == "Ground")
        {
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<GiantAi>().enabled = true;
        }
    }
}
