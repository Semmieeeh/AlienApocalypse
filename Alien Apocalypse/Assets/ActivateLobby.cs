using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateLobby : MonoBehaviour
{
    public GameObject lobby;
    public GameObject startScreen;
    public void ActivateLobbyButton()
    {
        lobby.SetActive(true);
        startScreen.SetActive(false);
    }
}
