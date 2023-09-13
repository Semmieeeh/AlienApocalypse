using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForCollision : MonoBehaviour
{
    public bool isLeft;
    public WallRunning wr;
    public Movement m;
    private void Update()
    {
        if(wr == null || m == null)
        {
            wr = GetComponentInParent<WallRunning>();
            m = GetComponentInParent<Movement>();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Wall" && isLeft == true && m.grounded == false)
        {
            wr.hitLeft = true;
        }

        if (other.gameObject.tag == "Wall" && isLeft == false && m.grounded == false)
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
