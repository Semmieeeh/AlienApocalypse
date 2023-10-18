using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    public Image barImg, differenceImg,flashImg;

    public float maxValue;

    public float CurrentValue { get; private set; }

    [SerializeField]
    float barFilSpeed;

    [SerializeField]
    float differenceDelay;

    public float currentTimer;

    public float barProgress, diffProgress;
    void Update()
    {
        currentTimer += Time.deltaTime;

        barImg.fillAmount = Mathf.InverseLerp (0, maxValue, CurrentValue);

        flashImg.fillAmount = barImg.fillAmount;

        var c = flashImg.color;

        c.a = Mathf.Lerp (1, 0, Mathf.InverseLerp (0, barFilSpeed / 2, currentTimer));

        flashImg.color = c;


        if ( currentTimer >= differenceDelay )
        {
            diffProgress = Mathf.InverseLerp (differenceDelay, differenceDelay + barFilSpeed, currentTimer);

            differenceImg.fillAmount = Mathf.Lerp(differenceImg.fillAmount, Mathf.InverseLerp (0, maxValue, CurrentValue), diffProgress);
        }
    }

    public void SetValue(float newValue )
    {
        if ( newValue == CurrentValue )
            return;

        CurrentValue = newValue;

        currentTimer = 0;
    }
}
