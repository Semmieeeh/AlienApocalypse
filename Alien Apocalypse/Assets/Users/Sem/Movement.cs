using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;
    public float jumpHeight = 5f;

    private bool sprinting;
    private bool jumping;
    private bool grounded = false;
    private Vector2 input;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");
    }

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
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
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * 0.2f : walkSpeed * 0.2f), ForceMode.Force);
            }
        }
        grounded = false;
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
            return velocityChange;
        }
        else
        {
            return new Vector3();
        }
    }
}
