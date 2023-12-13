using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ChestManager : MonoBehaviourPunCallbacks
{
    public GameObject chestPrefab;
    public List<Chest> chests;
    public Chest currentChest;
    public Chest oldChest;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject chestInst = PhotonNetwork.Instantiate(chestPrefab.name, transform.GetChild(i).transform.localPosition, transform.GetChild(i).transform.localRotation);

                chestInst.gameObject.SetActive(false);
                chestInst.transform.parent = transform.GetChild(i).transform;

                if (chestInst.TryGetComponent<Chest>(out Chest chest))
                {
                    chests.Add(chest);
                    chest.chestManager = this;
                }
            }

            photonView.RPC("NewChest", RpcTarget.All);
        }
    }

    public void New()
    {
        photonView.RPC("NewChest", RpcTarget.All);
    }

    [PunRPC]
    void NewChest()
    {
        for(int i = 0; i < chests.Count; i++)
        {
            if(chests[i] == oldChest)
            {
                GameObject newChest = PhotonNetwork.Instantiate(chestPrefab.name, chests[i].transform.position, chests[i].transform.rotation);

                newChest.gameObject.SetActive(false);
                newChest.transform.parent = chests[i].transform.parent;

                if(newChest.TryGetComponent<Chest>(out Chest chest1))
                {
                    PhotonNetwork.Destroy(chests[i].gameObject);

                    chests[i] = chest1;
                    chest1.chestManager = this;
                }

                //break;
            }
        }

        oldChest = currentChest;
        currentChest = null;

        bool repeat = true;
        while(repeat)
        {
            int randomValue = Random.Range(0, chests.Count - 1);

            for(int i = 0; i < chests.Count; i++)
            {
                if(i == randomValue)
                {
                    if(chests[i] == oldChest)
                    {
                        break;
                    }

                    if(!chests[i].opened)
                    {
                        chests[i].gameObject.SetActive(true);
                        currentChest = chests[i];

                        repeat = false;
                        return;
                    }
                }
            }
        }
    }
}
