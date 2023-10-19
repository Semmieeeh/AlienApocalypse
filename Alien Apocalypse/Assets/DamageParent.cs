using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageParent : MonoBehaviourPunCallbacks, IDamagable
{
    public float damageMultiplier;
    public EnemyHealth parent;


    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit, float bulletForce)
    {
        DealDamage(damage * damageMultiplier ,onKill,onHit,bulletForce);
        parent.hitLimb = GetComponent<Rigidbody>();
        parent.knockBack = bulletForce;
        
    }

    public void DealDamage(float damage,UnityEvent onKill,UnityEvent onhit,float bulletforce)
    {
        parent.Damagable(damage, onKill , onhit, bulletforce);
        parent.hitLimb = GetComponent<Rigidbody>();
    }
}
