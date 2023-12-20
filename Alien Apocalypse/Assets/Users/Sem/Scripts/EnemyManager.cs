using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviourPunCallbacks
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
    float multiplier = 0;
    public GameObject ufo;

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject u = PhotonNetwork.Instantiate(ufo.name, new Vector3(0, 500, 0), Quaternion.identity);
            if (PhotonNetwork.IsMasterClient)
            {
                u.GetComponent<UfoSpawner>().enabled = true;
            }
            u.transform.parent = gameObject.transform;
            waveStartTime = 20;
            instance = this;
            multiplier = 1;
            startedTheWaves = false;
            waveSize = 1;
            curSpawnPos = spawnPoints[0];
            waveStartTimeCounter = waveStartTime;
            cooldownCounter = waveCooldown;
            if (startedTheWaves == false && PhotonNetwork.CurrentRoom.PlayerCount > 0)
            {
                wavesStarted = true;
                isInCooldown = true;
                enemiesSpawning = false;
                StartCoroutine(nameof(StartEnemyWaves));
                startedTheWaves = true;
                Debug.Log("Started waves");
            }
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
            for (int i = 0; i < waveSize * PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                photonView.RPC("SpawnEnemies", RpcTarget.All, 1);
                enemiesSpawning = true;
                yield return new WaitForSeconds(spawnSpeed);
            }

            isInCooldown = true;
            enemiesSpawning = false;
            cooldownCounter = waveCooldown;
            wavesCompleted++;
            waveSize += 3 * PhotonNetwork.CurrentRoom.PlayerCount;
            curSpawnPos = spawnPoints[Random.Range(0, 3)].transform;
            yield return new WaitForSeconds(waveCooldown);
            multiplier = multiplier * 1.15f;
            
            
        }
    }
    [PunRPC]
    public void SpawnEnemies(int enemyCount)
    {
        if(PhotonNetwork.IsMasterClient)
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
                GameObject enemyObj = PhotonNetwork.Instantiate(enemiesToSpawn[enemyIndex].name, curSpawnPos.position, curSpawnPos.rotation);
                EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();
                enemyHealth.multiplier = multiplier;
                enemyObj.GetComponent<EnemyAiTest>().attackDamage = enemyObj.GetComponent<EnemyAiTest>().attackDamage * multiplier;

                enemyHealth.instance = this;
            }
        }
    }
}