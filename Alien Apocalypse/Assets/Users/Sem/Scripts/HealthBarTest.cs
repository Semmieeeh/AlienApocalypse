using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarTest : MonoBehaviour
{
    [SerializeField]
    HealthBar healthBar;

    public float value;

    // Update is called once per frame
    void Update()
    {
        healthBar.SetValue (value);
    }
}
