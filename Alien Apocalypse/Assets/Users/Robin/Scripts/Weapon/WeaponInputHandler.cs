using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInputHandler : MonoBehaviour
{
    public UnityEvent primaryButton;
    public UnityEvent secondaryButton;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            primaryButton?.Invoke();
        }

        if(Input.GetButton("Fire2"))
        {
            secondaryButton?.Invoke();
        }
    }
}
