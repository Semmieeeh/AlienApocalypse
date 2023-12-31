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
    public  float maxDistance;
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
    public UIAbility ui;
    

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Start()
    {
        if (pv.IsMine)
        {
            armLowerTime = .1f;
            playerCam = Camera.main.transform;
            player = gameObject.transform;
            idleStrength = 2;
            minDamperStrength = .5f;
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
            inRange = Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable);
        }
        if (pv.IsMine)
        {

            if (pv.IsMine && playerCam != null)
            {
                inRange = Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable);

                if (pointingArm)
                {
                    armLowerTime -= Time.deltaTime;
                }

                if (Input.GetButton("Grapple") && !isGrappling && !pointingArm && canGrapple)
                {
                    damperStrength = minDamperStrength;
                    grappleStrength = idleStrength;
                    LiftArm();
                    SyncArmAnimation(1);
                }

                if (!Input.GetButton("Grapple") && !isGrappling && !pointingArm && !canGrapple)
                {
                    canGrapple = true;
                }

                if (armLowerTime <= 0 && !Input.GetButton("Grapple") && !isGrappling)
                {
                    CheckForRayCast();
                }

                if (isGrappling)
                {
                    if (joint != null)
                    {
                        joint.maxDistance -= retractionSpeed;
                    }

                    if (Input.GetButtonDown("Grapple") || (joint != null && Vector3.Distance(player.position, grapplePoint) <= joint.minDistance))
                    {
                        StopGrapple();
                        SyncArmAnimation(0);
                    }

                    if (childOfPoint == null || grapplePointParent == null)
                    {
                        StopGrapple();
                    }

                    if (childOfPoint != null && grapplePointParent != null && grapplePoint != null && joint != null)
                    {
                        childOfPoint.transform.parent = grapplePointParent.transform;
                        grapplePoint = childOfPoint.transform.position;
                        joint.connectedAnchor = grapplePoint;
                    }
                }
            }
        }
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
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable) && abilityCooldown <=0)
        {
            if (hit.transform.gameObject.tag != "Enemy")
            {
                StartGrapple();
                arm.SetInteger("FireState", 2);
                SyncArmAnimation(2);
            }            

        }
        else
        {
            StopGrapple();
            canGrapple = true;
            armLowerTime = maxAnimDuration;
            arm.SetInteger("FireState", 2);
            SyncArmAnimation(2);
        }
    }

    void StartGrapple()
    {
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            
            canGrapple = false;
            arm.gameObject.GetComponent<AudioSource>().Play();
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
            ui.Activate();
            abilityCooldown = 2;
            ui.cooldown = abilityCooldown;
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