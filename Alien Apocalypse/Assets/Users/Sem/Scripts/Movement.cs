using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Speed modifiers")]
    public float walkSpeed = 20f;
    public float sprintSpeed = 30f;
    public float maxVelocityChange = 20f;
    public float turnSpeed;
    public float slowDownForce;
    public float stopTime;
    public bool stopped;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalFov = Camera.main.fieldOfView;
        maxFov = Camera.main.fieldOfView + 10;
    }

    private void Update()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");

        FovChange();

        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }


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

        if(stopTime > 0 && rb.velocity.magnitude < 0.1f)
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

    public void Fire()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            if (hit.transform.gameObject.GetComponent<EnemyHealth>())
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("EnemyTakeDamage", RpcTarget.All, damage);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        grounded = other.gameObject.tag == "Ground";
    }

    private void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping)
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
}