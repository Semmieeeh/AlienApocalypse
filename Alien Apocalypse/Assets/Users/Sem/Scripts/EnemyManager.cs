using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

    public float waveStartTimeCounter;
    public float cooldownCounter;

    public void Start()
    {
        instance = this;
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        waveStartTime = 10;
        waveSize = 5;

        waveStartTimeCounter = waveStartTime;
        cooldownCounter = waveCooldown;
        if (photonView.IsMine)
        {
            waveStatusText = GameObject.Find("WaveText").gameObject.GetComponent<TextMeshProUGUI>();
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            wavesStarted = true;
            isInCooldown = true;
            StartCoroutine(nameof(StartEnemyWaves));
        }
    }

    public void Update()
    {
        cooldownCounter -= Time.deltaTime;
        UpdateUIText();
    }

    public void UpdateUIText()
    {
        if (photonView.IsMine)
        {
            if (isInCooldown == true)
            {
                waveStatusText.rectTransform.sizeDelta = new Vector2(250, 60);
                waveStatusText.text = "Cooldown: " + cooldownCounter.ToString("F1");
            }

            if(isInCooldown == false)
            {
                if(waveStatusText != null)
                {
                    waveStatusText.rectTransform.sizeDelta = new Vector2(300, 60);
                    waveStatusText.text = "Wave Start: " + cooldownCounter.ToString("F1");
                }
            }
           
            if(cooldownCounter <= 0 && enemiesSpawning == true)
            {
                if (waveStatusText != null)
                {
                    waveStatusText.text = "Enemies Spawning";
                }
            }
        }
    }


    public IEnumerator StartEnemyWaves()
    {
        while (true)
        {
            isInCooldown = false;
            cooldownCounter = waveStartTime;
            enemiesSpawning = false;
            yield return new WaitForSeconds(waveStartTime);
            
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
            yield return new WaitForSeconds(waveCooldown);
            
            
        }
    }

    public void SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            
            int enemyIndex = Random.Range(0, enemiesToSpawn.Length);
            GameObject enemyObj = PhotonNetwork.Instantiate(enemiesToSpawn[enemyIndex].name, spawnPoint.position, spawnPoint.rotation);
            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();
            enemyHealth.instance = this;
        }
    }
}