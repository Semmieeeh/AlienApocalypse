using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using Unity.Burst.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class ShootEffect : MonoBehaviour
{
    [SerializeField]
    GameObject shootRayObject;

    [SerializeField]
    GameObject hitDecal;

    [SerializeField]
    LayerMask enemyMask;

    [SerializeField]
    VisualEffect[] effects;

    [SerializeField]
    ParticleSystem[] particles;

    public virtual void Activate(bool hitEnemy, params Vector3[] hitPoints )
    {
        foreach ( var effect in effects )
        {
            effect.Play ( );
        }

        foreach ( var particle in particles )
        {
            particle.Play ( );
        }

        for ( int i = 0; i < hitPoints.Length; i++ )
        {
            VFXShootRay ray = Instantiate (shootRayObject).GetComponent<VFXShootRay> ( );

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

            VisualEffect decal = Instantiate (hitDecal, hitPoints[i], Quaternion.identity).GetComponent<VisualEffect> ( );

            decal.Play ( );

        }
    }

    public virtual void Activate (params RaycastHit[] hit )
    {
        foreach ( var effect in effects )
        {
            effect.Play ( );
        }

        foreach ( var particle in particles )
        {
            particle.Play ( );
        }

        for ( int i = 0; i < hit.Length; i++ )
        {
            VFXShootRay ray = Instantiate (shootRayObject).GetComponent<VFXShootRay> ( );

            Vector3 point = hit[i].point;

            if ( point == Vector3.zero )
            {
                point = transform.forward * 100;
            }

            if ( ray )
            {
                ray.Shoot (transform.position, point);
            }

            if(hit[i].transform && hit[i].transform.gameObject.layer == enemyMask )
                continue;

            VisualEffect decal = Instantiate (hitDecal, hit[i].point, Quaternion.identity).GetComponent<VisualEffect> ( );

            decal.transform.forward = hit[i].normal;

            decal.Play ( );

        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ShootEffect))]
[CanEditMultipleObjects]
public class ShootEffectEditor : Editor
{
    ShootEffect effect;

    public override void OnInspectorGUI ( )
    {
        base.OnInspectorGUI ( );
        effect = target as ShootEffect;

        if (GUILayout.Button("Shoot"))
        {
            Physics.Raycast (effect.transform.position, effect.transform.forward, out RaycastHit hit);

            effect.Activate (hit);
        }
    }
}
#endif
