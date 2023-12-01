using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DashAbility : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    public float dashCooldown;
    private float maxDashCooldown;
    public bool unlockedSkill;
    public float dashAmount;
    public float multiplier;
    public UIAbility UIAbility;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxDashCooldown = 3;
        dashAmount = 15;
        multiplier = 1;
    }
    void Update()
    {
        
        if(unlockedSkill == true)
        {
            dashCooldown -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                Dash();
            }
        }
    }

    public void Dash()
    {
        if (photonView.IsMine)
        {
            if (dashCooldown <= 0)
            {
                dashAmount = dashAmount * multiplier;

                float velBeforeStop = rb.velocity.magnitude;
                rb.velocity = Vector3.zero;
                this.rb.AddForce(Camera.main.transform.forward * (velBeforeStop + dashAmount), ForceMode.Impulse);
                dashCooldown = maxDashCooldown;
                UIAbility.cooldown = maxDashCooldown;
                UIAbility.Activate();
            }
        }
    }
}
