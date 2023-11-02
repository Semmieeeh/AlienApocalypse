using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Data")]
    public float projectileDamage;
    public float projectileSpeed;
    public float radius;
    public LayerMask mask;

    public GameObject explosion;
    GameObject exp;
    public float explosionTime;

    Vector3 lastPos;
    public Vector3 hitPoint;
    RaycastHit hit;

    float startTime;
    public float timeToDestroy;

    public UnityEvent onHit;
    public UnityEvent onKill;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        //transform.LookAt(hitPoint);
        transform.Translate(projectileSpeed * Time.deltaTime * Vector3.forward);

        if(Physics.Linecast(lastPos, transform.position, out hit))
        {
            if(hit.transform.gameObject != gameObject)
            {
                Collider[] collider = Physics.OverlapSphere(transform.position, radius, mask);

                foreach(Collider col in collider)
                {
                    if(col.TryGetComponent<IDamagable>(out IDamagable damagable))
                    {
                        damagable.Damagable(projectileDamage, onKill, onHit, 30);
                    }
                }

                exp = PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);

                Invoke(nameof(DestroyGameObject), explosionTime);
            }
        }

        if(Time.time - startTime > timeToDestroy)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        lastPos = transform.position;
    }

    void DestroyGameObject()
    {
        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Destroy(exp);

    }

    public void InitializeProjectile(float projectileDamage, float projectileSpeed, float radius, Vector3 lastPos, Vector3 hitPoint)
    {
        this.projectileDamage = projectileDamage;
        this.projectileSpeed = projectileSpeed;
        this.radius = radius;

        this.lastPos = lastPos;
        this.hitPoint = hitPoint;
    }

    public void InitialzieEvent(UnityEvent onHit, UnityEvent onKill)
    {
        this.onHit = onHit;
        this.onKill = onKill;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}