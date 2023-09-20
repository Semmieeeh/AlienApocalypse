using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTest : MonoBehaviour
{
    [Header("Ai Modifiers")]
    public float moveSpeed;    
    private NavMeshAgent agent;
    private Vector3 origin;
    public float roamRange;

    [Space]
    [Header("Current Target")]
    public Vector3 target;

    [Header("Player")]
    public GameObject player;
    public float angleToPLayer;
    public float detectionAngle;
    public float detectionRange;
    public enum EnemyState
    {
        idle,
        chasing
    }
    public EnemyState state;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        target = NewDestination(origin, roamRange,-1);
        agent.destination = target;
        state = EnemyState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.idle:
                
                if(Vector3.Distance(transform.position,target)< 3f)
                {
                    target = NewDestination(origin, roamRange, -1);
                    agent.destination = target;
                }
                break;

            case EnemyState.chasing:

                break;

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

    public void Attack()
    {
        
    }
    public void CheckForPlayer()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 targetDir = playerPos - transform.position;
        angleToPLayer = Vector3.Angle(targetDir, transform.forward);
        if (Vector3.Distance(transform.position,player.transform.position)< 10 && angleToPLayer < 90)
        {
            state = EnemyState.chasing;
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
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);


        Gizmos.color = Color.yellow;
    }
}
