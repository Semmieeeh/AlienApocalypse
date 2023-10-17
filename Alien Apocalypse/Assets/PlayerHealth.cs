using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [Header("General Health Modifiers")]
    public float health;
    public float maxHealth;
    public float healthRegenAmount;
    public float minHealth;
    public bool godMode;
    public bool revive;
    [Header("Downed Variables")]
    public float movementSpeedMultiplier;
    public float healthDecrease;
    public bool knocked;
    public CapsuleCollider c;
    public BoxCollider b;
    private float height;
    [Header("Animator")]
    public Animator animator;
    public Animator arms;
    public GameObject animatorObj;
    public GameObject robot;
    public GameObject weapons;
    public Vector3 robotPos;
    public Vector3 otherPos;
    public HealthBar healthBar;
    private float lastHit;

    public enum PlayerState
    {
        alive,
        downed,
        dead
    }
    public PlayerState state;
    void Start()
    {
        robotPos = robot.transform.localPosition;
        otherPos = animatorObj.transform.localPosition;
        height = c.height;
        healthDecrease = 5f;
        minHealth = 0;
        maxHealth = 100;
        health = maxHealth;
        state = PlayerState.alive;
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.SetValue(health);

        switch (state)
        {

            case PlayerState.alive:
                UpdateHealth(health);
                lastHit -= Time.deltaTime;
                if(lastHit <0 && health < maxHealth)
                {
                    health += healthRegenAmount *Time.deltaTime;
                }
                break;

            case PlayerState.downed:

                
                health -= healthDecrease * Time.deltaTime;
                GetComponent<DashAbility>().enabled = false;
                GetComponent<Grappling>().enabled = false;
                GetComponent<WallRunning>().enabled = false;
                UpdateHealth(health);
                if(health <= 0)
                {
                    state = PlayerState.dead;
                }
                break;

            case PlayerState.dead:

                Debug.Log("You died bitch nigga");
                break;

        }
        if (revive)
        {
            photonView.RPC("Revive",RpcTarget.All);
            revive = false;

        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        //update health text object
        health -= damage;
        lastHit = 3;
        
        if (health <= 0)
        {
            Downed();
        }
    }
    [PunRPC]
    public void Downed()
    {
        if (!godMode)
        {
            
            state = PlayerState.downed;
            c.height = height * 0.1f;
            knocked = true;
            Vector3 k = new Vector3(0, 0.6f, 0);
            b.center = k;
            Vector3 v = new Vector3(0, 1, 0);
            c.center = v;
            health = maxHealth;
            animator.SetTrigger("Downed");
            GetComponent<Movement>().downed = true;
            robot.transform.localPosition = new Vector3(0, 0.65f, -1f);
            animatorObj.transform.localPosition = new Vector3(0.045f, 0.658f, -0.331f);
            weapons.SetActive(false);
        }

    }
    [PunRPC]
    public void Revive()
    {
        Vector3 newPos = transform.position;
        newPos.y += 5;
        transform.position = newPos;
        GetComponent<DashAbility>().enabled = true;
        GetComponent<Grappling>().enabled = true;
        GetComponent<WallRunning>().enabled = true;
        state = PlayerState.alive;
        c.height = height;
        GetComponent<Movement>().downed = false;

        knocked = false;
        Vector3 k = new Vector3(0, -0.9529285f, 0);
        b.center = k;
        Vector3 v = new Vector3(0, 0, 0);
        c.center = v;
        robot.transform.localPosition = new Vector3(0, -1, 0);
        health = maxHealth * 0.25f;
        animator.SetTrigger("Revived");
        arms.SetTrigger("Revived");
        
        animatorObj.transform.localPosition = otherPos;
        weapons.SetActive(true);
    }
    [PunRPC]
    public void UpdateHealth(float h)
    {
        health = h;
        //health slider ding van stefan
    }
}
