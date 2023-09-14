using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnemyHitsTest : MonoBehaviour
{
    public UIEnemyHits hits;

    public bool hit,headShot;

    private void Update()
    {
        if (hit)
        {
            hits.EnemyHit();
            hit = false;
        }

        if (headShot)
        {
            hits.EnemyHeadShotHit();    
            headShot = false;
        }
    }
}
