using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public bool canSpawn = true;
    public GameObject[] enemiesToSpawn;
    public int enemyIndex;
    public int check;
    public Transform[] spawnPoints;

    public List<EnemyHealth> enemies = new List<EnemyHealth>();

    [Space]
    [Header("Wave info")]
    public int waveSize;
    public int maxWaveSize = 120;
    public float wavesCompleted;
    public float waveTimeLimit;
    public float spawnSpeed;
    public float waveStartTime;
    public float waveCooldown;
    public bool wavesStarted;
    public bool isInCooldown;
    public bool enemiesSpawning;
    [Space]
    [Header("UI Element")]
    public TextMeshProUGUI waveStatusText;
    public bool startedTheWaves;
    public Transform curSpawnPos;
    public float waveStartTimeCounter;
    public float cooldownCounter;
    public float multiplier;
    public GameObject ufo;

    public void Start()
    {
        GameObject u = Instantiate(ufo, new Vector3(0, 500, 0), Quaternion.identity);
        u.GetComponent<UfoSpawner>().enabled = true;
        u.transform.parent = gameObject.transform;
        waveStartTime = 20;
        instance = this;
        startedTheWaves = false;
        waveSize = 1;
        curSpawnPos = spawnPoints[0];
        waveStartTimeCounter = waveStartTime;
        cooldownCounter = waveCooldown;
        if (startedTheWaves == false)
        {
            wavesStarted = true;
            isInCooldown = true;
            enemiesSpawning = false;
            StartCoroutine(nameof(StartEnemyWaves));
            startedTheWaves = true;
            Debug.Log("Started waves");
        }


    }
    private void Update()
    {
        cooldownCounter -= Time.deltaTime;
    }

    public IEnumerator StartEnemyWaves()
    {
        while (true)
        {
            isInCooldown = false;
            cooldownCounter = waveStartTime;
            enemiesSpawning = false;
            yield return new WaitForSeconds(waveStartTime);
            waveStartTime = 5;
            for (int i = 0; i < waveSize; i++)
            {
                SpawnEnemies(1);
                enemiesSpawning = true;
                yield return new WaitForSeconds(spawnSpeed);
            }

            isInCooldown = true;
            enemiesSpawning = false;
            cooldownCounter = waveCooldown;
            wavesCompleted++;
            waveSize += 2;
            curSpawnPos = spawnPoints[Random.Range(0, 3)].transform;
            yield return new WaitForSeconds(waveCooldown);
            multiplier = multiplier * 1.15f;
            
            
        }
    }
    public void SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            int max;
            if (wavesCompleted <= enemiesToSpawn.Length)
            {
                max = wavesCompleted.ToInt();
            }
            else
            {
                max = enemiesToSpawn.Length;
            }
            int enemyIndex = Random.Range(0, max);
            transform.GetChild(4).GetComponent<UfoSpawner>().PlayParticle();
            GameObject enemyObj = Instantiate(enemiesToSpawn[enemyIndex], curSpawnPos.position, curSpawnPos.rotation);
            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();

            enemyHealth.multiplier = multiplier;
            if(enemyObj.TryGetComponent<EnemyAiTest>(out EnemyAiTest test))
            {
                test.attackDamage = test.attackDamage * multiplier;
            }
            if(enemyObj.TryGetComponent<GiantAi>(out GiantAi ai))
            {
                ai.attackDamage = ai.attackDamage * multiplier;
            }
            enemyHealth.instance = this;
        }
    }
}