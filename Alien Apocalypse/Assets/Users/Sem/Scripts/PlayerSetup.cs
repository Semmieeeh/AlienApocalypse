using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Linq;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Movement movement;
    public Material[] playerColors;
    public GameObject playerBody;
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
    public FollowRotation rot2;
    public GameObject localUI;
    Transform t;
    public Camera specCam;
    public int number;
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        number = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        uiVanStefan.SetActive(true);
        weapon.layer = 7;
        gameObject.layer = 3;
        foreach (var part in playerParts)
        {
            part.layer = 13;
        }
        specCam.gameObject.SetActive(false);
        specCam.name = "SpecCam";
        //Destroy(specCam);
        cam.GetComponent<Camera>().enabled = true;
        cam.GetComponent<MouseLook>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;
        cam.GetComponent<MouseLook>().cam2.SetActive(true);
        nicknameTextObject.SetActive(false);
        StartCoroutine(nameof(WaitForEnemyObj));
        inputObject.SetActive(true);
        rot.enabled = true;
        rot2.enabled = true;
        Vector3 v = new Vector3(0,0,0);
        GameObject.Find("SpectatorManager").GetComponent<SpectatorMode>().enabled = true;
        GameObject.Find("SpectatorManager").GetComponent<SpectatorMode>().myPlayer = gameObject;
        photonView.RPC("SetNickname", RpcTarget.All, PhotonNetwork.NickName);
        photonView.RPC("SetPlayerColor", RpcTarget.All);
        gameObject.tag = "Player";
        if(!TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Rigidbody rig = gameObject.AddComponent<Rigidbody>();
            rig = GetComponent<Rigidbody>();
            rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rig.interpolation = RigidbodyInterpolation.Interpolate;
            rig.freezeRotation = true;
        }
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

    [PunRPC]
    public void SetPlayerColor()
    {
        //playerBody.GetComponent<Renderer>().material = playerColors[number];
        //PlayerSetup[] setup = FindObjectsOfType<PlayerSetup>();
        //foreach(PlayerSetup p in setup)
        //{
        //    if(p != this.gameObject.GetComponent<PlayerSetup>())
        //    {
        //        p.playerBody.GetComponent<Renderer>().material = p.playerColors[p.number];
        //    }
        //}
    }
}
