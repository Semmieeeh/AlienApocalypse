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
    public GameObject enemy;
    public Transform spawnPoint;

    public List<EnemyHealth> enemies = new List<EnemyHealth>();

    private void Start()
    {
        instance = this;
    }

    public void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (canSpawn == true && PhotonNetwork.CurrentRoom.PlayerCount >0)
            {
                GameObject enemyObj = PhotonNetwork.Instantiate(enemy.name, spawnPoint.position, Quaternion.identity);
                
                enemies.Add(enemyObj.GetComponent<EnemyHealth>());
                enemyObj.GetComponent<EnemyHealth>().identity = enemies.Count - 1;
                canSpawn = false;
            }
        }

        
    }
    public void ReassignIdentities()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].identity = i;
        }
    }
}
