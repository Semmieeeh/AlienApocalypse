using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconManager : MonoBehaviour
{
    public List<Beacon> beacons;


    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).TryGetComponent<Beacon>(out Beacon beacon))
            {
                beacon.beaconManager = this;
                beacons.Add(beacon);
            }
        }    
    }

    public void BeaconsCondition()
    {
        int enemyScore = 0;
        int playerScore = 0;


        for(int i = 0; i < beacons.Count; i++)
        {
            if(beacons[i].totalScore == -100)
            {
                enemyScore += beacons[i].totalScore;
            }
            else if(beacons[i].totalScore == 100)
            {
                playerScore += beacons[i].totalScore;
            }
        }

    }
}
