using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTest : MonoBehaviourPunCallbacks, IPunObservable
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
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        origin = transform.position;
        photonView.RPC("NewTarget",RpcTarget.All);
        agent.destination = target;
        state = EnemyState.idle;
        attackRange = agent.stoppingDistance + 1;
        photonView.ObservedComponents.Add(this);

    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
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
                        if (Vector3.Distance(transform.position, target) < 3f)
                        {
                            photonView.RPC("NewTarget", RpcTarget.All);
                            
                        }
                        break;

                    case EnemyState.chasing:
                        if (nearestPlayer != null)
                        {
                            agent.destination = nearestPlayer.transform.position;

                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange && canAttack == true)
                            {
                                photonView.RPC("Attack", RpcTarget.All, attackDamage);

                            }
                        }
                        break;
                }
            }
        }
    }

    [PunRPC]
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
        target = NewDestination(origin, roamRange, -1);
        agent.destination = target;
    }

    [PunRPC]
    public void Attack(float damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (nearestPlayer.TryGetComponent(out PlayerHealth player))
            {
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
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        float totalFOV = detectionAngle;
        float rayRange = detectionRange;
        float halfFOV = totalFOV / 2.0f;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);

        Gizmos.color = Color.yellow;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }
}