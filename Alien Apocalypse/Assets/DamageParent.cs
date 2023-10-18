using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageParent : MonoBehaviourPunCallbacks, IDamagable
{
    public float damageMultiplier;
    public EnemyHealth parent;


    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit)
    {
        DealDamage(damage * damageMultiplier ,onKill,onHit);
        parent.hitLimb = GetComponent<Rigidbody>();
    }

    public void DealDamage(float damage,UnityEvent onKill,UnityEvent onhit)
    {
        parent.Damagable(damage, onKill , onhit);
        parent.hitLimb = GetComponent<Rigidbody>();
    }
}
