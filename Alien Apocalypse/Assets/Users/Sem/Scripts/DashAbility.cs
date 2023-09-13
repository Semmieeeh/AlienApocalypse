using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    Rigidbody rb;
    public float dashCooldown;
    private float maxDashCooldown;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxDashCooldown = 3;
    }
    void Update()
    {
        dashCooldown -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Dash();
        }
    }

    public void Dash()
    {
        if (dashCooldown <= 0)
        {
            float velBeforeStop = rb.velocity.magnitude;
            rb.velocity = Vector3.zero;
            rb.AddForce(Camera.main.transform.forward * velBeforeStop * 1.3f, ForceMode.Impulse);
            dashCooldown = maxDashCooldown;
        }
    }
}
