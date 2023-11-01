using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [Header("General Health Modifiers")]
    [SerializeField]
    float m_health;

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
    private float lastDmg;
    public GameObject spectatorCam;

    public float Health
    {
        get
        {
            return m_health;
        }
        set
        {
            if (value == m_health)
                return;
            m_health = value;
            OnHealthUpdate();
        }
    }

    public enum PlayerState
    {
        alive,
        downed,
        dead
    }
    public PlayerState state;
    void Start()
    {
        lastDmg = 3;
        robotPos = robot.transform.localPosition;
        otherPos = animatorObj.transform.localPosition;
        height = c.height;
        minHealth = 0;
        maxHealth = 100;
        Health = maxHealth;
        state = PlayerState.alive;
    }

    // Update is called once per frame
    void Update()
    {


        switch (state)
        {
            
            case PlayerState.alive:
                
                lastHit -= Time.deltaTime;
                if(lastHit <0 && Health < maxHealth)
                {
                    Health += healthRegenAmount *Time.deltaTime;
                }
                break;

            case PlayerState.downed:

                GetComponent<DashAbility>().enabled = false;
                GetComponent<Grappling>().enabled = false;
                GetComponent<WallRunning>().enabled = false;
                GetComponent<SlidingAbility>().enabled = false;
                lastDmg -= Time.deltaTime;
                if(lastDmg < 0)
                {
                    TakeDamage(healthDecrease);
                    lastDmg = 3;
                }

                
                if (Health <= 0)
                {
                    state = PlayerState.dead;
                }
                break;

            case PlayerState.dead:
                GetComponent<DashAbility>().enabled = false;
                GetComponent<Grappling>().enabled = false;
                GetComponent<WallRunning>().enabled = false;
                GetComponent<SlidingAbility>().enabled = false;
                GetComponent<Movement>().enabled = false;
                GameObject.Find("SpectatorManager").GetComponent<SpectatorMode>().isSpectator = true;
                Debug.Log("You died");
                break;

        }
        if (revive)
        {
            photonView.RPC("Revive",RpcTarget.All);
            revive = false;
            GetComponent<SpectatorMode>().isSpectator = false;

        }
    }


    void OnHealthUpdate()
    {
        healthBar.SetValue(Health);
        photonView.RPC("UpdateHealth", RpcTarget.All, Health);
    }

    public void TakeDamage(float damage)
    {
        //update health text object
        Health -= damage;
        lastHit = 3;
        
        if (Health <= 0)
        {
            photonView.RPC("Downed", RpcTarget.All);
        }
    }
    [PunRPC]
    public void Downed()
    {
        if (!godMode && !knocked)
        {

            
            state = PlayerState.downed;
            c.height = height * 0.1f;
            knocked = true;
            Vector3 k = new Vector3(0, 0.6f, 0);
            b.center = k;
            Vector3 v = new Vector3(0, 1, 0);
            c.center = v;
            
            animator.SetTrigger("Downed");
            GetComponent<Movement>().downed = true;
            robot.transform.localPosition = new Vector3(0, 0.65f, -1f);
            animatorObj.transform.localPosition = new Vector3(0.045f, 0.658f, -0.331f);
            weapons.SetActive(false);
            Health = maxHealth;
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
        GetComponent<SlidingAbility>().enabled = true;
        state = PlayerState.alive;
        GameObject.Find("SpectatorManager").GetComponent<SpectatorMode>().isSpectator = false;
        Camera.main.GetComponent<MouseLook>().enabled = true;
        c.height = height;
        GetComponent<Movement>().downed = false;

        knocked = false;
        Vector3 k = new Vector3(0, -0.9529285f, 0);
        b.center = k;
        Vector3 v = new Vector3(0, 0, 0);
        c.center = v;
        robot.transform.localPosition = new Vector3(0, -1, 0);
        Health = maxHealth * 0.25f;
        animator.SetTrigger("Revived");
        arms.SetTrigger("Revived");
        
        animatorObj.transform.localPosition = otherPos;
        weapons.SetActive(true);
    }
    [PunRPC]
    public void UpdateHealth(float h)
    {
        Health = h;
        //health slider ding van stefan
    }
}
