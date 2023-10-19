using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    [Header("Beacon State")]
    public BeaconState beaconState;
    public enum BeaconState
    {
        neutral,
        enemyControll,
        playerControll,
    }

    [Header("Sphere Settings")]
    public float radius;
    public LayerMask hitLayer;

    [Header("Beacon")]
    public List<GameObject> enemies;

    public int totalScore;

    public float changeScorePerPlayer;
    public float changeScorePerEnemy;
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

    void FixedUpdate()
    {
        CheckInRange();
    }

    void CheckInRange()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, radius, hitLayer);

        foreach(Collider col in hitCollider)
        {
            enemies.Add(col.gameObject);
        }
    }

    void AddPoints()
    {
        if(Time.time - startTime > pointsAddedInterval)
        {
            //if()

            startTime = Time.time;
        }
    }
}