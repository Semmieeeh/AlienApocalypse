using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOption : UISelectable
{
    [Header("Option Data")]
    [SerializeField]
    string m_optionName;

    [SerializeField]
    string m_optionDescription;

    public string OptionName
    {
        get => m_optionName;
    }

    public string OptionDescription
    {
        get => m_optionDescription;
    }

    [SerializeField]
    OptionDisplay m_display;

    public OptionDisplay OptionDisplay
    {
        get
        {
            if(m_display == null) m_display = FindObjectOfType<OptionDisplay>();
            return m_display;
        }
        set
        {
            m_display = value;
        }
    }

    public override void OnHoverEnter()
    {
        OptionDisplay.LoadOption(OptionName, OptionDescription);
    }
}
