using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnemyHitsTest : MonoBehaviour
{
    public UIEnemyHits hits;

    public bool test;

    private void Update()
    {
        if (test)
        {
            hits.EnemyHit();
            test = false;
        }
    }
}
