using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Grappling : MonoBehaviourPunCallbacks
{
    public bool unlockedSkill;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, playerCam, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public bool isGrappling = false;
    public float grappleStrength;
    public float idleStrength;
    public float retractionSpeed;
    public float damperStrength;
    public float minDamperStrength;
    public float maxDamperStrength;
    public GameObject grapplePointChild;
    public GameObject grapplePointParent;
    public float armLowerTime;
    public RaycastHit hit;
    public bool pointingArm;
    public GameObject childOfPoint;
    public bool letGoBeforeMaxHeight;
    public float animDuration;
    float maxAnimDuration;
    public GameObject armObj;
    public bool canGrapple;
    public Animator arm;
    PhotonView pv;
    public bool inRange;
    public float pullStrength;
    public float stunTime;
    public float abilityCooldown;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Start()
    {
        if (pv.IsMine)
        {
            playerCam = Camera.main.transform;
            player = gameObject.transform;
            idleStrength = 4;
            minDamperStrength = 1f;
            maxAnimDuration = 0.17f;
            animDuration = maxAnimDuration;
            canGrapple = true;
        }
    }

    void Update()
    {

        abilityCooldown -= Time.deltaTime;
        if (pv.IsMine && playerCam != null)
        {
            pv.RPC("RaycastRPC", RpcTarget.All);
        }
        if (pv.IsMine)
        {
            if (pointingArm == true)
            {
                armLowerTime -= Time.deltaTime;
            }
            if (Input.GetButton("Grapple") && isGrappling == false && pointingArm == false && canGrapple == true)
            {
                damperStrength = minDamperStrength;
                grappleStrength = idleStrength;
                LiftArm();
                pv.RPC("SyncArmAnimation", RpcTarget.Others, 1); // Notify other players about the animation change
            }
            if (!Input.GetButton("Grapple") && isGrappling == false && pointingArm == false && canGrapple == false)
            {
                canGrapple = true;
            }
            if (armLowerTime <= 0 && !Input.GetButton("Grapple") && isGrappling == false)
            {
                CheckForRayCast();
            }
            if (isGrappling == false)
            {
                return;
            }
            else
            {
                if (joint != null)
                {
                    joint.maxDistance -= retractionSpeed;
                }
            }
            if (isGrappling && Input.GetButtonDown("Grapple"))
            {
                StopGrapple();
                pv.RPC(nameof(SyncArmAnimation), RpcTarget.Others, 0);
            }
            else if (joint != null && isGrappling && Vector3.Distance(player.position, grapplePoint) <= joint.minDistance)
            {
                StopGrapple();
                arm.SetInteger("FireState", 0);
                pv.RPC(nameof(SyncArmAnimation), RpcTarget.Others, 0);
            }
            if (childOfPoint == null || grapplePointParent == null)
            {
                StopGrapple();
            }
            if (childOfPoint == null)
            {
                return;
            }

            if (childOfPoint != null && grapplePointParent != null && grapplePoint != null && joint != null)
            {
                childOfPoint.transform.parent = grapplePointParent.transform;
                grapplePoint = childOfPoint.transform.position;
                joint.connectedAnchor = grapplePoint;
            }
        }
    }

    [PunRPC]
    public void RaycastRPC()
    {
        inRange = Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable);
    }

    [PunRPC]
    public void SyncArmAnimation(int animationState)
    {
        arm.SetInteger("FireState", animationState);
    }

    public void LiftArm()
    {
        arm.SetInteger("FireState", 1);
        pointingArm = true;
        pv.RPC(nameof(SyncArmAnimation), RpcTarget.All, 1);
    }

    public void CheckForRayCast()
    {
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            if (hit.transform.gameObject.tag != "Enemy")
            {
                StartGrapple();
                arm.SetInteger("FireState", 2);
                pv.RPC(nameof(SyncArmAnimation), RpcTarget.All, 2);
            }
            else if (hit.transform.tag == "Enemy" && abilityCooldown <= 0)
            {
                pv.RPC(nameof(StartEnemyGrapple), RpcTarget.All);
                arm.SetInteger("FireState", 2);
                pv.RPC(nameof(SyncArmAnimation), RpcTarget.All, 2);
            }

        }
        else
        {
            StopGrapple();
            canGrapple = true;
            armLowerTime = maxAnimDuration;
            arm.SetInteger("FireState", 2);
            pv.RPC(nameof(SyncArmAnimation), RpcTarget.All, 2);
        }
    }

    void StartGrapple()
    {
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            canGrapple = false;
            armLowerTime = maxAnimDuration;
            pointingArm = false;
            isGrappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0f;

            joint.spring = grappleStrength;
            joint.damper = damperStrength;
            joint.massScale = 4.5f;

            grapplePointParent = hit.transform.gameObject;
            childOfPoint = Instantiate(grapplePointChild, grapplePoint, Quaternion.identity);
        }
    }
    private GameObject pulledEnemy;

    [PunRPC]
    public void StartEnemyGrapple()
    {
        if (playerCam != null)
        {
            if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable))
            {
                canGrapple = false;
                armLowerTime = maxAnimDuration;
                pointingArm = false;
                isGrappling = true;
                grapplePoint = hit.point;
                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;
                float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
                joint.maxDistance = distanceFromPoint * 0.7f;
                joint.minDistance = distanceFromPoint * 0.9f;
                abilityCooldown = 3;

                joint.spring = grappleStrength;
                joint.damper = damperStrength;
                joint.massScale = 4.5f;

                grapplePointParent = hit.transform.gameObject;
                grapplePointParent.GetComponent<EnemyAiTest>().enabled = false;
                grapplePointParent.GetComponent<NavMeshAgent>().enabled = false;
                childOfPoint = Instantiate(grapplePointChild, grapplePoint, Quaternion.identity);
                grapplePointParent.GetComponent<Rigidbody>().velocity = Vector3.zero;
                pulledEnemy = grapplePointParent;
                pv.RPC(nameof(SyncEnemyStates), RpcTarget.All, false);
                pv.RPC(nameof(PullEnemy), RpcTarget.All);
                Invoke("ResetEnemyRPC", stunTime);


            }
        }
    }

    [PunRPC]
    public void SyncEnemyStates(bool enableComponents)
    {
        if (pulledEnemy != null)
        {
            pulledEnemy.GetComponent<EnemyAiTest>().enabled = enableComponents;
            pulledEnemy.GetComponent<NavMeshAgent>().enabled = enableComponents;
        }
    }

    [PunRPC]
    public void PullEnemy()
    {
        if (pulledEnemy != false)
        {
            Vector3 pullDirection = gameObject.transform.position - pulledEnemy.transform.position;
            pullDirection.y += 3;
            Rigidbody rb = pulledEnemy.GetComponent<Rigidbody>();
            pullStrength = Vector3.Distance(gameObject.transform.position, pulledEnemy.transform.position) * 0.125f;
            if (pullStrength < 0.2)
            {
                pullStrength = 0.2f;
            } 
            else if (pullStrength > 1.2f)
            {
                pullStrength = 1.2f;
            }

            Debug.Log(pullStrength.ToString());
            rb.freezeRotation = false;
            
            rb.AddForce(pullDirection * pullStrength, ForceMode.Impulse);
            
        }
    }

    [PunRPC]
    public void ResetEnemy()
    {
        if(pulledEnemy != null)
        {
            pulledEnemy.GetComponent<NavMeshAgent>().enabled = true;
            pulledEnemy.GetComponent<EnemyAiTest>().enabled = true;
            Rigidbody rb = pulledEnemy.GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.rotation = new Quaternion(0,0,0,1);
            pulledEnemy = null;
            pv.RPC("SyncEnemyStates", RpcTarget.All, true);
        }
        
    }

    [PunRPC]
    public void ResetEnemyRPC()
    {
        pv.RPC(nameof(ResetEnemy), RpcTarget.All);
    }
    public void StopGrapple()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
        arm.SetInteger("FireState", 0);
        if (childOfPoint != null)
        {
            Destroy(childOfPoint);
            childOfPoint = null;
        }
        if (grapplePointParent != null)
        {
            grapplePointParent = null;
        }
        isGrappling = false;
        pointingArm = false;
        animDuration = maxAnimDuration;
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}