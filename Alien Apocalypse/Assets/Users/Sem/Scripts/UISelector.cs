using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISelector : UISelectable, IOption<int>
{
    [SerializeField]
    int m_optionIndex;

    public string[] choises;

    [SerializeField]
    TextMeshProUGUI valueText;

    [SerializeField]
    int currentChoiceIndex;

    public int OptionIndex
    {
        get
        {
            return m_optionIndex;
        }
    }

    protected override void Start ( )
    {
        OnChoiseChanged ( );
    }

    public void PreviousChoise ( )
    {
        currentChoiceIndex--;
        if ( currentChoiceIndex < 0 )
            currentChoiceIndex = choises.Length - 1;

        OnChoiseChanged ( );
    }

    public void NextChoice ( )
    {
        currentChoiceIndex++;
        if ( currentChoiceIndex >= choises.Length )
            currentChoiceIndex = 0;

        OnChoiseChanged ( );
    }

    void OnChoiseChanged ( )
    {
        valueText.text = choises[currentChoiceIndex];
    }

    public int GetValue ( )
    {
        return currentChoiceIndex;
    }

    public void SetValue(int value )
    {
        currentChoiceIndex = value;
        OnChoiseChanged ( );
    }
}
