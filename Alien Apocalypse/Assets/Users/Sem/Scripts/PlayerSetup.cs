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
    public TextMeshProUGUI waveStatusTextObj;
    public string nickname = "Unnamed";
    public TextMeshPro nicknameText;
    public EnemyManager enemy;
    public GameObject weapon;
    public GameObject myWeapon;
    public GameObject[] playerParts;
    public GameObject inputObject;
    public FollowRotation rot;
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        
        uiVanStefan.SetActive(true);
        weapon.layer = 7;
        gameObject.layer = 3;
        foreach (var part in playerParts)
        {
            part.layer = 13;
        }
        cam.GetComponent<Camera>().enabled = true;
        cam.GetComponent<MouseLook>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;
        cam.GetComponent<MouseLook>().cam2.SetActive(true);
        nicknameTextObject.SetActive(false);
        StartCoroutine(nameof(WaitForEnemyObj));
        inputObject.SetActive(true);
        rot.enabled = true;
    }

    [PunRPC]
    public void SetNickname(string name)
    {
        nickname = name;
        nicknameText.text = nickname;
    }
    public IEnumerator WaitForEnemyObj()
    {
        yield return new WaitForSeconds(0.5f);
        if (GameObject.Find("EnemyManager(Clone)") != null)
        {
            enemy = GameObject.Find("EnemyManager(Clone)").gameObject.GetComponent<EnemyManager>();
        }
        if (enemy != null)
        {
            enemy.waveStatusText = waveStatusTextObj;
        }
    }
}
