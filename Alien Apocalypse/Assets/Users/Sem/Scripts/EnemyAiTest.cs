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
    public EnemyType type;
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
    public enum EnemyType
    {
        FLYING,
        WALKING,
        BOMBER,
        HEAVY,

    }
    
    EnemyState state;
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
        interval = 0.2f;
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
    float time;
    public float interval;
    int armInt;
    int legInt;
    bool inRange;
    public bool doAnim;
    private void Update()
    {

        time += Time.deltaTime;
        timePassed += Time.deltaTime;
        CheckForPlayer();
        if (timePassed > attackSpeed)
        {
            canAttack = true;
            timePassed = Random.Range(-attackSpeed, attackSpeed);
        }
        if (canChooseNew == true)
        {
            NewTarget();
            canChooseNew = false;
        }
        if(doAnim == true && !flyingEnemy && time >=interval)
        {
            photonView.RPC(nameof(UpdateAlienLegs), RpcTarget.All, legInt);
            photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, armInt, null, inRange);
            doAnim = false;
            time = 0;
        }
        switch (state)
        {
            case EnemyState.idle:
                inRange = false;
                legInt = 1;
                armInt = 0;

                switch (type)
                {
                    case EnemyType.FLYING:
                        currentOffset = agent.baseOffset;
                        float newOffset = Mathf.Lerp(currentOffset, firstOffset, Time.deltaTime * moveSpeed / 4);
                        agent.baseOffset = newOffset;
                        agent.stoppingDistance = 0;
                        if (Vector3.Distance(transform.position, target) <= targetRange - 1)
                        {
                            NewTarget();

                        }
                        break;
                    case EnemyType.WALKING:
                        agent.stoppingDistance = targetRange;
                        if (Vector3.Distance(transform.position, target) <= targetRange - 1)
                        {
                            NewTarget();

                        }

                        break;
                    case EnemyType.BOMBER:
                        agent.stoppingDistance = targetRange;
                        if (Vector3.Distance(transform.position, target) <= targetRange - 1)
                        {
                            NewTarget();

                        }


                        break;
                    case EnemyType.HEAVY:

                        agent.stoppingDistance = targetRange;                       
                        if (Vector3.Distance(transform.position, target) <= targetRange - 1)
                        {
                            NewTarget();

                        }
                        break;
                }


                break;

            case EnemyState.chasing:

                legInt = 2;
                

                switch (type)
                {
                    case EnemyType.FLYING:

                        if (nearestPlayer != null && agent != null)
                        {
                            agent.destination = nearestPlayer.transform.position;
                            agent.stoppingDistance = attackRange;

                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange)
                            {
                                inRange = true;
                                Quaternion targetRotation = CalculateRotationToPlayer();
                                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                                if (canAttack == true)
                                {
                                    doAnim = true;
                                    photonView.RPC("Attack", RpcTarget.All, attackDamage);

                                }

                            }
                            else
                            {
                                armInt = 1;
                            }
                        }



                            currentOffset = agent.baseOffset;
                        desiredOffset = nearestPlayer.transform.position.y + firstOffset;
                        float newOffset = Mathf.Lerp(currentOffset, desiredOffset, Time.deltaTime * moveSpeed / 4);
                        agent.baseOffset = newOffset;

                        if (nearestPlayer.GetComponent<PlayerHealth>().knocked == true)
                        {
                            canChooseNew = true;
                            state = EnemyState.idle;


                        }

                        break;
                    case EnemyType.WALKING:

                        if (nearestPlayer != null && agent != null)
                        {
                            agent.destination = nearestPlayer.transform.position;
                            agent.stoppingDistance = attackRange;

                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange)
                            {
                                inRange = true;
                                legInt = 0;
                                Quaternion targetRotation = CalculateRotationToPlayer();
                                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                                if (canAttack == true)
                                {
                                    doAnim = true;
                                    photonView.RPC("Attack", RpcTarget.All, attackDamage);

                                }

                            }
                            else
                            {
                                armInt = 1;
                            }
                        }

                        if (nearestPlayer.GetComponent<PlayerHealth>().knocked == true)
                        {
                            canChooseNew = true;
                            state = EnemyState.idle;


                        }
                        break;
                    case EnemyType.BOMBER:
                        armInt = 2;
                        legInt = 2;
                        if (nearestPlayer != null && agent != null)
                        {
                            agent.destination = nearestPlayer.transform.position;
                            agent.stoppingDistance = attackRange;

                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange)
                            {
                                inRange = true;
                                Quaternion targetRotation = CalculateRotationToPlayer();
                                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                                if (canAttack == true)
                                {
                                    doAnim = true;
                                    photonView.RPC("Attack", RpcTarget.All, attackDamage);

                                }

                            }
                            else
                            {
                                armInt = 1;
                            }
                        }

                        if (nearestPlayer.GetComponent<PlayerHealth>().knocked == true)
                        {
                            canChooseNew = true;
                            state = EnemyState.idle;


                        }
                        break;
                    case EnemyType.HEAVY:
                        armInt = 2;
                        legInt = 2;
                        if (nearestPlayer != null && agent != null)
                        {
                            agent.destination = nearestPlayer.transform.position;
                            agent.stoppingDistance = attackRange;

                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) < attackRange)
                            {
                                inRange = true;
                                Quaternion targetRotation = CalculateRotationToPlayer();
                                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                                if (canAttack == true)
                                {
                                    doAnim = true;
                                    photonView.RPC("Attack", RpcTarget.All, attackDamage);

                                }

                            }
                            else
                            {
                                armInt = 1;
                            }
                        }

                        if (nearestPlayer.GetComponent<PlayerHealth>().knocked == true)
                        {
                            canChooseNew = true;
                            state = EnemyState.idle;


                        }
                        break;
                }

            break;
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
        EnemyHealth h = GetComponent<EnemyHealth>();

        if (nearestPlayer != null)
        {
            if (nearestPlayer.TryGetComponent(out PlayerHealth player) && nearestPlayer.TryGetComponent(out Movement m))
            {
                source.clip = clips[0];
                source.Play();
                photonView.RPC(nameof(UpdateAlienArms), RpcTarget.All, null, "Attack", null);
                canAttack = false;
                switch (type)
                {
                    case EnemyType.WALKING:
                        if (m.rb.velocity.magnitude <= 0f)
                        {
                            
                            flash.Play();
                            Vector3 j = player.transform.position;
                            j.y += 0.4f;
                            bulletTracer.Activate(true, j);
                            player.TakeDamage(damage);
                            HitIndicatorManager.Instance.AddTarget(transform);
                            Debug.Log("Hit!");
                        }
                        else if (m.rb.velocity.magnitude > 0f)
                        {
                            
                            
                            flash.Play();
                            int k = Random.Range(0, m.rb.velocity.magnitude.ToInt());
                            Debug.Log(k);
                            if (k < 5)
                            {
                                player.TakeDamage(damage);
                                HitIndicatorManager.Instance.AddTarget(transform);
                                Vector3 v = player.transform.position;
                                bulletTracer.Activate(true, v);
                                Debug.Log("Hit By Chance!");
                            }
                            else
                            {
                                Vector3 v = new Vector3(Random.Range(player.transform.position.x - particleMissOffset, player.transform.position.x + particleMissOffset), Random.Range(player.transform.position.y - particleMissOffset, player.transform.position.y + particleMissOffset), Random.Range(player.transform.position.z - particleMissOffset, player.transform.position.z + particleMissOffset));
                                v *= 10;
                                bulletTracer.Activate(true, v);
                                Debug.Log("Missed!");
                            }

                        }

                        break;
                    case EnemyType.FLYING:

                        if (m.rb.velocity.magnitude <= 0f)
                        {
                            
                            flash.Play();
                            Vector3 j = player.transform.position;
                            j.y += 0.4f;
                            bulletTracer.Activate(true, j);
                            player.TakeDamage(damage);
                            HitIndicatorManager.Instance.AddTarget(transform);
                            Debug.Log("Hit!");
                        }
                        else if (m.rb.velocity.magnitude > 0f)
                        {                            
                            flash.Play();
                            int k = Random.Range(0, m.rb.velocity.magnitude.ToInt());
                            Debug.Log(k);
                            if (k < 5)
                            {
                                player.TakeDamage(damage);
                                HitIndicatorManager.Instance.AddTarget(transform);
                                Vector3 v = player.transform.position;
                                bulletTracer.Activate(true, v);
                                Debug.Log("Hit By Chance!");
                            }
                            else
                            {
                                Vector3 v = new Vector3(Random.Range(player.transform.position.x - particleMissOffset, player.transform.position.x + particleMissOffset), Random.Range(player.transform.position.y - particleMissOffset, player.transform.position.y + particleMissOffset), Random.Range(player.transform.position.z - particleMissOffset, player.transform.position.z + particleMissOffset));
                                v *= 10;
                                bulletTracer.Activate(true, v);
                                Debug.Log("Missed!");
                            }

                        }

                        break;
                    case EnemyType.HEAVY:
                        player.TakeDamage(damage);
                        HitIndicatorManager.Instance.AddTarget(transform);
                        Debug.Log("Punched");
                        //punch anim
                        break;
                    case EnemyType.BOMBER:
                        
                        player.TakeDamage(damage);
                        HitIndicatorManager.Instance.AddTarget(transform);
                        h.Explode();
                        Debug.Log("Exploded");
                        break;

                }


            }
        }
    }

    bool animExecuted;
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
                        if(state != EnemyState.chasing)
                        {
                            state = EnemyState.chasing;
                            doAnim = true;
                        }
                    }

                }
            }
            else
            {
                if(state != EnemyState.idle)
                {
                    state = EnemyState.idle;
                    doAnim = true;
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