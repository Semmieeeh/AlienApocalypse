using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIToggle : UISelectable, IOption<bool>
{
    [SerializeField]
    int m_optionIndex;


    [SerializeField]
    Image m_Border;

    [SerializeField]
    Image m_check;

    [SerializeField]
    ColorBlock block;

    [SerializeField]
    bool m_isOn;

    public UnityEvent<bool> onValueChanged;

    public bool IsOn
    {
        get
        {
            return m_isOn;
        }
        set
        {
            if (value == m_isOn) return;
            m_isOn = value;

            OnValueChanged(m_isOn);
        }
    }

    public int OptionIndex
    {
        get
        {
            return m_optionIndex;
        }
    }

    protected override void Start ( )
    {
        OnValueChanged ( IsOn);
    }

    protected override void Update()
    {
        base.Update();

        m_Border.color = block.GetColor(this);
    }

    public override void OnClickEnd()
    {
        IsOn = !IsOn;
    }

    void OnValueChanged(bool newValue)
    {
        m_check.enabled = newValue;
        onValueChanged?.Invoke(newValue);
    }

    public bool GetValue ( )
    {
        return IsOn;
    }

    public void SetValue(bool value )
    {
        IsOn = value;
    }
}
