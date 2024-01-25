using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseManager : MonoBehaviour
{
    private static UIPauseManager m_instance;

    public static UIPauseManager Instance
    {
        get
        {
            if ( m_instance == null )
                m_instance = FindObjectOfType<UIPauseManager> ( );
            return m_instance;
        }

    }
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
        set
        {
            if(value == m_Paused) return;
            m_Paused = value;

            OnPauseStateChanged (m_Paused );
            HandleFreezeScreen ( );

        }
    }

    private bool _freezeScreen;

    public bool FreezeScreen
    {
        get
        {
            return _freezeScreen;
        }
        set
        {
            if ( value == _freezeScreen )
                return;
            _freezeScreen = value;

            HandleFreezeScreen ( );

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

            HandleFreezeScreen ( );
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

    void HandleFreezeScreen ( )
    {
        bool freeze = FreezeScreen || Paused;

        Debug.Log (FreezeScreen);
        Debug.Log (Paused);

        float t;
        if ( !freeze )
        {
            t = 1;
        }
        else
        {
            t = 0;
        }
        player.GetComponent<Movement> ( ).enabled = !freeze;
        player.GetComponent<Grappling> ( ).enabled = !freeze;
        player.GetComponent<DashAbility> ( ).enabled = !freeze;
        player.GetComponent<SlidingAbility> ( ).enabled = !freeze;
        player.GetComponent<GrappleRope> ( ).enabled = !freeze;
        weaponInput.GetComponent<WeaponInputHandler> ( ).enabled = !freeze;
        cam.GetComponent<MouseLook> ( ).enabled = !freeze;
        Time.timeScale = t;
    }

    void OnPauseStateChanged(bool paused )
    {
        
        m_inOptions = false;
        pauseMenu.SetActive (paused);

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
