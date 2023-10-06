using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Data")]
    public float projectileDamage;
    public float projectileSpeed;
    public float radius;
    public LayerMask mask;

    Vector3 lastPos;
    public Vector3 hitPoint;
    RaycastHit hit;

    public UnityEvent onHit;
    public UnityEvent onKill;

    void Update()
    {
        transform.LookAt(hitPoint);
        transform.Translate(projectileSpeed * Time.deltaTime * Vector3.forward);

        if(Physics.Linecast(lastPos, transform.position, out hit))
        {
            Collider[] collider = Physics.OverlapSphere(transform.position, radius, mask);
            
            foreach(Collider col in collider)
            {
                if(col.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    damagable.Damagable(projectileDamage, onKill, onHit);
                }
            }

            PhotonNetwork.Destroy(gameObject);
        }

        lastPos = transform.position;
    }

    public void InitializeProjectile(float projectileDamage, float projectileSpeed, float radius, Vector3 lastPos, Vector3 hitPoint)
    {
        this.projectileDamage = projectileDamage;
        this.projectileSpeed = projectileSpeed;
        this.lastPos = lastPos;
        this.hitPoint = hitPoint;
    }

    public void InitialzieEvent(UnityEvent onHit, UnityEvent onKill)
    {
        this.onHit = onHit;
        this.onKill = onKill;
    }
}
