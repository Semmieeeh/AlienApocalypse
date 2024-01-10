using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Children : MonoBehaviour
{

    public GameObject[] objects;
    public VisualEffect[] effects;
    public GameObject particlePivot;
    private void Start()
    {
        foreach (GameObject obj in objects)
        {
            obj.layer = 7;
        }

        if (particlePivot != null)
        {
            particlePivot.transform.localPosition = Vector3.zero;
        }

    }

}
