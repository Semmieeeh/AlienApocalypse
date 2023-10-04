using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour, INicknameCallback
{
    [SerializeField]
    TMP_InputField input;
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
        PlayerName.TrySetNickName( name );
    }
}
