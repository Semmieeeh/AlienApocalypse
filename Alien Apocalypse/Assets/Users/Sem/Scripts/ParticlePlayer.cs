using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField]
    VisualEffect[] effects;

    [SerializeField]
    ParticleSystem[] particles;

    public bool playOnStart;

    [Header("Random Settings")]
    public bool playRandom;

    public float randomTimer;


    private void Start ( )
    {
        if ( playOnStart )
        {
            Play ( );
        }

        if ( playRandom )
        {
            StartCoroutine (PlayRandom ( ));
        }
    }


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

    IEnumerator PlayRandom ( )
    {
        if ( playRandom == false )
            yield break;
            

        effects.Random ( )?.Play ( );
        particles.Random ( )?.Play ( );

        yield return new WaitForSeconds (randomTimer);
        yield return StartCoroutine (PlayRandom ( ));
    }
}
