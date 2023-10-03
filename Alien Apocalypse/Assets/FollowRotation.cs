using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public GameObject followRotation;
    public float rotationAmount;

    private void Update()
    {
     
        if(followRotation!=null)
        {
            if (transform.rotation.x < rotationAmount)
            {
                transform.rotation = followRotation.transform.rotation;
            }
            
        }
    }
}
