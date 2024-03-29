using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMode : MonoBehaviour
{
    public bool isSpectator;
    public int currentPlayer;
    public GameObject myPlayer;
    public List<PlayerHealth> playerList = new List<PlayerHealth>();
    public HashSet<PlayerHealth> uniquePlayers = new HashSet<PlayerHealth>();
    public GameObject specUI;
    public GameObject deathScreen;
    private void Update()
    {
        PlayerHealth[] playerHealthComponents = FindObjectsOfType<PlayerHealth>();

        foreach (PlayerHealth playerHealth in playerHealthComponents)
        {
            if (myPlayer != null)
            {
                if (!uniquePlayers.Contains(playerHealth))
                {
                    playerList.Add(playerHealth);
                    uniquePlayers.Add(playerHealth);
                }
            }
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] == null)
                {
                    playerList.RemoveAt(i);
                }
            }
        }



        //if(Camera.main.TryGetComponent<MouseLook>(out MouseLook mouse))
        //{
        //    mouse.enabled = !isSpectator;
        //}

        if (isSpectator)
        {
            if (playerList.Count > 1)
            {
                playerList[currentPlayer].specUI.SetActive(true);
                if (Input.GetMouseButtonDown(1))
                {
                    if (currentPlayer + 1 <= playerList.Count)
                    {
                        currentPlayer += 1;

                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (currentPlayer - 1 >= 1)
                    {
                        currentPlayer -= 1;
                    }
                }
                Camera.main.transform.position = playerList[currentPlayer].spectatorCam.transform.position;
            }
            else if(playerList.Count == 1)
            {
                GameObject canvas = GameObject.Find("PlayerCanvas");
                canvas.transform.GetChild(4).gameObject.SetActive(true);
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            
        }




    }
}