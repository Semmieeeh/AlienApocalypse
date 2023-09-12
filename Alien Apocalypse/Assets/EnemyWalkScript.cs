using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkScript : MonoBehaviour
{
    public GameObject waypoints;
    public Transform currentTarget;
    public int i;
    private Vector3 myPos;
    private Vector3 desiredPos;
    private void Start()
    {
        waypoints = GameObject.Find("Waypoints");
        currentTarget = waypoints.transform.GetChild(0);
        
    }
    void Update()
    {
        myPos = transform.position;
        Navigation();
    }

    public void Navigation()
    {
        if(Vector3.Distance(transform.position, currentTarget.position) > 2)
        {
            
            
            desiredPos = currentTarget.position;
            Vector3 newPos = Vector3.Lerp(myPos, desiredPos, 5f *Time.deltaTime);
            transform.position = newPos;
        }
        else if(Vector3.Distance(transform.position, currentTarget.position) < 2)
        {
            i++;
            
            if (i >= waypoints.transform.childCount)
            {
                i = 0;
                
            }
            currentTarget = waypoints.transform.GetChild(i);
        }

        
        
    }
}
