using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    private static UIPopup m_instance;

    public static UIPopup Instance
    {
        get
        {
            if ( m_instance == null )
                m_instance = FindObjectOfType<UIPopup> ( );
            return m_instance;
        }
    }
    public TextMeshProUGUI topText, mainText;

    public bool IsActive => gameObject.activeInHierarchy;

    public void Popup(string cause, string description)
    {
        this.topText.text = cause;
        this.mainText.text = description;

        gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}
