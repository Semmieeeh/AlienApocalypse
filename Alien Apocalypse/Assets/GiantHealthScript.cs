using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GiantHealthScript : MonoBehaviour
{
    public float health;
    float knockback;
    float maxhealth = 500;
    float minhealth = 0;
    private void Start()
    {
        health = maxhealth;
    }
    private void Update()
    {
        if (health <= minhealth)
        {
            Destroy(gameObject);
        }
    }
    

}
