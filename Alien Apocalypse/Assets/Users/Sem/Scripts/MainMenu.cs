using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField]
    UIPopup m_Popup;

    [SerializeField]
    GameObject mainMenu, playSection, settingsSection,controlsSection, creditsSection, quitSection;

    [SerializeField]
    Material transition;

    [SerializeField]
    float transitionTime;

    float currentTransitionTime;

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
        if(currentTransitionTime <= transitionTime)
            currentTransitionTime += Time.deltaTime;

        if(transition)
            transition.SetFloat ("_Progress", Mathf.InverseLerp (0, transitionTime, currentTransitionTime));

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



    private async void ToggleSection(GameObject toEnable)
    {
        var menus = new[]
        {
            mainMenu, playSection, settingsSection, controlsSection, creditsSection, quitSection
        };

        currentTransitionTime = 0;
        await Task.Delay ( Mathf.RoundToInt(transitionTime / 2 * 1000));

        for (int i = 0; i < menus.Length; i++)
        {
            var m = menus[i];

            if (m == null) continue;

            m.SetActive(m == toEnable);
        }
    }
}
