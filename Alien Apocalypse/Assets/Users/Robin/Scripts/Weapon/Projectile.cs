using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Data")]
    public float projectileDamage;
    public float projectileSpeed;

    Vector3 lastPos;
    Vector3 hitPoint;
    RaycastHit hit;

    public UnityEvent onHit;
    public UnityEvent onKill;

    void Update()
    {
        transform.LookAt(hitPoint);
        transform.Translate(projectileSpeed * Time.deltaTime * transform.forward);

        //Debug.Log($"CurrentPos = {transform.position} : LastPos = {lastPos}");

        if(Physics.Linecast(lastPos, transform.position, out hit))
        {
            //Debug.Log("Destroy = " + hit.point);
            onHit?.Invoke();
            Destroy(gameObject);
        }

        lastPos = transform.position;
    }

    public void InitializeProjectile(float projectileDamage, float projectileSpeed, Vector3 lastPos, Vector3 hitPoint)
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
