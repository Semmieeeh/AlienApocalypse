using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMode : MonoBehaviour
{
    public bool isSpectator;
    public int currentPlayer;
    public GameObject myCam, renderCam;

    public List<PlayerHealth> playerList = new List<PlayerHealth>();
    public HashSet<PlayerHealth> uniquePlayers = new HashSet<PlayerHealth>();

    private void Update()
    {
        if (isSpectator)
        {
            
        }
        else
        {
            
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentPlayer + 1 < playerList.Count)
            {
                currentPlayer += 1;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (currentPlayer - 1 >= 0)
            {
                currentPlayer -= 1;
            }
        }

        PlayerHealth[] playerHealthComponents = FindObjectsOfType<PlayerHealth>();

        foreach (PlayerHealth playerHealth in playerHealthComponents)
        {
            if (!uniquePlayers.Contains(playerHealth))
            {
                playerList.Add(playerHealth);
                uniquePlayers.Add(playerHealth);
            }
        }
    }
}