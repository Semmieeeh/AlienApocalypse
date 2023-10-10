using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private float timePassed;
    RaycastHit hit;
    public bool flyingEnemy;
    public Animator anim;
    public Animator armAnim;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
            agent.angularSpeed = turnSpeed;
            origin = transform.position;
            
            agent.destination = target;
            state = EnemyState.idle;
            agent.stoppingDistance = attackRange + 1;
            photonView.RPC("NewTarget", RpcTarget.All);
            if (flyingEnemy)
            {
                photonView.RPC("Offset", RpcTarget.All);
            }
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
        
        if(canChooseNew == true)
        {
            photonView.RPC("NewTarget", RpcTarget.All);
            canChooseNew = false;
        }
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
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
                timePassed = 0;
            }
            if (photonView.IsMine)
            {
                CheckForPlayer();
                switch (state)
                {
                    case EnemyState.idle:

                        anim.SetInteger("WalkState", 1);
                        if (Vector3.Distance(transform.position, target) < 3f)
                        {
                            photonView.RPC("NewTarget", RpcTarget.All);
                            
                        }
                        break;

                    case EnemyState.chasing:
                        
                        if (nearestPlayer != null)
                        {
                            agent.destination = nearestPlayer.transform.position;
                            
                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange)
                            {
                                anim.SetInteger("WalkState", 0);
                                
                                photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, 0, null, true);
                                agent.updateRotation = true;
                                if (canAttack == true)
                                {
                                    
                                    photonView.RPC("Attack", RpcTarget.All, attackDamage);
                                    
                                }

                            }
                            else
                            {
                                canChooseNew = true;
                                photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, 1, null, false);
                                anim.SetInteger("WalkState", 2);
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

    [PunRPC]
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
        if (PhotonNetwork.IsMasterClient)
        {
            if (nearestPlayer.TryGetComponent(out PlayerHealth player))
            {
                photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, null, "Attack", null);
                player.TakeDamage(damage);
                canAttack = false;
            }
        }
    }

    public void CheckForPlayer()
    {
        if (photonView.IsMine && PhotonNetwork.IsMasterClient)
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
                        if(nearestPlayer.GetComponent<PlayerHealth>().state == PlayerHealth.PlayerState.alive)
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
    }

    [PunRPC]
    private void UpdateAlienArms(int state, string attackTrigger, bool inRange)
    {
        armAnim.SetInteger("WalkState",state);
        armAnim.SetTrigger(attackTrigger);  
        armAnim.SetBool("InRange", inRange);
    }

    [PunRPC]
    void UpdateAlienLegs()
    {

    }

}