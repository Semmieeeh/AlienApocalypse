 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public bool canSpawn = true;
    public GameObject[] enemiesToSpawn;
    public int enemyIndex;
    public int check;
    public Transform spawnPoint;

    public List<EnemyHealth> enemies = new List<EnemyHealth>();

    [Space]
    [Header("Wave info")]
    public int waveSize;
    public int maxWaveSize = 120;
    public float wavesCompleted;
    public float waveTimeLimit;
    public float timePassed;
    public float spawnSpeed;
    public float waveStartTime;
    public float waveCooldown;
    public bool wavesStarted;

    private void Start()
    {
        instance = this;
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        waveStartTime = 10;
        waveCooldown = 10;
        waveSize = 5;
        spawnSpeed = 1;
        
    }

    public void Update()
    {
        // Track time passed
        timePassed += Time.deltaTime;
        if(wavesStarted == false && PhotonNetwork.CurrentRoom.PlayerCount >0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(StartEnemyWaves());
                wavesStarted = true;
            }
        }
    }

    private IEnumerator StartEnemyWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveStartTime);
            for (int i = 0; i < waveSize * PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                SpawnEnemies(1);
                yield return new WaitForSeconds(spawnSpeed);
            }
            yield return new WaitForSeconds(waveCooldown);
            waveSize += 5;
            timePassed = 0;
        }
    }

    private void SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            
            enemyIndex = Random.Range(0, enemiesToSpawn.Length);            
            GameObject enemyObj = PhotonNetwork.Instantiate(enemiesToSpawn[enemyIndex].name, spawnPoint.position, Quaternion.identity);
            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();

            enemyHealth.instance = this;           
        }
    }

}
