using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformativePopup : MonoBehaviour
{

    public PopUpData[] popups;

    [SerializeField]
    Image spriteImage;

    [SerializeField]
    TextMeshProUGUI messageText;

    [SerializeField]
    Image animationImage;

    [SerializeField]
    bool active;

    private Animator m_Animator;

    Animator Animator
    {
        get
        {
            if (m_Animator == null) m_Animator = GetComponent<Animator>();
            return m_Animator;
        }
        set
        {
            m_Animator = value;
        }
    }

    public bool Active
    {
        get
        {
            return active;
        }
    }

    public void Popup(InformativePopUpType type)
    {
        Popup(popups[(int)type]);
    }

    void Popup(PopUpData popup)
    {
        spriteImage.sprite = popup.sprite;
        spriteImage.color = popup.color;

        messageText.text = popup.message;
        messageText.color = popup.color;

        animationImage.color = popup.color;

        active = true;
        Animator.SetTrigger("Toggle");
    }

    [System.Serializable]
    public struct PopUpData
    {
        public Color color;
        public string message;
        public Sprite sprite;
    } 
}

public enum InformativePopUpType
{
    Kill,
    Revive,
    Other
}
