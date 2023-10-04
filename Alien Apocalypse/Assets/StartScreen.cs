using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject loadingScreen;

    [SerializeField]
    TextMeshProUGUI loadingText;

    [SerializeField]
    float maxTimeOut;

    float currentTimeOut;

    bool foundLobbies;

    [SerializeField]
    UIPopup popup;

    [SerializeField]
    GameObject roomPrefab;

    [SerializeField]
    Transform roomsParent;

    [SerializeField]
    GameObject noOneOnline;

    List<RoomDisplay> roomUIs = new ( );



    private List<RoomInfo> rooms = new ( );

    /// <summary>
    /// Tries to connect to server
    /// </summary>
    public void Connect ( )
    {
        currentTimeOut = 0;
        gameObject.SetActive (true);
        SetLoadingScreen ("Connecting...");

        PhotonNetwork.ConnectUsingSettings ( );
    }

    private void Update ( )
    {
        if ( !foundLobbies)
        {
            currentTimeOut += Time.deltaTime;

            if ( currentTimeOut >= maxTimeOut )
            {
                Popup ("Server Timeout!", "Cannot establish connection to server under specific time!", true);
            }
        }
    }

    /// <summary>
    /// Callback function whenever the player succesfully connected to the server
    /// </summary>
    public override void OnConnectedToMaster ( )
    {
        SetLoadingScreen ("Finding lobbies...");
        PhotonNetwork.JoinLobby ( );
    }

    public async void Disconnect ( )
    {
        SetLoadingScreen ("Disconnecting...");

        if ( PhotonNetwork.InLobby )
        {
            PhotonNetwork.LeaveLobby ( );
            await Task.WhenAll (LeftLobby ( ));

        }


        if ( PhotonNetwork.IsConnected )
        {
            Debug.Log ("Disconnecting");

            PhotonNetwork.Disconnect ();
        }
    }

    public async Task LeftLobby()
    {
        while ( PhotonNetwork.InLobby )
        {
            await Task.Yield ( );
        }
    }

    public override void OnDisconnected ( DisconnectCause cause )
    {
        Debug.Log ("Disconnected");

        // Check if disconnection is intentional
        switch ( cause )
        {
            case DisconnectCause.None:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByClientLogic:
            case DisconnectCause.ApplicationQuit:
                // => The player intentionally disconnected
                break;

            default:
                // => The player disconnected unintentionally 
                Popup ("Disconnected!", $"Disconnected from server! cause : {cause}", true);
                break;
        }
        MainMenu.Instance.ToMainMenu ( );

    }
    public override void OnJoinRoomFailed ( System.Int16 returnCode, System.String message )
    {
        Popup ("Join room Failed!", $"Failed to join room: {message}, return code: {returnCode}", false);
    }


    public override void OnRoomListUpdate ( List<RoomInfo> roomList )
    {
        if ( rooms.Count <= 0 )
        {
            rooms = roomList;
        }
        else
        {
            foreach ( var room in roomList )
            {
                for ( int i = 0; i < rooms.Count; i++ )
                {
                    if ( rooms[i].Name == room.Name )
                    {
                        List<RoomInfo> newList = rooms;


                        if ( room.RemovedFromList )
                        {
                            newList.Remove (newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }
                        rooms = newList;
                    }
                }
            }
        }

        OnLobbiesFound ( );

        UpdateRoomUI ( );

        Debug.Log ("Room list updated!");
    }

    public void OnLobbiesFound ( )
    {
        ToggleLoadingScreen (false);

        foundLobbies = true;

    }

    void UpdateRoomUI ( )
    {
        foreach ( var roomUI in roomUIs )
        {
            Destroy (roomUI.gameObject);
        }

        roomUIs.Clear();

        if(rooms.Count <= 0 )
        {
            noOneOnline.SetActive (true);
            return;
        }
        noOneOnline.SetActive (false);

        for(int i = 0; i < rooms.Count; i++ )
        {
            RoomDisplay display = Instantiate (roomPrefab, roomsParent).GetComponent<RoomDisplay> ( );
            roomUIs.Add (display);

            display.SetServer (rooms[i]);
        }
    }

    public void Popup ( string cause, string message, bool toMainMenu )
    {
        if ( toMainMenu )
            MainMenu.Instance.ToMainMenu ( );

        popup.Popup (cause, message);
    }

    /// <summary>
    /// Toggles the loading screen on/off
    /// </summary>
    /// <param name="enabled"></param>
    void ToggleLoadingScreen ( bool enabled )
    {
        loadingScreen.SetActive (enabled);
    }

    /// <summary>
    /// Sets the message the loading screen should display
    /// </summary>
    /// <param name="message"></param>
    public void SetLoadingScreen ( string message )
    {
        loadingText.text = message.ToUpper ( );

        ToggleLoadingScreen (true);
    }
}
