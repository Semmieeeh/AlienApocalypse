using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXShootRay : MonoBehaviour
{
    [SerializeField]
    LineRenderer linerenderer;

    [SerializeField]
    float duration;

    float timer;

    public float Progress{
        get => Mathf.InverseLerp (duration, 0, timer);
    }

    private void Start ( )
    {
        linerenderer = GetComponent<LineRenderer> ( );
    }

    /// <summary>
    /// Will activate the shoot ray effect from startpos to endpos
    /// </summary>
    /// <param name="startPos">The origin of the ray, mostly the gun barrel for example</param>
    /// <param name="endPos">The endpoint of the ray, like a wall</param>
    public void Shoot ( Vector3 startPos, Vector3 endPos )
    {
        linerenderer = GetComponent<LineRenderer> ( );

        linerenderer.SetPosition (0, startPos);
        linerenderer.SetPosition (1, endPos);
    }

    private void Update ( )
    {
        linerenderer.material.SetFloat ("_Progress", Progress);

        timer += Time.deltaTime;

        if(timer >= duration )
        {
            Destroy (gameObject);
        }
    }
}
