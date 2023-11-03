using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public BeaconManager beaconManager;

    [Header("Sphere Settings")]
    public float radius;
    public LayerMask hitLayer;

    [Header("Beacon")]
    public List<GameObject> players;
    public List<GameObject> enemies;

    public int totalScore;

    public int changeScorePlayer;
    public int changeScoreEnemy;
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

    void AddPoints()
    {
        if(Time.time - startTime > pointsAddedInterval)
        {
            CheckInRange();

            if(enemies.Count > 0 && players.Count == 0)
            {
                totalScore -= changeScoreEnemy;

                if(totalScore >= 100)
                {
                    totalScore = 100;
                }
            }
            else if(enemies.Count == 0)
            {
                totalScore += changeScorePlayer;

                if(totalScore <= -100)
                {
                    totalScore = -100;
                }
            }

            beaconManager.BeaconsCondition();

            startTime = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}