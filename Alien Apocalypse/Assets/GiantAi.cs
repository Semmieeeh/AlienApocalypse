using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using static PlayerHealth;

public class GiantAi : MonoBehaviour
{
    public bool isEnclave;
    [Header("Ai Modifiers")]
    public float moveSpeed;
    public float turnSpeed;
    private NavMeshAgent agent;
    private Vector3 origin;
    public float roamRange;

    [Space]
    [Header("Current Target")]
    public Vector3 target;
    [Header("Player")]
    public float angleToPlayer;
    public float detectionAngle;
    public float detectionRange;
    [Header("Sounds")]
    public AudioSource source;
    public AudioClip[] clips;

    public enum EnemyState
    {
        idle,
        chasing,
    }
    public EnemyState state;
    public float attackRange;
    public float targetRange;
    public GameObject nearestPlayer;
    public bool canAttack;
    public float attackSpeed;
    public float attackDamage;
    public Animator anim;
    public List<Beacon> beaconlist = new List<Beacon>();
    public HashSet<Beacon> uniqueBeacons = new HashSet<Beacon>();
    public bool isBomber;
    public float animationtime;
    public int giant;
    void Start()
    {
        targetRange = 10;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        Beacon[] beacons = FindObjectsOfType<Beacon>();
        for (int i = 0; i < beacons.Length; i++)
        {
            int j = Random.Range(0, beacons.Length);
            origin = beacons[j].transform.position;
            roamRange = beacons[j].radius;
        }
        target = transform.position;
        state = EnemyState.idle;

        NewTarget();
    }

    private void Update()
    {
        anim.SetInteger("GiantState", giant);
        attackSpeed -= Time.deltaTime;
        if(attackSpeed <= 0)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }

        switch (state)
        {
            case EnemyState.idle:
                giant = 1;
                if (Vector3.Distance(transform.position, target) <= targetRange - 1)
                {
                    NewTarget();

                }
                break;
            case EnemyState.chasing:
                giant = 1;
                agent.destination = nearestPlayer.transform.position;
                if(Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange + 0.2f)
                {
                    giant = 0;
                    if (canAttack)
                    {
                        
                        StartCoroutine(nameof(Attack),attackDamage);
                        
                        canAttack = false;
                    }
                }
                break;

        }
        CheckForPlayer();
    }


    public Vector3 NewDestination(Vector3 origin, float dist, int layerMask)
    {
        NavMeshHit navHit;
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out navHit, roamRange, layerMask);
        Vector3 v = navHit.position;
        v.y = transform.position.y;
        return v;
    }

    public void NewTarget()
    {
        if (agent != null)
        {
            target = NewDestination(origin, roamRange, -1);
            agent.destination = target;
        }
    }

    public IEnumerator Attack(float damage)
    {
        if (!isEnclave)
        {
            attackSpeed = 5;
            float kb;
            moveSpeed = 0;
            agent.speed = moveSpeed;
            anim.SetTrigger("Attack");
            int i = Random.Range(0, 10);
            if (i == 0)
            {
                animationtime = 1f;
                kb = 20;
            }
            else
            {
                animationtime = 0.25f;
                kb = 10;
            }
            anim.SetInteger("AttackInt", i);

            yield return new WaitForSeconds(animationtime);

            PlayerHealth player = nearestPlayer.GetComponent<PlayerHealth>();
            player.TakeDamage(damage);
            player.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * kb, ForceMode.Impulse);
            player.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * kb / 2, ForceMode.Impulse);
            HitIndicatorManager.Instance.AddTarget(transform);
            attackSpeed = 2;
            canAttack = false;
            yield return new WaitForSeconds(1f);
            moveSpeed = 2;
            agent.speed = moveSpeed;
        }
        else
        {
            attackSpeed = 1;
            PlayerHealth player = nearestPlayer.GetComponent<PlayerHealth>();
            player.TakeDamage(damage);

        }
        
    }

    public void CheckForPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float nearestDistance = float.MaxValue;

        foreach (GameObject player in players)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 targetDir = playerPos - transform.position;
            angleToPlayer = Vector3.Angle(targetDir, transform.forward);

            if (Vector3.Distance(transform.position, playerPos) < detectionRange && angleToPlayer < detectionAngle)
            {

                if (Vector3.Distance(transform.position, playerPos) < nearestDistance)
                {
                    nearestDistance = Vector3.Distance(transform.position, playerPos);
                    nearestPlayer = player;
                    if (nearestPlayer.GetComponent<PlayerHealth>().state == PlayerState.alive)
                    {
                        if (state != EnemyState.chasing)
                        {
                            state = EnemyState.chasing;
                        }
                    }

                }
                else
                {
                    nearestPlayer = null;

                }
            }
            else
            {
                if (state != EnemyState.idle)
                {
                    state = EnemyState.idle;
                }
            }
        }
    }


    private Quaternion CalculateRotationToPlayer()
    {
        if (nearestPlayer != null)
        {
            Vector3 directionToPlayer = nearestPlayer.transform.position - transform.position;
            directionToPlayer.y = 0;
            return Quaternion.LookRotation(directionToPlayer);
        }
        return transform.rotation;
    }

}