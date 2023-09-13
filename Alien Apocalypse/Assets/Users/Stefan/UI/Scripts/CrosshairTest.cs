using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTest : MonoBehaviour
{
    [SerializeField]
    CrosshairManager manager;

    [SerializeField]
    float power, bloom;

    public bool shoot;

    private void Update()
    {
        if (shoot || Input.GetMouseButtonDown(0))
        {
            shoot = false;
            Shoot();
        }
    }
    public void Shoot()
    {
        manager.SetBloomAndPower(bloom, power);
        manager.Shoot();
    }
}
