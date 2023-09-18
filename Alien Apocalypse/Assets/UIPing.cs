using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMeshProUGUI textElement;

    private void FixedUpdate()
    {
        textElement.text = "Ping" + (PhotonNetwork.IsConnected? PhotonNetwork.GetPing().ToString() : "??");
    }
}
