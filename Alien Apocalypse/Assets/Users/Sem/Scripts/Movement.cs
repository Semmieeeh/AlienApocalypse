using System.Runtime.InteropServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
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
    public LayerMask mask;
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
    bool animgrounded;

    float startSpeed;
    WallRunning wr;
    private void Start()
    {
        wr = GetComponent<WallRunning>();
        rb = GetComponent<Rigidbody>();
        normalFov = Camera.main.fieldOfView;
        startSpeed = walkSpeed;
        
    }
    public void AdjustFov(OptionsManager.OptionsData data)
    {
        
        normalFov = data.Fov;
        Camera.main.fieldOfView = normalFov;
        maxFov = Camera.main.fieldOfView + 10;
    }
    bool appliedKnocked;
    RaycastHit hit;
    float animtime;
    public float animinterval = 0.1f;
    private void Update()
    {
        FovChange();
        grounded = Physics.Raycast(transform.position, -transform.up, out hit, 1.05f,mask);
        animgrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1.5f, mask);
        if (downed && appliedKnocked == false)
        {
            Knocked();
            walkSpeed = downedSpeed;
            appliedKnocked = false;

        }
        else if (!downed)
        {
            walkSpeed = startSpeed;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        sprinting = Input.GetButton("Sprint") && !downed && input.magnitude > 0.5f;
        jumping = Input.GetButton("Jump");
        Physics.IgnoreLayerCollision(3, 11, true);
        StopWhenNoInput();
        

        animtime += Time.deltaTime;
        time += Time.deltaTime;
        AnimationCheck();
        ArmAnimCheck();
    }
    
    bool IsMoving()
    {
        if (input.magnitude > 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    float time;
    float interval = 0.1f;
    int animInt;
    public void ArmAnimCheck()
    {
        
        if (time > interval)
        {
            
            if (sprinting && input.magnitude > 0.5f)
            {
                
                animInt = 2;
            }

            if (!sprinting && input.magnitude > 0.5f)
            {
                animInt = 1;
            }

            if (input.magnitude < 0.5f)
            {
                animInt = 0;
            }
        }  
    }
    
    void Knocked()
    {
        armAnim.SetTrigger("Knocked");
    }   
    public void StopWhenNoInput()
    {
        
        if (rb.velocity.magnitude > 0.2f && input.magnitude<0.5f && grounded)
        {
            Vector3 oppositeForce = -rb.velocity.normalized * slowDownForce;
            rb.AddForce(oppositeForce);
        }

        
    }

    public void FovChange()
    {
        if (sprinting && input.magnitude > 0.5f || input.magnitude >0.5f && grounded == false)
        {
            Debug.Log("Increasing fov");
            if (Camera.main.fieldOfView < maxFov)
            {
                float fovChange = Mathf.Lerp(curFov, normalFov + rb.velocity.magnitude, Time.deltaTime * 5);
                Camera.main.fieldOfView = fovChange;
            }
        }
        else
        {
            Debug.Log("Decreasing fov");
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
        if (input.y < 0)
        {
            walkingBackwards = true;

        }
        else
        {
            walkingBackwards = false;

        }

        if (animtime > animinterval)
        {
            //photonView.RPC("Walking", RpcTarget.All);
            Walking();
        }

        if (grounded)
        {
            if (jumping && !downed)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);

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
        else if (wallrunUnlocked == false)
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
        }

        if (!sprinting && input.magnitude > 0.5f && grounded)
        {
            walkState = 1;
        }

        if (input.magnitude < 0.5f && grounded)
        {
            walkState = 0;

        }
        anim.SetBool("WalkingBackwards", walkingBackwards);
        anim.SetInteger("WalkState", walkState);
        anim.SetBool("Downed", downed);
        anim.SetBool("Grounded", animgrounded);
        //armAnim.SetBool("Jumping", animgrounded);
    }
   
    private void Walking()
    {
        anim.SetBool("WalkingBackwards", walkingBackwards);
    }
    

    
}