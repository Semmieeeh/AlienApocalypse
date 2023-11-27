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

    public void Start()
    {
        
        waveStatusText = GameObject.Find("WaveText").gameObject.GetComponent<TextMeshProUGUI>();
        if (PhotonNetwork.IsMasterClient)
        {
            waveStartTime = 20;
            instance = this;
            multiplier = 1;
            startedTheWaves = false;
            waveSize = 5;
            curSpawnPos = spawnPoints[0];
            waveStartTimeCounter = waveStartTime;
            cooldownCounter = waveCooldown;
            if (photonView.IsMine)
            {
                if (PhotonNetwork.IsMasterClient && startedTheWaves == false && PhotonNetwork.CurrentRoom.PlayerCount > 0)
                {
                    wavesStarted = true;
                    isInCooldown = true;
                    photonView.RPC(nameof(UpdateIsInCooldown), RpcTarget.AllBuffered, isInCooldown);
                    enemiesSpawning = false;
                    photonView.RPC(nameof(UpdateIsInCooldownTwo), RpcTarget.AllBuffered, enemiesSpawning);
                    StartCoroutine(nameof(StartEnemyWaves));
                    startedTheWaves = true;
                }
            }

            


        }
        


    }
    private void Update()
    {
        cooldownCounter -= Time.deltaTime;
    }
    [PunRPC]
    public void UpdateIsInCooldown(bool newValue)
    {
        isInCooldown = newValue;

    }

    [PunRPC]
    public void UpdateIsInCooldownTwo(bool newValue)
    {
        enemiesSpawning = newValue;

    }

    public IEnumerator StartEnemyWaves()
    {
        while (true)
        {
            isInCooldown = false;
            photonView.RPC(nameof(UpdateIsInCooldown), RpcTarget.AllBuffered, isInCooldown);
            cooldownCounter = waveStartTime;
            enemiesSpawning = false;
            photonView.RPC(nameof(UpdateIsInCooldownTwo), RpcTarget.AllBuffered, enemiesSpawning);
            yield return new WaitForSeconds(waveStartTime);
            waveStartTime = 5;
            for (int i = 0; i < waveSize * PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                SpawnEnemies(1);
                enemiesSpawning = true;
                photonView.RPC(nameof(UpdateIsInCooldownTwo), RpcTarget.AllBuffered, enemiesSpawning);
                yield return new WaitForSeconds(spawnSpeed);
            }

            isInCooldown = true;
            photonView.RPC(nameof(UpdateIsInCooldown), RpcTarget.AllBuffered, isInCooldown);
            enemiesSpawning = false;
            photonView.RPC(nameof(UpdateIsInCooldownTwo), RpcTarget.AllBuffered, enemiesSpawning);
            cooldownCounter = waveCooldown;
            wavesCompleted++;
            waveSize += 3 * PhotonNetwork.CurrentRoom.PlayerCount;
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
            if(wavesCompleted <= enemiesToSpawn.Length)
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