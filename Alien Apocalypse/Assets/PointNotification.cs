using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointNotification : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI pointTexts, messageText;

    [SerializeField]
    bool active;

    private Animator m_animator;

    public bool Active
    {
        get
        {
            return active;
        }
    }

    Animator Animator
    {
        get
        {
            if (m_animator == null) m_animator = GetComponent<Animator>();
            return m_animator;
        }
        set
        {
            m_animator = value;
        }
    }

    public void Notify(int points, string message)
    {
        pointTexts.text = "+ " +  points.ToString();

        messageText.text = message;

        active = true;
        Animator.SetTrigger("Start");
    }
}
