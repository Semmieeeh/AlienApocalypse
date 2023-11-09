using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasePitch : MonoBehaviour
{
   
    void Update()
    {
        GetComponent<AudioSource>().pitch += 0.15f * Time.deltaTime;
    }
}
