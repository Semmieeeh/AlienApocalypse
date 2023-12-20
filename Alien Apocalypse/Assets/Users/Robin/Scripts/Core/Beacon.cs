using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Beacon : MonoBehaviourPunCallbacks
{
    public BeaconManager beaconManager;

    [Header("Sphere Settings")]
    public float radius;
    public LayerMask hitLayer;

    [Header("Beacon")]
    public List<GameObject> players;
    public List<GameObject> enemies;

    [Header("Visualization")]
    public UIFlag flag;
    public float totalScore;

    public float changeScorePlayer;
    public float changeScoreEnemy;
    public float pointsAddedInterval;

    float startTime;

    void Start()
    {
        startTime = Time.time;
        
    }

    void Update()
    {
        AddPoints();
    }

    public Collider[] jort;

    void CheckInRange()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, radius, hitLayer);

        jort = hitCollider;

        players.Clear();
        enemies.Clear();

        int playerInt = LayerMask.NameToLayer("Player");
        int enemyInt = LayerMask.NameToLayer("Enemy");

        foreach(Collider col in hitCollider)
        {
            if(col.gameObject.layer == playerInt)
            {
                players.Add(col.gameObject);
            }
            else if(col.gameObject.layer == enemyInt)
            {
                enemies.Add(col.gameObject);
            }
        }
    }
    public Color c;
    public Color playerColor;
    public Color enemyColor;
    void AddPoints()
    {
        if(Time.time - startTime > pointsAddedInterval)
        {
            CheckInRange();

            if(enemies.Count > 0 && players.Count == 0)
            {
                totalScore -= changeScoreEnemy;
                c = enemyColor;
                if (totalScore >= 100)
                {
                    
                    totalScore = 100;
                    
                }
                if(totalScore <= 0)
                {
                    GameObject.Find("Death Screen").GetComponent<Animator>().SetBool("Active", true);
                    FindObjectOfType<PlayerHealth>().Die();
                }
            }
            else if(enemies.Count == 0)
            {
                totalScore += changeScorePlayer;
                c = playerColor;
                if (totalScore <= 0)
                {
                    
                    totalScore = 0;
                    
                }
            }

            beaconManager.BeaconsCondition();

            startTime = Time.time;
            photonView.RPC("FlagRPC", RpcTarget.All);
        }
    }
    [PunRPC]
    void FlagRPC()
    {
        flag.currentValue = totalScore;
        flag.coloredBorder.GetComponent<Image>().color = c;

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}