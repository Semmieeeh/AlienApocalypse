using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;


    [SerializeField]
    UIPopup m_Popup;

    [SerializeField]
    GameObject mainMenu, playSection, settingsSection,controlsSection, creditsSection, quitSection;

    public UIPopup Popup
    {
        get
        {
            if (m_Popup == null) m_Popup = FindObjectOfType<UIPopup>();
            return m_Popup;
        }
        set
        {
            m_Popup = value;
        }
    }

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update ( )
    {
        Debug.Log (PlayerName.NickName);
    }
    public void PopupMessage(string description)
    {
        PopupMessage("Important", description);
    }
    public void PopupMessage(string cause, string description)
    {
        if (Popup == null) throw new System.NullReferenceException(nameof(Popup));

        Popup.Popup(cause, description);
    }

    public void ToMainMenu()
    {
        ToggleSection(mainMenu);
    }

    public void ToPlaySection()
    {
        ToggleSection(playSection);
    }

    public void ToSettingsSection()
    {
        ToggleSection(settingsSection);
    }

    public void ToControlsSection ( )
    {
        ToggleSection (controlsSection);
    }

    public void ToCreditsSection()
    {
        ToggleSection(creditsSection);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void ToggleSection(GameObject toEnable)
    {
        var menus = new[]
        {
            mainMenu, playSection, settingsSection, controlsSection, creditsSection, quitSection
        };

        for (int i = 0; i < menus.Length; i++)
        {
            var m = menus[i];

            if (m == null) continue;

            if (m == toEnable)
            {
                m.SetActive(true);
            }
            else
            {
                m.SetActive(false);
            }
        }
    }
}
