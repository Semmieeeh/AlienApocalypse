using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseManager : MonoBehaviour
{
    [SerializeField]
    KeyCode pauseKey = KeyCode.Escape;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject optionsMenu;

    private bool m_Paused;
    public GameObject player;
    public GameObject cam;
    public bool Paused
    {
        get
        {
            return m_Paused;
        }
        private set
        {
            if(value == m_Paused) return;
            m_Paused = value;

            OnPauseStateChanged (m_Paused );
        }
    }

    private bool m_inOptions;

    public bool InOptions
    {
        get
        {
            return m_inOptions;
        }
        private set
        {
            if ( value == m_inOptions )
                return;
            m_inOptions = value;

            OnOptionsStateChanged (m_inOptions );
        }
    }

    private void Update ( )
    {
        if (Input.GetKeyUp(pauseKey))
        {
            Paused = !Paused;
        }

        player.GetComponent<Movement>().enabled = !Paused;
        player.GetComponent<Grappling>().enabled = !Paused;
        player.GetComponent<DashAbility>().enabled = !Paused;
        player.GetComponent<WallRunning>().enabled = !Paused;
        player.GetComponent<SlidingAbility>().enabled = !Paused;
        player.GetComponent<GrappleRope>().enabled = !Paused;
        cam.GetComponent<MouseLook>().enabled = !Paused;
    }

    public void DisableMenu ( )
    {
        Paused = false;
    }

    public void EnableOptions ( )
    {
        InOptions = true;
    }

    public void DisableOptions ( )
    {
        InOptions = false;  
    }

    public void ToMainMenu ( )
    {
        throw new System.NotImplementedException ( );
    }


    void OnPauseStateChanged(bool paused )
    {
        pauseMenu.SetActive (paused);

        m_inOptions = false;

        optionsMenu.SetActive (false);

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    void OnOptionsStateChanged(bool inOptions )
    {
        optionsMenu.SetActive (inOptions);
        pauseMenu.SetActive (!inOptions);
    }
}
