using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ShootEffect : ParticlePlayer
{
    [SerializeField]
    GameObject shootRayObject;

    [SerializeField]
    GameObject hitDecal;

    [SerializeField]
    LayerMask enemyMask;

    public virtual void Activate ( bool hitEnemy, Vector3 normal, params Vector3[] hitPoints )
    {
        Play ( );

        for ( int i = 0; i < hitPoints.Length; i++ )
        {
            VFXShootRay ray = PhotonNetwork.Instantiate (shootRayObject.name,transform.position,Quaternion.identity).GetComponent<VFXShootRay> ( );

            Vector3 point = hitPoints[i];

            if ( point == Vector3.zero )
            {
                point = transform.forward * 100;
            }

            if ( ray )
            {
                ray.Shoot (transform.position, point);
            }

            if ( hitEnemy )
                continue;

            VisualEffect decal = PhotonNetwork.Instantiate (hitDecal.name, hitPoints[i], Quaternion.identity).GetComponent<VisualEffect> ( );
            decal.transform.forward = normal;
            decal.Play ( );

        }
    }

    public virtual void Activate ( params RaycastHit[] hit )
    {
        Play ( );

        for ( int i = 0; i < hit.Length; i++ )
        {
            VFXShootRay ray = PhotonNetwork.Instantiate (shootRayObject.name,transform.position,Quaternion.identity).GetComponent<VFXShootRay> ( );

            Vector3 point = hit[i].point;

            if ( point == Vector3.zero )
            {
                point = transform.forward * 100;
            }

            if ( ray )
            {
                ray.Shoot (transform.position, point);
            }

            if ( hit[i].transform && hit[i].transform.gameObject.layer == enemyMask )
                continue;

            VisualEffect decal = PhotonNetwork.Instantiate (hitDecal.name, hit[i].point, Quaternion.identity).GetComponent<VisualEffect> ( );

            decal.transform.forward = hit[i].normal;

            decal.Play ( );

        }
    }
    public virtual void Activate ( bool hitEnemy, RaycastHit hit )
    {
        Play ( );
        VFXShootRay ray = PhotonNetwork.Instantiate(shootRayObject.name, transform.position, Quaternion.identity).GetComponent<VFXShootRay> ( );

        Vector3 point = hit.point;

        if ( point == Vector3.zero )
        {
            point = transform.forward * 100;
        }

        if ( ray )
        {
            ray.Shoot (transform.position, point);
        }

        VisualEffect decal = PhotonNetwork.Instantiate (hitDecal.name, point, Quaternion.identity).GetComponent<VisualEffect> ( );

        decal.transform.forward = hit.normal;

        decal.Play ( );


    }
}

#if UNITY_EDITOR

[CustomEditor (typeof (ShootEffect))]
[CanEditMultipleObjects]
public class ShootEffectEditor : Editor
{
    ShootEffect effect;

    public override void OnInspectorGUI ( )
    {
        base.OnInspectorGUI ( );
        effect = target as ShootEffect;

        if ( GUILayout.Button ("Shoot") )
        {
            Physics.Raycast (effect.transform.position, effect.transform.forward, out RaycastHit hit);

            effect.Activate (hit);
        }
    }
}
#endif
