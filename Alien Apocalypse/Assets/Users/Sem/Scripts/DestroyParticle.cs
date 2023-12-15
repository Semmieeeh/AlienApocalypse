using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public float time;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(time);

        if(Application.isPlaying)
            Destroy(gameObject);
        else
        {
            DestroyImmediate (gameObject);
        }
    }


}
