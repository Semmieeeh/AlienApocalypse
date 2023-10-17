using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIImageSelector : UISelectable, IOption<int>
{
    [SerializeField]
    int m_optionIndex;

    public Sprite[] choises;

    [SerializeField]
    Image valueImage;

    [SerializeField]
    int currentChoiceIndex;

    public int OptionIndex
    {
        get
        {
            return m_optionIndex;
        }
    }

    public int CurrentChoiseIndex
    {
        get
        {
            return currentChoiceIndex;
        }
        set
        {
            currentChoiceIndex = value;
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
        valueImage.sprite = choises[currentChoiceIndex];
    }

    public int GetValue ( )
    {
        return currentChoiceIndex;
    }

    public void SetValue ( int value )
    {
        currentChoiceIndex = value;
        OnChoiseChanged ( );
    }

}
