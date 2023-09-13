using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Speed modifiers")]
    public float walkSpeed = 20f;
    public float sprintSpeed = 30f;
    public float maxVelocityChange = 20f;
    public float turnSpeed;

    [Header("Wall Jumping")]
    public float jumpHeight = 3f;
    public float wallrunGravity;
    public float wallrunAscendGravity;
    public bool onWall;
    private bool sprinting;
    private bool jumping;
    public bool wallJumping;
    public bool jumpedOnWall;
    public float wallCooldown;
    public bool canWallJump;
    [Header("Ground check")]
    public bool grounded = false;
    private Vector2 input;
    private Rigidbody rb;
    public float damage = 25;

    [Header("Field of view")]
    public float curFov;
    public float normalFov;
    public float maxFov;
    public bool canDash;
    public float dashCooldown;
    public float maxDashCooldown;
    public float airMultiplier = 1f;
    public GameObject cameraPivot;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalFov = Camera.main.fieldOfView;
        maxFov = Camera.main.fieldOfView + 10;
        wallCooldown = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rb.velocity.magnitude);
        dashCooldown -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.LeftAlt) )
        {
            Dash();
        }
        wallCooldown -= Time.deltaTime;
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        sprinting = Input.GetButton("Sprint");
        if(Input.GetButton("Jump") && !onWall)
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }

        if (Input.GetButton("Jump") && onWall)
        {
            wallJumping = true;
        }
        else
        {
            wallJumping = false;
        }



        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        
        if(input.magnitude > 0.5f)
        {
            if (sprinting)
            {

                if (Camera.main.fieldOfView != maxFov)
                {
                    Camera.main.fieldOfView += 200 * Time.deltaTime;

                }

            }
            if (!sprinting)
            {
                if (Camera.main.fieldOfView != normalFov)
                {
                    Camera.main.fieldOfView -= 200 * Time.deltaTime;
                }
            }
        }
        if (sprinting && input.magnitude <0.5f)
        {
            if (Camera.main.fieldOfView != normalFov)
            {
                Camera.main.fieldOfView -= 200 * Time.deltaTime;
            }
        }
        if (sprinting && input.magnitude > 0.5f)
        {
            if (Camera.main.fieldOfView != normalFov)
            {
                Camera.main.fieldOfView -= 200 * Time.deltaTime;
            }
        }



        curFov = Camera.main.fieldOfView;
        if (Camera.main.fieldOfView >= maxFov)
        {
            Camera.main.fieldOfView = maxFov;
        }

        if (Camera.main.fieldOfView <= normalFov)
        {
            Camera.main.fieldOfView = normalFov;
        }


        if(wallCooldown <= 0)
        {
            canWallJump = true;
        }
    }
    public void Fire()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            if (hit.transform.gameObject.GetComponent<EnemyHealth>())
            {
                EnemyHealth e = hit.transform.gameObject.GetComponent<EnemyHealth>();
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("EnemyTakeDamage", RpcTarget.All, damage);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Ground")
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if(grounded == false && other.gameObject.tag == "Wall")
        {
            if(canWallJump == true)
            {
                onWall = true;
            }
        }
        else
        {
            onWall = false;
        }
        
    }
    private void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);    
            }
            else if (input.magnitude > 0.5f && grounded == true)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.Force);
            }
            
        }
        if (!grounded)
        {
            if (input.magnitude > 0.5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * airMultiplier : walkSpeed * airMultiplier), ForceMode.Force );
            }
        }
        

        if (onWall == true)
        {

            if (sprinting)
            {
                Vector3 rbVel = new Vector3(rb.velocity.x, wallrunGravity, rb.velocity.z);
                rb.velocity = rbVel;
                
            }

            if (sprinting && input.magnitude >0.5f)
            {
                Vector3 rbVel = new Vector3(rb.velocity.x, wallrunAscendGravity, rb.velocity.z);
                rb.velocity = rbVel;

            }
            if (wallJumping && canWallJump == true)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                wallCooldown = 1;
                canWallJump = false;
                
            }

        }
        grounded = false;
        onWall = false;
    }

    public void Dash()
    {
        if(dashCooldown <= 0)
        {
            float velBeforeStop = rb.velocity.magnitude;
            rb.velocity = Vector3.zero;
            rb.AddForce(Camera.main.transform.forward * velBeforeStop* 1.3f, ForceMode.Impulse);
            dashCooldown = maxDashCooldown;
        }
    }
    Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity*=speed;
        Vector3 velocity = rb.velocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange,maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            Vector3 turnTorque = Vector3.up * turnSpeed * input.x * Mathf.Sign(input.y);
            rb.AddTorque(turnTorque);
            return velocityChange;

            
        }
        else
        {
            return new Vector3();
        }
    }
}
