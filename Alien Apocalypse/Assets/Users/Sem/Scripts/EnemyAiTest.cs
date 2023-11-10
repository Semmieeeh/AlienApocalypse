using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using static PlayerHealth;

public class EnemyAiTest : MonoBehaviourPunCallbacks
{
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
    public float flyingHeight;
    [Header("Sounds")]
    public AudioSource source;
    public AudioClip[] clips;
    [Header("Particles")]
    public ShootEffect bulletTracer;
    public VisualEffect flash;
    public float particleMissOffset;


    public enum EnemyState
    {
        idle,
        chasing
    }
    public EnemyState state;
    public float attackRange;
    public float targetRange;
    public GameObject nearestPlayer;
    public bool canAttack;
    public float attackSpeed;
    public float attackDamage;
    public float timePassed;
    RaycastHit hit;
    public bool flyingEnemy;
    public Animator anim;
    public Animator armAnim;
    public List<Beacon> beaconlist = new List<Beacon>();
    public HashSet<Beacon> uniqueBeacons = new HashSet<Beacon>();
    public bool isBomber;
    void Start()
    {
        targetRange = 10;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        Beacon[] beacons = FindObjectsOfType<Beacon>();
        for(int  i = 0; i < beacons.Length; i++)
        {
            int j = Random.Range(0, beacons.Length);
            origin = beacons[j].transform.position;
            roamRange = beacons[j].radius;
        }
        target = transform.position;
        state = EnemyState.idle;
        
        
        if (flyingEnemy)
        {
            Offset();
            
        }
        if (flyingEnemy)
        {
            agent.stoppingDistance = 0;
        }
        else
        {
            agent.stoppingDistance = targetRange - 4;
        }
        NewTarget();
        anim.SetBool("Flying", flyingEnemy);
    }
    private float firstOffset;
    float currentOffset;
    float desiredOffset;
    public void Offset()
    {
        firstOffset = Random.Range(4f, 8f);
        agent.baseOffset = firstOffset;
        attackRange += agent.baseOffset;
    }
    bool canChooseNew;
    void Update()
    {
        
        if (canChooseNew == true)
        {
            NewTarget();
            canChooseNew = false;
        }
        if (photonView.IsMine)
        {
            if (canAttack == false)
            {
                timePassed += Time.deltaTime;
            }

            if (timePassed > attackSpeed)
            {
                canAttack = true;
                timePassed = Random.Range(-attackSpeed, attackSpeed);
            }
            if (photonView.IsMine)
            {
                CheckForPlayer();
                switch (state)
                {
                    case EnemyState.idle:

                        if (flyingEnemy)
                        {
                            agent.stoppingDistance = 0;
                        }
                        else
                        {
                            agent.stoppingDistance = targetRange;
                        }
                        if (agent != null)
                        {
                            agent.destination = target;
                        }
                        photonView.RPC(nameof(UpdateAlienLegs), RpcTarget.All, 1);
                        if (Vector3.Distance(transform.position, target) <= targetRange -1)
                        {
                            NewTarget();

                        }
                        if (flyingEnemy)
                        {
                            currentOffset = agent.baseOffset;
                            float newOffset = Mathf.Lerp(currentOffset, firstOffset, Time.deltaTime * moveSpeed/4);
                            agent.baseOffset = newOffset;
                        }
                        break;

                    case EnemyState.chasing:

                        if (flyingEnemy)
                        {
                            
                            currentOffset = agent.baseOffset;
                            desiredOffset = nearestPlayer.transform.position.y + firstOffset;
                            float newOffset = Mathf.Lerp(currentOffset, desiredOffset, Time.deltaTime * moveSpeed / 4);
                            agent.baseOffset = newOffset;
                        }

                        if (nearestPlayer != null && agent!=null)
                        {
                            agent.destination = nearestPlayer.transform.position;
                            agent.stoppingDistance = attackRange;

                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange)
                            {
                                photonView.RPC(nameof(UpdateAlienLegs), RpcTarget.All, 0);
                                photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, 0, null, true);

                                Quaternion targetRotation = CalculateRotationToPlayer();
                                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                                if (canAttack == true)
                                {

                                    photonView.RPC("Attack", RpcTarget.All, attackDamage);

                                }

                            }
                            else
                            {
                                canChooseNew = true;
                                if (!flyingEnemy)
                                {
                                    photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, 1, null, false);
                                    photonView.RPC(nameof(UpdateAlienLegs), RpcTarget.All, 2);
                                }
                            }

                            if (nearestPlayer.GetComponent<PlayerHealth>().knocked == true)
                            {
                                state = EnemyState.idle;

                            }
                        }
                        break;
                }
            }
        }
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

    [PunRPC]
    public void Attack(float damage)
    {
        if (nearestPlayer != null)
        {
            if (nearestPlayer.TryGetComponent(out PlayerHealth player))
            {
                if (isBomber)
                {
                    EnemyHealth h = GetComponent<EnemyHealth>();
                    player.TakeDamage(damage);
                    h.Explode();
                    Debug.Log("Exploded");
                }
                if (nearestPlayer.TryGetComponent(out Movement m) && !isBomber)
                {
                    if (m.rb.velocity.magnitude <= 0f)
                    {
                        source.clip = clips[0];
                        source.Play();
                        photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, null, "Attack", null);
                        flash.Play();
                        Vector3 j = player.transform.position;
                        j.y += 0.4f;
                        bulletTracer.Activate(true, j);
                        player.TakeDamage(damage);
                        canAttack = false;
                        Debug.Log("Hit!");
                    }
                    else if (m.rb.velocity.magnitude > 0f)
                    {
                        source.clip = clips[0];
                        source.Play();
                        canAttack = false;
                        flash.Play();
                        int k = Random.Range(0, m.rb.velocity.magnitude.ToInt());
                        Debug.Log(k);
                        if (k < 5)
                        {
                            photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, null, "Attack", null);
                            player.TakeDamage(damage);
                            Vector3 v = player.transform.position;
                            bulletTracer.Activate(true, v);
                            Debug.Log("Hit By Chance!");
                        }
                        else
                        {
                            photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, null, "Attack", null);
                            Vector3 v = new Vector3(Random.Range(player.transform.position.x - particleMissOffset, player.transform.position.x + particleMissOffset), Random.Range(player.transform.position.y - particleMissOffset, player.transform.position.y + particleMissOffset), Random.Range(player.transform.position.z - particleMissOffset, player.transform.position.z + particleMissOffset));
                            v *= 10;
                            bulletTracer.Activate(true, v);
                            Debug.Log("Missed!");
                        }

                    }
                }
            }
        }
    }

    public void CheckForPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float nearestDistance = float.MaxValue;
        nearestPlayer = null;

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
                        state = EnemyState.chasing;
                    }

                }
            }
            else
            {
                state = EnemyState.idle;

            }
        }
    } 

    [PunRPC]
    private void UpdateAlienArms(int state, string attackTrigger, bool inRange)
    {
        armAnim.SetInteger("WalkState",state);
        armAnim.SetTrigger(attackTrigger);  
        armAnim.SetBool("InRange", inRange);
    }

    [PunRPC]
    void UpdateAlienLegs(int i)
    {
        anim.SetInteger("WalkState", i);
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