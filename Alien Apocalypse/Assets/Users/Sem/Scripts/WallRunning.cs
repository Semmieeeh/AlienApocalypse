using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class WallRunning : MonoBehaviour
{
    public Movement m;
    public Collider collider1;
    public Collider collider2;
    public float wallrunGravity;
    public float wallrunAscendGravity;
    public bool onWall;
    public bool wallJumping;
    public bool jumpedOnWall;
    public float wallCooldown;
    public bool canWallJump;
    public bool hitLeft;
    public bool hitRight;
    public GameObject cameraPivot;
    public float rotateAmount = 20;
    public float normalRotation;
    public Animator anim;
    public bool wallrunningActive;
    public float startingMass;
    void Start()
    {
        wallCooldown = 1;
        normalRotation = Camera.main.transform.rotation.z;
        startingMass = wallrunGravity;
        m.wallrunUnlocked = true;
        
    }
    private void Update()
    {
        WallRun();
        if(wallrunningActive == true)
        {
            wallrunGravity -= 2f * Time.deltaTime;
        }
        m.wallRunning = wallrunningActive;
    }
    void FixedUpdate()
    {
        if (onWall == true)
        {

            if(m.rb != null)
            {
                if (m.sprinting)
                {
                    Vector3 rbVel = new Vector3(m.rb.velocity.x, wallrunGravity, m.rb.velocity.z);
                    m.rb.velocity = rbVel;
                }

                if (m.sprinting && m.input.magnitude > 0.5f)
                {
                    Vector3 rbVel = new Vector3(m.rb.velocity.x, wallrunGravity, m.rb.velocity.z);
                    m.rb.velocity = rbVel;

                }
                if (wallJumping && canWallJump == true)
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

    public void RotateCameraOnCollision()
    {
        if (hitLeft)
        {
            if(m.grounded == false)
            {
                anim.SetInteger("RunDirection", 2);
                wallrunningActive = true;

            }
            else
            {
                anim.SetInteger("RunDirection", 0);
                wallrunningActive = false;
                wallrunGravity = startingMass;  
            }
            Debug.Log("Hit Left collider");

        }
        else if(hitRight)
        {
            if (m.grounded == false)
            {
                anim.SetInteger("RunDirection", 1);
                wallrunningActive = true;
                
            }
            else
            {
                anim.SetInteger("RunDirection", 0);
                wallrunningActive = false;
                wallrunGravity = startingMass;
            }
            Debug.Log("Hit Right collider");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        anim.SetInteger("RunDirection", 0);
        wallrunningActive = false;
        wallrunGravity = startingMass;
    }
    public void OnTriggerStay(Collider other)
    {

        if (m.grounded == false && other.gameObject.tag == "Wall")
        {
            if (canWallJump == true)
            {
                onWall = true;
            }
        }
        else
        {
            onWall = false;
            wallrunGravity = startingMass;

        }
    }


    public void WallRun()
    {
        wallCooldown -= Time.deltaTime;
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
