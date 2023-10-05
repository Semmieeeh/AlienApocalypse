using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerName
{
    private static List<INicknameCallback> nicknameChangedListeners = new();

    
    static UIPopup m_popup;

    static UIPopup popup
    {
        get
        {
            if(m_popup == null) m_popup = Object.FindObjectOfType<UIPopup>();
            return m_popup;
        }
        set
        {
            m_popup = value;
        }
    }
    /// <summary>
    /// The nickname of the local player from the network
    /// </summary>
    public static string NickName
    {
        get
        {
            return PhotonNetwork.NickName;
        }
        set
        {
            PhotonNetwork.NickName = value;
        }
    }

    /// <summary>
    /// This method will check if the new nickname message is valid, and can blur any ban words found
    /// </summary>
    /// <param name="newName"></param>
    public static void TrySetNickName (string newName)
    {
        if ( !ValidateInput (newName) )
        {
            popup.Popup ("Invalid name!", $"Cannot name player with name {newName}");
            return;
        }

        if ( !NameUtility.IsMessageClean (newName) )
        {
            newName = NameUtility.CensorMessage (newName);
        }

        SetNickName(newName);
        
    }

    /// <summary>
    /// This will update the players nickname if the message is valid
    /// </summary>
    /// <param name="newName"></param>
    private static void SetNickName (string newName )
    {
        NickName = newName;
        Debug.Log ("Changed player name!");

        foreach ( var listener in nicknameChangedListeners )
        {
            listener.OnNickNameChanged (NickName);
        }
    }

    public static bool ValidateInput(string input )
    {
        if ( input == null || input.Length <= 0 )
            return false;
        return true;
    }

    public static bool ValidNickName ( )
    {
        return ValidateInput (NickName);
    }

    public static void AddListener(INicknameCallback listener )
    {
        nicknameChangedListeners.Add (listener);
    }

    public static void RemoveListener(INicknameCallback listener )
    {
        if ( nicknameChangedListeners.Contains (listener) )
            nicknameChangedListeners.Remove (listener);
    }
}

public static class NameUtility
{
    /// <summary>
    /// Insert banned words here
    /// </summary>
    public static readonly string[] bannedWords =
    {

    };

    /// <summary>
    /// The character to use to censor bad words found in strings/inputs
    /// </summary>
    public static readonly char CensorMessageCharacter = '*';

    /// <summary>
    /// Will check if a message contains bad words. Bad words can be inputted in NameUtility.bannedWords
    /// </summary>
    /// <param name="message">The message to check if its clean</param>
    /// <returns>True if the message is clean, false if it isnt</returns>
    public static bool IsMessageClean (string message )
    {
        for ( int i = 0; i < bannedWords.Length; i++ )
        {
            if ( message.Contains (bannedWords[i]) )
                return false;
        }
        return true;
    }

    /// <summary>
    /// This function will censor a message, Consider checking if the message is clean first before censoring it using IsMessageClean(string message)
    /// </summary>
    /// <param name="message">The message to censor</param>
    /// <returns>The inputted message but censored if they found bad words</returns>
    public static string CensorMessage ( string message )
    {
        for ( int i = 0; i < bannedWords.Length; i++ )
        {
            if ( message.Contains (bannedWords[i]) )
            {
                string censorMessage = new string (CensorMessageCharacter, bannedWords[i].Length);

                message = message.Replace (bannedWords[i], censorMessage);
            }
        }

        return message;
    }
}

public interface INicknameCallback
{
    /// <summary>
    /// Callback function called when the players name has changed
    /// </summary>
    /// <param name="name">The new nickname</param>
    void OnNickNameChanged ( string name );
}