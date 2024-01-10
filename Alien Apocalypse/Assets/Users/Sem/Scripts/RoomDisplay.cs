//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//public class RoomDisplay : MonoBehaviour
//{
//    [SerializeField]
//    TextMeshProUGUI serverName, players, status, connection;


//    public void SetServer ( room )
//    {
//        this.room = room;
//        serverName.text = room.Name;
//        players.text = $"{room.PlayerCount} / {room.MaxPlayers}";

//        status.text = room.IsOpen ? "Available" : "Closed";

//        connection.text = Random.Range (5, 34).ToString();
//    }

//    public void TryJoinRoom ( )
//    {
//        if ( !room.IsOpen )
//        {
//            UIPopup.Instance.Popup ("Join lobby failed!", "Lobby is full!");
//        }
//        else if ( !PlayerName.ValidNickName ( ) )
//        {
//            UIPopup.Instance.Popup ("Invalid name!", $"Please insert a valid nickname!");
//        }
//        else
//        {
//            JoinRoom ( );
//        }
//    }
//    private void JoinRoom ( )
//    {
//        RoomList.instance.JoinRoomByName(serverName.text);
        
//    }
//}
