using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField]
    VisualEffect[] effects;

    [SerializeField]
    ParticleSystem[] particles;

    public virtual void Play ( )
    {
        foreach ( var effect in effects )
        {
            effect.Play ( );
        }

        foreach ( var particle in particles )
        {
            particle.Play ( );
        }
    }
}
