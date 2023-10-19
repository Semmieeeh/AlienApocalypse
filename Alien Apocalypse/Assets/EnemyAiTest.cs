using JetBrains.Annotations;
using Photon.Pun;
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
    public GameObject nearestPlayer;
    public bool canAttack;
    public float attackSpeed;
    public float attackDamage;
    public float timePassed;
    RaycastHit hit;
    public bool flyingEnemy;
    public Animator anim;
    public Animator armAnim;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        origin = transform.position;
        target = transform.position;
        agent.destination = target;
        state = EnemyState.idle;
        agent.stoppingDistance = attackRange;
        NewTarget();
        if (flyingEnemy)
        {
            Offset();
        }
    }
    [PunRPC]
    public void Offset()
    {
        agent.baseOffset = Random.Range(4f, 8f);
        attackRange += agent.baseOffset;
    }
    bool canChooseNew;
    void Update()
    {

        photonView.RPC("AiSyncUpdate",RpcTarget.All);
    }

    [PunRPC]
    void AiSyncUpdate()
    {
        if (canChooseNew == true)
        {
            NewTarget();
            canChooseNew = false;
        }
        if (photonView.IsMine)
        {
            if (flyingHeight > 0.1f)
            {
                Physics.Raycast(transform.position, -transform.up, out hit);
                Transform flight = hit.transform;
                Vector3 flightTransform = new Vector3(flight.position.x, flight.position.y + 10, flight.position.z);
                transform.position = flightTransform;
            }
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

                        photonView.RPC(nameof(UpdateAlienLegs), RpcTarget.All, 1);
                        if (Vector3.Distance(transform.position, target) <= attackRange)
                        {
                            NewTarget();

                        }
                        break;

                    case EnemyState.chasing:

                        if (nearestPlayer != null)
                        {
                            agent.destination = nearestPlayer.transform.position;

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
                                photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, 1, null, false);
                                photonView.RPC(nameof(UpdateAlienLegs), RpcTarget.All, 2);
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
        return navHit.position;
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
        if (nearestPlayer.TryGetComponent(out PlayerHealth player))
        {
            if (nearestPlayer.TryGetComponent(out Movement m))
            {
                if (m.rb.velocity.magnitude <=0f)
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
                else if(m.rb.velocity.magnitude >0f)
                {
                    source.clip = clips[0];
                    source.Play();
                    canAttack = false;
                    flash.Play();
                    int k = Random.Range(0, m.rb.velocity.magnitude.ToInt());
                    Debug.Log(k);
                    if (k  < 5)
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