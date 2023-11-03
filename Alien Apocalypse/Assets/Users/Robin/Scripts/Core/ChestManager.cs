using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class ChestManager : MonoBehaviourPunCallbacks
{
    public GameObject chestPrefab;
    public List<Chest> chests;
    public Chest currentChest;

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();    

        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject chestInst = PhotonNetwork.Instantiate(chestPrefab.name, transform.GetChild(i).transform.localPosition, transform.GetChild(i).transform.localRotation);

            chestInst.gameObject.SetActive(false);
            chestInst.transform.parent = transform.GetChild(i).transform;

            if(chestInst.TryGetComponent<Chest>(out Chest chest))
            {
                chests.Add(chest);
                chest.chestManager = this;
            }
        }

        photonView.RPC("NewChest", RpcTarget.All);
    }

    public void New()
    {
        foreach(Chest chest in chests)
        {
            if(chest == currentChest)
            {
                chests.Remove(chest);
                break;
            }
        }

        currentChest = null;

        photonView.RPC("NewChest", RpcTarget.All);
    }

    [PunRPC]
    void NewChest()
    {
        int randomValue = Random.Range(0, chests.Count - 1);

        for(int i = 0; i < chests.Count; i++)
        {
            if(i == randomValue)
            {
                if(!chests[i].opened)
                {
                    chests[i].gameObject.SetActive(true);
                    currentChest = chests[i];

                    return;
                }
            }
        }
    }
}
