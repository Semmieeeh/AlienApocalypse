using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.VFX;

public class Children : MonoBehaviourPunCallbacks
{

    public GameObject[] objects;
    public VisualEffect[] effects;
    public GameObject particlePivot;
    private void Start()
    {
        if (photonView.IsMine)
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

}
