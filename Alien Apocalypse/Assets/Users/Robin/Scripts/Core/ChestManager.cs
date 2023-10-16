using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviourPunCallbacks
{
    public List<Chest> chests;
    public Chest currentChest;

    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).TryGetComponent<Chest>(out Chest chest))
            {
                chests.Add(chest);
                chest.chestManager = this;
            }
        }

        photonView.RPC(nameof(NewChest), RpcTarget.All);
    }

    [PunRPC]
    public void NewChest()
    {
        int randomValue = Random.Range(0, chests.Count);

        for(int i = 0; i < chests.Count; i++)
        {
            if(i == randomValue)
            {
                if(!chests[i].opened)
                {
                    photonView.RPC(nameof(SetChestActive), RpcTarget.All, i, true);
                }
            }
        }
    }

    [PunRPC]
    void SetChestActive(int index, bool active)
    {
        chests[index].gameObject.SetActive(active);
    }
}
