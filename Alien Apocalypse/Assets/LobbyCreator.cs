using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyCreator : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    UIPopup popup;
    public void TryCreateRoom ( )
    {
        if ( ValidateInput (inputField.text) )
        {
            CreateRoom ( );
        }
        else
        { 
            popup.Popup ("Invalid name!", $"Cannot create lobby with name {inputField.text}" );
        }
    }

    public void EnableCreationScreen ( )
    {
        if ( PlayerName.ValidNickName ( ) )
        {
            EnableScreen ( );
        }
        else
        {
            popup.Popup ("Invalid name!", $"Please insert a valid nickname!");
        }
    }

    public void EnableScreen ( )
    {
        gameObject.SetActive (true);
    }

    public void DisableScreen ( )
    {
        gameObject.SetActive (false);
    }

    private void CreateRoom ( )
    {
        throw new System.NotImplementedException ("TODO: SEM EN/OF ROBIN ZORG DAT DE SPELER EEN ROOM KAN MAKEN EN JOINEN KLIK OP DE ERROR MESSAGE!");
    }

    public bool ValidateInput(string input )
    {
        //Als je de regels voor een lobby naam wilt aanpassen doe dat hier
        if ( input == null || input.Length <= 0 ) 
            return false;
        
        return true;
    }
}
