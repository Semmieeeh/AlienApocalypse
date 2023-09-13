using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForCollision : MonoBehaviour
{
    public bool isLeft;
    public WallRunning wr;
    private void Update()
    {
        if(wr == null)
        {
            wr = GetComponentInParent<WallRunning>();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Wall" && isLeft == true)
        {
            wr.hitLeft = true;
        }

        if (other.gameObject.tag == "Wall" && isLeft == false)
        {
            wr.hitRight = true;
        }
        
    }
    public void FixedUpdate()
    {
        wr.hitRight = false;
        wr.hitLeft = false;
    }


}
