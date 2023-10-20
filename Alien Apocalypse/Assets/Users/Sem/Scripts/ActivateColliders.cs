using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActivateColliders : MonoBehaviourPunCallbacks
{
    public GameObject colliders;
    public Transform spawnPos;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(15f);
        photonView.RPC("Island", RpcTarget.All);
    }
    [PunRPC]
    public void Island()
    {       
        PhotonNetwork.Instantiate(colliders.name,transform.position,transform.rotation);
    }

}
