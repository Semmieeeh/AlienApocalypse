using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPing : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textElement;

    private void FixedUpdate()
    {
        //textElement.text = "Ping: " + (PhotonNetwork.IsConnected? PhotonNetwork.GetPing().ToString() : "??");
    }
}
