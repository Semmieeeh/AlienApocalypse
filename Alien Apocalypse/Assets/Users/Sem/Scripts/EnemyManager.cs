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
    public Transform spawnPoint;

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

    public float waveStartTimeCounter;
    public float cooldownCounter;
    float multiplier = 0;

    public void Start()
    {
        multiplier = 1;
        startedTheWaves = false;
        waveStatusText = GameObject.Find("WaveText").gameObject.GetComponent<TextMeshProUGUI>();
        if (PhotonNetwork.IsMasterClient)
        {
            instance = this;
            spawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
            waveSize = 5;

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

    [PunRPC]
    public void UpdateCooldownCounter(float newCooldown)
    {
        cooldownCounter = newCooldown;
        waveStatusText = GameObject.Find("WaveText").gameObject.GetComponent<TextMeshProUGUI>();
    }
    private float syncCooldownInterval = 0.05f;
    private float lastSyncTime;
    public void Update()
    {


        if (photonView.IsMine)
        {            
            if (PhotonNetwork.IsMasterClient)
            {
                cooldownCounter -= Time.deltaTime;
                photonView.RPC(nameof(UpdateUIText), RpcTarget.All);
                if (Time.time - lastSyncTime >= syncCooldownInterval)
                {
                    lastSyncTime = Time.time;
                    photonView.RPC(nameof(UpdateCooldownCounter), RpcTarget.AllBuffered, cooldownCounter);
                    

                }
            }
            
        }
        
    }
    [PunRPC]
    public void UpdateUIText()
    {
        if (waveStatusText != null)
        {
            if (isInCooldown)
            {
                waveStatusText.rectTransform.sizeDelta = new Vector2(350, 60);
                waveStatusText.text = "Next Wave In: " + cooldownCounter.ToString("F1");
            }
            else
            {
                waveStatusText.rectTransform.sizeDelta = new Vector2(300, 60);
                waveStatusText.text = "Wave Start: " + cooldownCounter.ToString("F1");
            }

            if (cooldownCounter <= 0 && enemiesSpawning == true)
            {
                if (waveStatusText != null)
                {
                    waveStatusText.text = "Enemies Spawning";
                }
            }
        }
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
            //waveSize += 5;
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
            Vector3 v = new Vector3(Random.Range(spawnPoint.position.x + 20f, spawnPoint.position.x - 20f), spawnPoint.position.y, Random.Range(spawnPoint.position.z + 20f, spawnPoint.position.z - 20f));

            GameObject enemyObj = PhotonNetwork.Instantiate(enemiesToSpawn[enemyIndex].name, v, spawnPoint.rotation);
            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();
            enemyHealth.multiplier = multiplier;
            enemyObj.GetComponent<EnemyAiTest>().attackDamage = enemyObj.GetComponent<EnemyAiTest>().attackDamage * multiplier;
            
            enemyHealth.instance = this;
        }
    }
}