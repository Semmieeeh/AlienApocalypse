using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour, INicknameCallback
{
    [SerializeField]
    TMP_InputField input;
    public RoomManager roomManager;
    private void OnEnable ( )
    {
        PlayerName.AddListener( this );
    }

    private void OnDisable ( )
    {
        PlayerName.RemoveListener( this );
    }

    public void OnNickNameChanged(string name )
    {
        input.text = name;
    }

    public void SetNickName (string name)
    {
        PhotonNetwork.NickName = name;
        roomManager.nickname = name;
    }
}
