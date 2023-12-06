using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var c = GetComponent<Canvas> ( );
        c.worldCamera = Camera.main;
        c.planeDistance = 0.5F;
    }
}
