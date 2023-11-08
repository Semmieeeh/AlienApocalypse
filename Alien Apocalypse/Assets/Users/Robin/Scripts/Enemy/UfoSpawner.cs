using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor.Rendering;

public class UfoSpawner : MonoBehaviourPunCallbacks
{
    public GameObject ufoPrefab;
    public float moveSpeed;
    Transform spawnPos;
    EnemyManager m;
    MeshRenderer mesh;
    public ParticleSystem[] partartarticles;
    public float heightOffset;
    bool active;

    private void Start()
    {
        m = transform.parent.GetComponent<EnemyManager>();
        mesh = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        UfoBehaviour();
        Vector3 v = new Vector3(0,200,0);
        transform.Rotate(v * Time.deltaTime);
    }
    public void PlayParticle()
    {
        for (int i = 0; i < partartarticles.Length; i++)
        {
            partartarticles[i].Play();
        }
    }
    void UfoBehaviour()
    {
        if (m.enemiesSpawning == false && m.cooldownCounter <5)
        {
            if(m.cooldownCounter > 0)
            {
                photonView.RPC("UfoActivate", RpcTarget.All);
            }
        }
        else if(m.cooldownCounter >5)
        {
            photonView.RPC("UfoDeactivate", RpcTarget.All);
        }
    }
    [PunRPC]
    void UfoActivate()
    {
        StopAllCoroutines();
        mesh.enabled = true;
        Vector3 desiredPos = new Vector3(m.curSpawnPos.position.x, m.curSpawnPos.position.y, m.curSpawnPos.position.z);
        desiredPos.y += heightOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSpeed * Time.deltaTime);
    }
    [PunRPC]
    void UfoDeactivate()
    {
        StartCoroutine(nameof(MeshAct));
        Vector3 desiredPos = transform.position;
        desiredPos.y = 500;
        transform.position = Vector3.Lerp(transform.position,desiredPos, moveSpeed * Time.deltaTime);
    }
    public IEnumerator MeshAct()
    {
        yield return new WaitForSeconds(3);
        mesh.enabled = false;
    }
    
}
