using System.Collections;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("References")]
    public Movement m;
    public Animator anim;
    public GameObject cameraPivot;

    [Header("Wall Running Settings")]
    public float wallrunGravity;
    public float wallCooldown;
    public bool canWallJump;
    public bool wallrunningActive;
    public float startingMass;
    public bool unlockedSkill;
    public float multiplier;

    [Header("Wall Collision")]
    public Collider collider1;
    public Collider collider2;
    public bool hitLeft;
    public bool hitRight;

    [Header("Wall Jump")]
    public float rotateAmount = 20;

    [Header("State")]
    public bool onWall;
    public bool wallJumping;

    private float normalRotation;

    void Start()
    {
        wallCooldown = 1;
        normalRotation = Camera.main.transform.rotation.z;
        startingMass = wallrunGravity;
        m.wallrunUnlocked = true;
        multiplier = 1;
    }

    private void Update()
    {
        if (unlockedSkill)
        {
            wallCooldown -= Time.deltaTime;
            if (m == null)
            {
                m = GetComponent<Movement>();
            }
            if (wallrunningActive)
            {
                wallrunGravity -= 2f * multiplier * Time.deltaTime;
            }
            m.wallRunning = wallrunningActive;
            WallRun();
        }
    }

    void FixedUpdate()
    {
        if (unlockedSkill)
        {
            if (onWall)
            {
                if (m.rb != null && m.sprinting)
                {
                    Vector3 rbVel = new Vector3(m.rb.velocity.x, wallrunGravity, m.rb.velocity.z);
                    m.rb.velocity = rbVel;

                    if (m.input.magnitude > 0.5f)
                    {
                        m.rb.velocity = rbVel;
                    }
                    if (wallJumping && canWallJump && onWall)
                    {
                        m.rb.velocity = new Vector3(m.rb.velocity.x, m.jumpHeight, m.rb.velocity.z);
                        wallCooldown = 1;
                        canWallJump = false;
                    }
                }
            }
            onWall = false;
            RotateCameraOnCollision();
        }
    }

    public void RotateCameraOnCollision()
    {
        if (m != null)
        {
            if (hitLeft || hitRight)
            {
                anim.SetInteger("RunDirection", hitLeft ? 2 : 1);
                wallrunningActive = true;
                
            }
            else
            {
                anim.SetInteger("RunDirection", 0);
                wallrunningActive = false;
                wallrunGravity = startingMass;
            }
        }
        else if(m == null)
        {
            m = GetComponent<Movement>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (unlockedSkill)
        {
            Invoke("CheckForAnimationCancel", 0.2f);
            wallrunningActive = false;
            wallrunGravity = startingMass;
        }
    }
    public void CheckForAnimationCancel()
    {
        anim.SetInteger("RunDirection", 0); anim.SetInteger("RunDirection", 0);
    }
    public void OnTriggerStay(Collider other)
    {
        if (unlockedSkill && other.gameObject.tag == "Wall")
        {
            if (canWallJump)
            {
                onWall = true;
            }
            else
            {
                onWall = false;
                wallrunGravity = startingMass;
            }
        }
    }

    public void WallRun()
    {
        

        if (Input.GetButton("Jump") && onWall)
        {
            wallJumping = true;
            wallrunGravity = startingMass;
        }
        else
        {
            wallJumping = false;
        }

        if (wallCooldown <= 0)
        {
            canWallJump = true;
        }
    }
}