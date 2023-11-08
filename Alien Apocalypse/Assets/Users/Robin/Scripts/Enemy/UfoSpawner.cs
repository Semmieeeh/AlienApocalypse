using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UfoSpawner : MonoBehaviourPunCallbacks
{
    public GameObject ufoPrefab;
    public float moveSpeed;
    float timeUntilNextSpawn;
    float spawningTime;
    Transform spawnPos;

    public void UfoSpawn(float timeUntilNextSpawn, float spawningTime, Transform spawnPos)
    {
        this.timeUntilNextSpawn = timeUntilNextSpawn;
        this.spawningTime = spawningTime;
        this.spawnPos = spawnPos;

        StartCoroutine(Ufo());
    }

    IEnumerator Ufo()
    {
        GameObject currentUfo = PhotonNetwork.Instantiate(ufoPrefab.name, transform.position, Quaternion.identity);

        bool isMoving = true;

        while(isMoving)
        {
            currentUfo.transform.position = Vector3.Lerp(currentUfo.transform.position, spawnPos.position, moveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }



        yield return null;
    }
}
