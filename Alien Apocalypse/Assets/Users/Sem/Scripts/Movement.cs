using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks
{
    [Header("Speed modifiers")]
    public float walkSpeed = 20f;
    public float sprintSpeed = 30f;
    public float downedSpeed = 0.2f;
    public float maxVelocityChange = 20f;
    public float turnSpeed;
    public float slowDownForce;
    public float stopTime;
    public bool stopped;
    public float fallSpeed = 0;


    [Header("Wall Jumping")]
    public float jumpHeight = 3f;

    [Header("Player State")]
    public bool sprinting;
    public bool jumping;

    [Header("Ground check")]
    public bool grounded = false;
    public Vector2 input;
    public Rigidbody rb;
    public float damage = 25;

    [Header("Field of view")]
    public float curFov;
    public float normalFov;
    public float maxFov;
    public float airMultiplier = 1f;
    public GameObject cameraPivot;
    public bool downed;
    public bool wallrunUnlocked;
    public bool wallRunning;
    public bool dashUnlocked;
    public float fallingspeedThreshold;
    public bool walkingBackwards;
    public int walkState;
    public Animator anim;
    public Animator armAnim;
    public int armState;

    float startSpeed;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalFov = Camera.main.fieldOfView;
        maxFov = Camera.main.fieldOfView + 10;
        PhotonNetwork.SerializationRate = 25;
        startSpeed = walkSpeed;
    }

    bool appliedKnocked;
    private void Update()
    {
        if (downed && appliedKnocked == false)
        {
            photonView.RPC("Knocked", RpcTarget.All);
            walkSpeed = downedSpeed;
            appliedKnocked = false;

        }
        else if(!downed)
        {
            walkSpeed = startSpeed;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");
        

        FovChange();

        Physics.IgnoreLayerCollision(3,11,true);
        StopWhenNoInput();
        AnimationCheck();
        ArmAnimCheck();
        
        
        if (Input.GetKey(KeyCode.S))
        {
            walkingBackwards = true;
            photonView.RPC("Walking", RpcTarget.All);
        }
        else
        {
            walkingBackwards = false;
            photonView.RPC("Walking", RpcTarget.All);
        }
    }

    bool IsMoving()
    {
        if (input.magnitude >0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ArmAnimCheck()
    {
        
        if(sprinting && input.magnitude > 0.5f)
        {
            photonView.RPC("AnimRPC", RpcTarget.All, 2);
            
        }

        if(!sprinting && input.magnitude > 0.5f )
        {
            photonView.RPC("AnimRPC", RpcTarget.All, 1);
        }
        
        if(input.magnitude < 0.5f)
        {
            photonView.RPC("AnimRPC", RpcTarget.All, 0);
        }

        
    }
    [PunRPC]
    public void AnimRPC(int i)
    {
        armAnim.SetInteger("ArmState", i);
        armAnim.SetBool("Jumping", grounded);
        armAnim.SetBool("Moving", IsMoving());
        


    }
    [PunRPC]
    void Knocked()
    {
        armAnim.SetTrigger("Knocked");
    }
    private void OnTriggerStay(Collider other)
    {
        grounded = other.gameObject.tag == "Ground";
        
    }
    public void StopWhenNoInput()
    {
        stopTime -= Time.deltaTime;
        if (input.magnitude < 0.5f && grounded && stopped == false)
        {
            stopTime = 1;
            stopped = true;
        }

        if (stopTime > 0)
        {
            Vector3 oppositeForce = -rb.velocity.normalized * slowDownForce;
            rb.AddForce(oppositeForce);
        }

        if (stopTime > 0 && rb.velocity.magnitude < 0.1f)
        {
            stopTime = 0;
        }

        if (stopTime > 0 && rb.velocity.magnitude > 0.1f && stopped == false)
        {
            stopTime = 0;
        }
    }
    private void FovChange()
    {
        if (sprinting && input.magnitude > 0.5f)
        {
            if (Camera.main.fieldOfView < maxFov)
            {
                float fovChange = Mathf.Lerp(curFov, normalFov + rb.velocity.magnitude, Time.deltaTime * 5);
                Camera.main.fieldOfView = fovChange;
            }
        }
        else
        {
            if (Camera.main.fieldOfView != normalFov && grounded)
            {
                float fovChange = Mathf.Lerp(curFov, normalFov, Time.deltaTime * 5);
                Camera.main.fieldOfView = fovChange;
            }
        }

        curFov = Camera.main.fieldOfView;
    }
    
   

    private void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping && !downed)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                anim.SetTrigger("Jump");
            }
            else if (input.magnitude > 0.5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.Force);
                stopped = false;
            }
        }

        if (!grounded && input.magnitude > 0.5f)
        {
            rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * airMultiplier : walkSpeed * airMultiplier), ForceMode.Force);
            stopped = false;
        }
        float maxSpeed = 75f;
        if (wallrunUnlocked == true)
        {
            if (rb.velocity.y < -0.5f && wallRunning == false)
            {
                fallSpeed += Time.deltaTime;
                Vector3 extraForceDirection = Vector3.down;
                if(rb.velocity.magnitude <= maxSpeed)
                {
                    rb.AddForce(extraForceDirection * 10 * fallSpeed, ForceMode.Acceleration);
                }
            }
            else
            {
                fallSpeed = 0;
            
            }
            
        }
        else if(wallrunUnlocked == false)
        {
            if (rb.velocity.y < -0.5f)
            {
                fallSpeed += Time.deltaTime;
                Vector3 extraForceDirection = Vector3.down;
                if (rb.velocity.magnitude <= maxSpeed)
                {
                    rb.AddForce(extraForceDirection * 10 * fallSpeed, ForceMode.Acceleration);
                }
            }
            else
            {
                fallSpeed = 0;
            }
            
        }

        grounded = false;
    }

    private Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;
        Vector3 velocity = rb.velocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            return velocityChange;
        }
        else
        {
            return Vector3.zero;
        }
    }
    bool jumped;
    public void AnimationCheck()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetJumpAnim", RpcTarget.All, grounded);
            if (!grounded && jumped == false)
            {
                jumped = true;
            }
            else if (grounded && jumped == true)
            {
                jumped = false;
            }
            if (sprinting && input.magnitude > 0.5f && grounded)
            {
                walkState = 2;
                photonView.RPC("UpdateAnimation", RpcTarget.All, walkState, walkingBackwards, false);
            }

            if (!sprinting && input.magnitude > 0.5f && grounded)
            {
                walkState = 1;
                photonView.RPC("UpdateAnimation", RpcTarget.All, walkState, walkingBackwards,false);
            }

            if (input.magnitude < 0.5f && grounded)
            {
                walkState = 0;
                photonView.RPC("UpdateAnimation", RpcTarget.All, walkState, walkingBackwards,false);
            }
        }
    }
   [PunRPC]
    public void UpdateAnimation(int state, bool walkingBackwards,bool down)
    {
        anim.SetBool("WalkingBackwards", walkingBackwards);
        anim.SetInteger("WalkState", state);
        anim.SetBool("Downed", down);
    }
    [PunRPC]
    private void Walking()
    {
        anim.SetBool("WalkingBackwards", walkingBackwards);
    }
    [PunRPC]
    void SetJumpAnim(bool b)
    {
        anim.SetBool("Grounded", b);
        armAnim.SetBool("Jumping", b);
    }

}