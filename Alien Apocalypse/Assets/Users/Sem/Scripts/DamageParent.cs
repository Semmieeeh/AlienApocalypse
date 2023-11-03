using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DamageParent : MonoBehaviourPunCallbacks, IDamagable
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
        DealDamage(damage * damageMultiplier ,onKill,onHit,bulletForce,blastDir);
        parent.hitLimb = GetComponent<Rigidbody>();
        parent.knockBack = bulletForce;
        parent.blastDirection = blastDir;
    }

    public void DealDamage(float damage,UnityEvent onKill,UnityEvent onhit,float bulletforce,Vector3 blastDir)
    {
        parent.Damagable(damage, onKill , onhit, bulletforce,blastDir);
        parent.hitLimb = GetComponent<Rigidbody>();
    }
    
}
