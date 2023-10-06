using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    VisualEffect[] effects;

    [SerializeField]
    ParticleSystem[] particles;

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


            if ( ray )
            {
                ray.Shoot (transform.position, hit[i].point);
            }

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
