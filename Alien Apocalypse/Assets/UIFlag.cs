using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlag : UITrackable
{
    public float maxValue;
    public float currentValue;


    [SerializeField]
    Image coloredBorder;

    public float Progress => Mathf.InverseLerp (0, maxValue, currentValue);

    protected override void Update ( )
    {
        base.Update ( );
        coloredBorder.fillAmount = Progress;
    }
}
