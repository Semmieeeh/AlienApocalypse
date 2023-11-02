using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamagable
{
    public void Damagable(float damage, UnityEvent onKill, UnityEvent onHit,float bulletForce,Vector3 blastDir);
}
