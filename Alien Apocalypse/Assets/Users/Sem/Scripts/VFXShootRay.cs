using UnityEngine;

[ExecuteInEditMode]
public class VFXShootRay : MonoBehaviour
{
    [SerializeField]
    LineRenderer linerenderer;

    [SerializeField]
    float duration;

    [SerializeField]
    bool moveToTarget = true;

    [SerializeField]
    bool dampenRay;

    float timer;

    Vector3 endPos;
    public float Progress
    {
        get => Mathf.InverseLerp (duration, 0, timer);
    }

    bool inited;
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

        this.endPos = endPos;

        linerenderer.widthMultiplier *= 2;
        inited = true;
    }

    private void Update ( )
    {
        if ( !inited )
            return;

        Debug.Log ("he hu");

        if(dampenRay)
            linerenderer.sharedMaterial.SetFloat ("_Progress", Progress);

        timer += Time.deltaTime;

        if ( moveToTarget )
        {

            var newEndpos = Vector3.Lerp (endPos, linerenderer.GetPosition (1), Progress * Time.deltaTime);

            linerenderer.SetPosition (0, newEndpos);
        }

        if ( timer >= duration )
        {
            if ( Application.isPlaying )
                Destroy (gameObject);
            else
            {
                DestroyImmediate (gameObject);
            }
        }
    }
}
