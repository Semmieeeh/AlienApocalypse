using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject enemyManager;
    [Space]
    public Transform spawnPoint;
    [Space]
    public GameObject roomCam;
    [Space]
    public GameObject connectingUI;
    public GameObject cantConnectUi;
    public GameObject nicknameUI;
    public string roomNameToJoin;
    public bool spawnedEnemy = false;
    public GameObject island;
    public Transform islandPos;

    public string nickname = "Unnamed";
    void Start()
    {
        spawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
    }
    public void SetNickName(string name)
    {
        nickname = name;
    }

    public GameObject pointsManager;
    public GameObject vanObj;
    public GameObject chests;
    float difficulty;
    public void Easy()
    {
        difficulty = 0.6f;
        SpawnPlayer();

    }
    public void Medium()
    {
        difficulty = 1;
        SpawnPlayer();

    }
    public void Hard()
    {
        difficulty = 1.4f;
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        EnemyManager enemy = Instantiate(enemyManager, Vector3.zero, Quaternion.identity).GetComponent<EnemyManager>();
        enemy.multiplier = difficulty;
        GameObject pointsMan = Instantiate(pointsManager, spawnPoint.position, Quaternion.identity);
        GameObject van = Instantiate(vanObj, new Vector3(2, 0.4f, -22f), Quaternion.identity);
        GameObject chest = Instantiate(chests, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject playerPrefab = Instantiate(prefabs[0], spawnPoint.position, spawnPoint.transform.rotation);
        GameObject player = playerPrefab.transform.GetChild(0).gameObject;
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        //player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.All, nickname);
        gameObject.SetActive(false);
    }
    
}
