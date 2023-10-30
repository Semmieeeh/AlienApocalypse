using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMode : MonoBehaviour
{

    bool isSpectator;

    public bool IsSpectator
    {
        get { return isSpectator; }
        set {
            if (value == isSpectator)
                return;
            isSpectator = value;
            OnValueChanged();
        }

    }
    public GameObject myCam,renderCam;

    public void OnValueChanged()
    {
        if (isSpectator)
        {
            List<PlayerHealth> playerList = new List<PlayerHealth>();
            playerList.Add(GameObject.FindObjectOfType<PlayerHealth>());
            foreach (PlayerHealth playerHealth in playerList)
            {
                if(playerHealth != GetComponent<PlayerHealth>())
                {
                    Vector3 pos = playerHealth.gameObject.GetComponent<Movement>().cameraPivot.transform.position;
                    Camera.main.gameObject.transform.position = pos;
                    Camera.main.gameObject.transform.parent = playerHealth.gameObject.GetComponent<Movement>().cameraPivot.transform;
                }
            }
        }
        else
        {
            List<PlayerHealth> playerList = new List<PlayerHealth>();
            playerList.Add(GameObject.FindObjectOfType<PlayerHealth>());
            foreach (PlayerHealth playerHealth in playerList)
            {
                if (playerHealth == GetComponent<PlayerHealth>())
                {
                    Vector3 pos = playerHealth.gameObject.GetComponent<Movement>().cameraPivot.transform.position;
                    Camera.main.gameObject.transform.position = pos;
                    Camera.main.gameObject.transform.parent = playerHealth.gameObject.GetComponent<Movement>().cameraPivot.transform;
                }
            }
        }
    }
}
