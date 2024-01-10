using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject weaponInput;
    public GameObject ui;
    public GameObject deathScreen;
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
        
        SceneManager.LoadScene(0);
    }


    void OnPauseStateChanged(bool paused )
    {
        pauseMenu.SetActive (paused);
        player.GetComponent<Movement>().enabled = !paused;
        player.GetComponent<Grappling>().enabled = !paused;
        player.GetComponent<DashAbility>().enabled = !paused;
        player.GetComponent<SlidingAbility>().enabled = !paused;
        player.GetComponent<GrappleRope>().enabled = !paused;
        weaponInput.GetComponent<WeaponInputHandler>().enabled = !paused;
        cam.GetComponent<MouseLook>().enabled = !paused;

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
