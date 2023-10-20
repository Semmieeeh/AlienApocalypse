using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : UISelectable, IOption<float>
{
    [SerializeField]
    int m_optionIndex;


    [SerializeField]
    TextMeshProUGUI minText, maxText,valueText;

    [SerializeField]
    Slider slider;

    public int OptionIndex
    {
        get
        {
            return m_optionIndex;
        }
    }

    protected override void Start ( )
    {
        base.Start ( );
        UpdateSlider ( );
    }

    public void UpdateSlider ( )
    {
        string format = slider.wholeNumbers ? "F0" : "F1";


        minText.text = slider.minValue.ToString(format);

        maxText.text = slider.maxValue.ToString (format);

        valueText.text = slider.value.ToString (format);
    }

    public float GetValue ( )
    {
        return slider.value;
    }

    public void SetValue(float value )
    {
        slider.value = value;
        UpdateSlider ( );
    }
}
