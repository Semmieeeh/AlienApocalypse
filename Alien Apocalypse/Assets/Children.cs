using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Children : MonoBehaviourPunCallbacks
{

    public GameObject[] objects;
    private void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject obj in objects)
            {
                obj.layer = 7;
            }
        }
        
    }

}
