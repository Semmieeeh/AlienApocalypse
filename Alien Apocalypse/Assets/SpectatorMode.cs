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
            myCam.gameObject.SetActive(false);
            renderCam.gameObject.SetActive(false);
            GameObject spec = GameObject.FindGameObjectWithTag("Spectator");
            spec.GetComponent<Camera>().enabled = true;

        }
        else
        {
            myCam.gameObject.SetActive(true);
            renderCam.gameObject.SetActive(false);
            GameObject spec = GameObject.FindGameObjectWithTag("Spectator");
            spec.GetComponent<Camera>().enabled = false;
        }
    }
}
