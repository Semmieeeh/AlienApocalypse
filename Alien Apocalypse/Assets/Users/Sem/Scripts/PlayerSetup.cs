using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;
    public GameObject uiVanStefan;
    public GameObject cam;
    public GameObject nicknameTextObject;
    public string nickname = "Unnamed";
    public TextMeshPro nicknameText;
    public GameObject weapon;
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        uiVanStefan.SetActive(true);
        weapon.layer = 7;
        gameObject.layer = 3;
        cam.GetComponent<Camera>().enabled = true;
        cam.GetComponent<MouseLook>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;
        cam.GetComponent<MouseLook>().cam2.SetActive(true);
        nicknameTextObject.SetActive(false);
    }

    [PunRPC]
    public void SetNickname(string name)
    {
        nickname = name;
        nicknameText.text = nickname;
    }
}
