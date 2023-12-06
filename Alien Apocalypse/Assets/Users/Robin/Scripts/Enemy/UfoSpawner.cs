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
    public bool sound;

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
    public float time;
    public float interval = 0.2f;
    Vector3 desiredPos;
    bool executed;
    void UfoBehaviour()
    {
        MoveUfo();
        if (m.enemiesSpawning == false && m.cooldownCounter <5)
        {
            if(m.cooldownCounter > 0 && executed == true)
            {
                photonView.RPC("UfoActivate", RpcTarget.All);
                executed = false;
                
            }
        }
        else if(m.cooldownCounter > 5 && executed == false)
        {
            photonView.RPC("UfoDeactivate", RpcTarget.All);
            executed = true;
        }
    }
    public void MoveUfo()
    {
        
        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSpeed * Time.deltaTime);
    }
    [PunRPC]
    void UfoActivate()
    {
        desiredPos = new Vector3(m.curSpawnPos.position.x, m.curSpawnPos.position.y, m.curSpawnPos.position.z);
        desiredPos.y += heightOffset;
        StopAllCoroutines();
        if(sound == false)
        {
            GetComponent<AudioSource>().Play();
            sound = true;
        }
        mesh.enabled = true;
        
    }
    [PunRPC]
    void UfoDeactivate()
    {
        sound = false;
        StartCoroutine(nameof(MeshAct));
        desiredPos = transform.position;
        desiredPos.y = 500;
    }
    public IEnumerator MeshAct()
    {
        yield return new WaitForSeconds(3);
        mesh.enabled = false;
    }
    
}
