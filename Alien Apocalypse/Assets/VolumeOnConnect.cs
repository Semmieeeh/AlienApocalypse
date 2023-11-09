using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VolumeOnConnect : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        GetComponent<AudioSource>().volume = 1; 
    }
}
