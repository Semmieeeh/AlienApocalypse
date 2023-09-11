using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;
    public GameObject cam;
    public string nickname;
    public TextMeshPro nicknameText;
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        cam.SetActive(true);
    }

    [PunRPC]
    public void SetNickname(string name)
    {
        nickname = name;
        nicknameText.text = nickname;
    }
}
