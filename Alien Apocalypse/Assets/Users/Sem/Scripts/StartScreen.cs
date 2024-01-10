using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviour
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

    [SerializeField]
    TextMeshProUGUI roomsText, populationText;

    //List<RoomDisplay> roomUIs = new ( );




    /// <summary>
    /// Tries to connect to server
    /// </summary>
    public void Connect ( )
    {
        //currentTimeOut = 0;
        //gameObject.SetActive (true);
        //SetLoadingScreen ("Connecting...");

        //PhotonNetwork.ConnectUsingSettings ( );
    }

    private void Update ( )
    {
        
    }

    

    

    
   


    

   
    
    public void SetPopulationText(int pop )
    {
        populationText.text = $"Population: {pop}";
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
