using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public GameObject followRotation;
    public float rotationAmount;
    public PlayerHealth hp;
    Quaternion q;
    private void Start()
    {
        q = transform.rotation;
    }

    private void Update()
    {
     
        if(followRotation!=null && hp.knocked == false)
        {
            if (transform.rotation.x < rotationAmount)
            {
                transform.rotation = followRotation.transform.rotation;
            }

        }
        else
        {
            //transform.localotation = q;
        }
    }
}
