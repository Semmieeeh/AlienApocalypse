using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DamageParent : MonoBehaviour, IDamagable
{
    public float damageMultiplier;
    public EnemyHealth parent;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit, float bulletForce, Vector3 blastDir)
    {

        parent.hitLimb = rb;
        parent.knockBack = bulletForce;
        parent.blastDirection = blastDir;
        DealDamage(damage * damageMultiplier, onKill, onHit, bulletForce, blastDir);
    }

    public void DealDamage(float damage,UnityEvent onKill,UnityEvent onhit,float bulletforce,Vector3 blastDir)
    {
        
        parent.hitLimb = GetComponent<Rigidbody>();
        parent.Damagable(damage, onKill, onhit, bulletforce, blastDir);
    }
    
}
