using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyManager : MonoBehaviour
{

    public bool canSpawn = true;
    public GameObject enemy;
    public Transform spawnPoint;


    [PunRPC]


    public void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (canSpawn == true && PhotonNetwork.CurrentRoom.PlayerCount >1)
            {
                GameObject enemyObj = PhotonNetwork.Instantiate(enemy.name, spawnPoint.position, Quaternion.identity);
                canSpawn = false;
            }
        }
    }
}
